using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EAMS_ACore.SMSModels;
using EAMS_DAL.DBContext;
using EAMS_ACore.IExternal;
using LBPAMS.Helper.ProjectRequestDtos;
using LBPAMS.Helper;
using Microsoft.VisualBasic.FileIO;
using LBPAMS.ResponseDTOs;
using Mono.TextTemplating;

namespace LBPAMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SMSController : ControllerBase
    {
        private readonly SMSContext _context;
        private readonly EamsContext _eamsContext;
        private readonly IExternal _externalService;

        public SMSController(SMSContext context, IExternal external, EamsContext eamsContext)
        {
            _context = context;
            _externalService = external;
            _eamsContext = eamsContext;
        }

        // GET: api/SMS
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SMSConfigurationResponse>>> GetSMSConfiguration()
        {
            // Step 1: Load district list with StateName into memory
            var districtList = await _eamsContext.DistrictMaster
                .Include(d => d.StateMaster)
                .Select(d => new
                {
                    d.DistrictMasterId,
                    d.DistrictName,
                    d.StateMasterId,
                    d.StateMaster.StateName
                })
                .ToListAsync();

            // Step 2: Load SMS configurations
            var smsList = await _context.SMSConfiguration.ToListAsync();

            // Step 3: Join in-memory
            var result = smsList.Select(sms =>
            {
                var district = districtList.FirstOrDefault(d => d.DistrictMasterId == sms.DistrictMasterId);
                return new SMSConfigurationResponse
                {
                    Id = sms.Id,
                    StateMasterId = sms.StateMasterId,
                    StateName = district?.StateName,
                    DistrictMasterId = sms.DistrictMasterId,
                    DistrictName = district?.DistrictName,
                    UserName = sms.UserName,
                    Password = sms.Password,
                    SenderId = sms.SenderId,
                    TemplateId = sms.TemplateId,
                    EntityId = sms.EntityId,
                    Message = sms.Message
                };
            }).ToList();

            return Ok(result);
        }

        // GET: api/SMS/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetSMSConfiguration(int id)
        {
            var sMSConfiguration = await _context.SMSConfiguration.FindAsync(id);

            if (sMSConfiguration == null)
            {
                return NotFound();
            }

            var district = await _eamsContext.DistrictMaster
                .Where(d => d.DistrictMasterId == sMSConfiguration.DistrictMasterId)
                .Select(d => new
                {
                    d.DistrictName,
                    d.StateMaster.StateName
                })
                .FirstOrDefaultAsync();

            var result = new
            {
                Id = sMSConfiguration.Id,
                StateMasterId = sMSConfiguration.StateMasterId,
                DistrictMasterId = sMSConfiguration.DistrictMasterId,
                UserName = sMSConfiguration.UserName,
                Password = sMSConfiguration.Password,
                SenderId = sMSConfiguration.SenderId,
                TemplateId = sMSConfiguration.TemplateId,
                EntityId = sMSConfiguration.EntityId,
                Message = sMSConfiguration.Message,
                StateName = district?.StateName,
                DistrictName = district?.DistrictName
            };

            return Ok(result);
        }


        // PUT: api/SMS/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSMSConfiguration(int id, SMSConfiguration sMSConfiguration)
        {
            if (id != sMSConfiguration.Id)
            {
                return BadRequest();
            }

            _context.Entry(sMSConfiguration).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SMSConfigurationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/SMS
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SMSConfiguration>> PostSMSConfiguration(SMSConfiguration sMSConfiguration)
        {
            _context.SMSConfiguration.Add(sMSConfiguration);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSMSConfiguration", new { id = sMSConfiguration.Id }, sMSConfiguration);
        }

        // DELETE: api/SMS/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSMSConfiguration(int id)
        {
            var sMSConfiguration = await _context.SMSConfiguration.FindAsync(id);
            if (sMSConfiguration == null)
            {
                return NotFound();
            }

            _context.SMSConfiguration.Remove(sMSConfiguration);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SMSConfigurationExists(int id)
        {
            return _context.SMSConfiguration.Any(e => e.Id == id);
        }

        [HttpPost("SendBulkSMS")]
        public async Task<IActionResult> SendBulkSMS([FromBody] ProjectRequestDto request)
        {
            if (request == null || request.StateMasterId <= 0)
            {
                return BadRequest("Invalid request data.");
            }
            var result = await _externalService.SendBulkSMS(request.StateMasterId, request.DistrictMasterId);
            return Ok(new { message = result });
        }

        [HttpPost("UploadSMSContacts")]
        public async Task<IActionResult> UploadSMSContacts([FromForm] UploadSmsCsvRequest request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("No file uploaded.");

            var extension = Path.GetExtension(request.File.FileName).ToLower();
            if (extension != ".csv")
                return BadRequest("Only CSV files are allowed.");

            var result = new List<SMSNumbers>();

            using (var parser = new TextFieldParser(request.File.OpenReadStream()))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                int lineNumber = 0;

                while (!parser.EndOfData)
                {
                    lineNumber++;
                    var fields = parser.ReadFields();

                    if (fields == null || fields.Length != 2)
                        return BadRequest($"Line {lineNumber} must contain exactly 2 columns: Name and Number.");

                    if (lineNumber == 1) continue; // skip header

                    var name = fields[0].Trim();

                    var rawNumber = fields[1].Trim();
                    var digitsOnly = new string(rawNumber.Where(char.IsDigit).ToArray());

                    // Remove leading country code if present
                    if (digitsOnly.StartsWith("91") && digitsOnly.Length > 10)
                    {
                        digitsOnly = digitsOnly.Substring(digitsOnly.Length - 10); // take last 10 digits
                    }

                    if (digitsOnly.Length != 10)
                        return BadRequest($"Invalid mobile number at line {lineNumber}: {fields[1]}");

                    var number = digitsOnly;


                    if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(number))
                        return BadRequest($"Empty value found at line {lineNumber}. Both Name and Number are required.");

                    result.Add(new SMSNumbers
                    {
                        Name = name,
                        Number = number,
                        StateMasterId = request.StateMasterId,
                        DistrictMasterId = request.DistrictMasterId,
                    });
                }
            }

            await _context.SMSNumbers.AddRangeAsync(result);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                IsSucced = true,
                Message = $"{result.Count} records Uploaded successfully",
            });
        }

        [HttpGet("GetSMSNumbersList")]
        public async Task<ActionResult> GetSMSNumbersList(
    int stateMasterId,
    int districtMasterId,
    int pageNumber = 1,
    int pageSize = 100)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0 || pageSize > 1000) pageSize = 100; // limit max size

            var query = _context.SMSNumbers
                .Where(d => d.StateMasterId == stateMasterId && d.DistrictMasterId == districtMasterId);

            var totalRecords = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            var result = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var response = new
            {
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                Data = result
            };

            return Ok(response);
        }

        [HttpDelete("DeleteSMSNumbers")]
        public async Task<IActionResult> DeleteSMSNumbers(int id)
        {
            var sMSNumbers = await _context.SMSNumbers.FindAsync(id);
            if (sMSNumbers == null)
            {
                return NotFound();
            }
            _context.SMSNumbers.Remove(sMSNumbers);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("DeleteAllSMSNumbers")]
        public async Task<IActionResult> DeleteAllSMSNumbers(int stateMasterId, int districtMasterId)
        {
            var deletedCount = await _context.SMSNumbers
                .Where(x => x.StateMasterId == stateMasterId && x.DistrictMasterId == districtMasterId)
                .ExecuteDeleteAsync();

            if (deletedCount == 0)
            {
                return NotFound();
            }

            return NoContent();
        }


    }
}
