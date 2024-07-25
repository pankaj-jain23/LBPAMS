using EAMS.Helper;
using EAMS.ViewModels.PSFormViewModel;
using EAMS_ACore;
using EAMS_ACore.HelperModels;
using EAMS_ACore.IAuthRepository;
using EAMS_ACore.IRepository;
using EAMS_ACore.Models;
using EAMS_ACore.Models.BLOModels;
using EAMS_ACore.Models.ElectionType;
using EAMS_ACore.Models.Polling_Personal_Randomisation_Models;
using EAMS_ACore.Models.Polling_Personal_Randomization_Models;
using EAMS_ACore.Models.PollingStationFormModels;
using EAMS_ACore.Models.PublicModels;
using EAMS_ACore.Models.QueueModel;
using EAMS_ACore.ReportModels;
using EAMS_ACore.SignalRModels;
using EAMS_DAL.DBContext;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;


namespace EAMS_DAL.Repository
{
    public class EamsRepository : IEamsRepository
    {
        private readonly EamsContext _context;
        private readonly IAuthRepository _authRepository;
        private readonly ILogger<EamsRepository> _logger;
        private readonly IConfiguration _configuration;
        public EamsRepository(EamsContext context, IAuthRepository authRepository, ILogger<EamsRepository> logger, IConfiguration configuration)
        {
            _context = context;
            _authRepository = authRepository;
            _logger = logger;
            _configuration = configuration;
        }


        #region UpdateMaster
        public async Task<ServiceResponse> UpdateMasterStatus(UpdateMasterStatus updateMasterStatus)
        {
            switch (updateMasterStatus.Type)
            {
                case "StateMaster":

                    var stateRecord = await _context.StateMaster.FirstOrDefaultAsync(d => d.StateMasterId == Convert.ToInt32(updateMasterStatus.Id));

                    if (stateRecord != null)
                    {
                        if (updateMasterStatus.IsStatus == false)
                        {
                            var districtsActiveOfState = await _context.DistrictMaster
                                .Where(d => d.StateMasterId == stateRecord.StateMasterId && d.DistrictStatus == true)
                                .ToListAsync();

                            if (districtsActiveOfState.Count > 0)
                            {
                                return new ServiceResponse { IsSucceed = false, Message = "Districts are active under this State. Make sure they are Inactive first." };
                            }
                            else
                            {
                                stateRecord.StateStatus = updateMasterStatus.IsStatus;
                                _context.StateMaster.Update(stateRecord);
                                await _context.SaveChangesAsync();

                                return new ServiceResponse { IsSucceed = true, Message = "State Deactivated Successfuly" };
                            }
                        }
                        else if (updateMasterStatus.IsStatus == true)
                        {
                            stateRecord.StateStatus = updateMasterStatus.IsStatus;
                            _context.StateMaster.Update(stateRecord);
                            await _context.SaveChangesAsync();

                            return new ServiceResponse { IsSucceed = true, Message = "State Activated Successfuly" };
                        }
                        else
                        {
                            return new ServiceResponse { IsSucceed = true, Message = "Status cant be empty" };

                        }

                    }
                    else
                    {
                        return new ServiceResponse { IsSucceed = false, Message = "Record Not Found." };

                    }


                case "DistrictMaster":
                    var districtId = Convert.ToInt32(updateMasterStatus.Id);
                    var districtRecord = await _context.DistrictMaster.FirstOrDefaultAsync(d => d.DistrictMasterId == districtId);

                    if (districtRecord != null)
                    {
                        if (updateMasterStatus.IsStatus == false)
                        {
                            var assembliesActiveOfDistrict = await _context.AssemblyMaster
                                .Where(d => d.StateMasterId == districtRecord.StateMasterId && d.DistrictMasterId == districtId && d.AssemblyStatus == true)
                                .ToListAsync();

                            if (assembliesActiveOfDistrict.Count > 0)
                            {
                                return new ServiceResponse { IsSucceed = false, Message = "Assemblies are active under this State District. Make sure they are Inactive first." };
                            }
                            else
                            {
                                districtRecord.DistrictStatus = updateMasterStatus.IsStatus;
                                _context.DistrictMaster.Update(districtRecord);
                                await _context.SaveChangesAsync();

                                return new ServiceResponse { IsSucceed = true, Message = "District Deactivated Successfuly" };
                            }
                        }
                        else if (updateMasterStatus.IsStatus == true)
                        {
                            var stateactive = await _context.StateMaster.AnyAsync(s => s.StateMasterId == districtRecord.StateMasterId && s.StateStatus == true);
                            if (stateactive == false)
                            {
                                return new ServiceResponse { IsSucceed = false, Message = "State must be active in order to set District status to true." };

                            }
                            else
                            {
                                districtRecord.DistrictStatus = updateMasterStatus.IsStatus;
                                _context.DistrictMaster.Update(districtRecord);
                                await _context.SaveChangesAsync();

                                return new ServiceResponse { IsSucceed = true, Message = "District Activated Successfuly" };

                            }
                        }
                        else
                        {
                            return new ServiceResponse { IsSucceed = false, Message = "Record Not Found." };

                        }



                    }
                    else
                    {
                        return new ServiceResponse { IsSucceed = false, Message = "Record Not Found." };
                    }


                case "PCMaster":
                    var pcId = Convert.ToInt32(updateMasterStatus.Id);
                    var pcRecord = await _context.ParliamentConstituencyMaster.FirstOrDefaultAsync(d => d.PCMasterId == pcId);

                    if (pcRecord != null)
                    {
                        if (updateMasterStatus.IsStatus == false)
                        {
                            var assembliesActiveOfPC = await _context.AssemblyMaster
                                .Where(d => d.StateMasterId == pcRecord.StateMasterId && d.PCMasterId == pcId && d.AssemblyStatus == true)
                                .ToListAsync();

                            if (assembliesActiveOfPC.Count > 0)
                            {
                                return new ServiceResponse { IsSucceed = false, Message = "Assemblies are active under this State PC. Make sure they are Inactive first." };
                            }
                            else
                            {
                                pcRecord.PcStatus = updateMasterStatus.IsStatus;
                                _context.ParliamentConstituencyMaster.Update(pcRecord);
                                await _context.SaveChangesAsync();

                                return new ServiceResponse { IsSucceed = true, Message = "PC Updated Successfuly." };
                            }
                        }
                        else if (updateMasterStatus.IsStatus == true)
                        {
                            var stactive = await _context.StateMaster.AnyAsync(s => s.StateMasterId == pcRecord.StateMasterId && s.StateStatus == true);

                            if (stactive == false)
                            {
                                return new ServiceResponse { IsSucceed = false, Message = "State must be active in order to set PC status to true." };

                            }
                            else
                            {
                                pcRecord.PcStatus = updateMasterStatus.IsStatus;
                                _context.ParliamentConstituencyMaster.Update(pcRecord);
                                await _context.SaveChangesAsync();

                                return new ServiceResponse { IsSucceed = true, Message = "PC Updated Successfuly." };

                            }

                        }
                        else
                        {
                            return new ServiceResponse { IsSucceed = false, Message = "Record Not Found." };

                        }


                    }
                    else
                    {
                        return new ServiceResponse { IsSucceed = false, Message = "Record Not Found." };
                    }

                case "AssemblyMaster":
                    var assemblyMaster = await _context.AssemblyMaster.Where(d => d.AssemblyMasterId == Convert.ToInt32(updateMasterStatus.Id)).FirstOrDefaultAsync();

                    if (assemblyMaster != null)
                    {
                        if (updateMasterStatus.IsStatus == false)
                        {
                            var boothsActiveOfAssembly = await _context.BoothMaster
                                .Where(d => d.StateMasterId == assemblyMaster.StateMasterId && d.DistrictMasterId == assemblyMaster.DistrictMasterId && d.AssemblyMasterId == assemblyMaster.AssemblyMasterId && d.BoothStatus == true)
                                .ToListAsync();

                            if (boothsActiveOfAssembly.Count > 0)
                            {
                                return new ServiceResponse
                                {
                                    IsSucceed = false,
                                    Message = "Booths are active under this State Assembly. Make sure they are Inactive first"
                                };
                            }
                            else
                            {
                                assemblyMaster.AssemblyStatus = updateMasterStatus.IsStatus;
                                _context.AssemblyMaster.Update(assemblyMaster);
                                _context.SaveChanges();
                                return new ServiceResponse { IsSucceed = true, Message = "Deactivated Successfuly." };
                            }
                        }

                        // Check if updating to true, ensure the district and PC (coz pc related to assembly) is active
                        else if (updateMasterStatus.IsStatus)

                        {
                            // under discussion that pc district here mandatory to active
                            //if (!await _context.ParliamentConstituencyMaster.AnyAsync(s => s.PCMasterId == assemblyMaster.PCMasterId && s.PcStatus == true) || !await _context.DistrictMaster.AnyAsync(s => s.DistrictMasterId == assemblyMaster.DistrictMasterId && s.DistrictStatus == true))
                            //{
                            //    return new ServiceResponse
                            //    {
                            //        IsSucceed = false,
                            //        Message = "District & PC must be active in order to activate Assembly."
                            //    };
                            //}
                            //else
                            //{
                            assemblyMaster.AssemblyStatus = updateMasterStatus.IsStatus;
                            _context.AssemblyMaster.Update(assemblyMaster);
                            _context.SaveChanges();
                            return new ServiceResponse { IsSucceed = true, Message = "Activated Successfuly." };
                            //}

                        }
                        else
                        {
                            return new ServiceResponse { IsSucceed = false, Message = "Status cant be empty." };
                        }


                    }
                    else
                    {
                        return new ServiceResponse { IsSucceed = false, Message = "Record Not Found." };
                    }

                case "SOMaster":
                    var isSOExist = await _context.SectorOfficerMaster.Where(d => d.SOMasterId == Convert.ToInt32(updateMasterStatus.Id)).FirstOrDefaultAsync();
                    if (isSOExist != null)
                    {

                        if (updateMasterStatus.IsStatus == true)
                        {

                            var assemblyActive = await _context.AssemblyMaster.Where(p => p.AssemblyCode == isSOExist.SoAssemblyCode && p.StateMasterId == isSOExist.StateMasterId).Select(p => p.AssemblyStatus).FirstOrDefaultAsync();
                            if (assemblyActive == true)
                            {
                                isSOExist.SoStatus = updateMasterStatus.IsStatus;
                                _context.SectorOfficerMaster.Update(isSOExist);
                                _context.SaveChanges();
                                return new ServiceResponse { IsSucceed = true, Message = "SO Activated Successfully" };
                            }
                            else
                            {
                                return new ServiceResponse { IsSucceed = false, Message = "Assembly is not Active od this Sector officer" };

                            }
                        }
                        else if (updateMasterStatus.IsStatus == false)
                        {

                            var boothListSo = await _context.BoothMaster.Where(p => p.AssignedTo == isSOExist.SOMasterId.ToString() && p.StateMasterId == isSOExist.StateMasterId).ToListAsync();

                            if (boothListSo.Count == 0)
                            {

                                isSOExist.SoStatus = updateMasterStatus.IsStatus;
                                _context.SectorOfficerMaster.Update(isSOExist);
                                _context.SaveChanges();
                                return new ServiceResponse { IsSucceed = true, Message = "SO Deactivated Successfully" };

                            }
                            else
                            {
                                return new ServiceResponse { IsSucceed = false, Message = "Kindly Release Booths of this Sector Officer first in order to deactivate record." };

                            }


                        }
                        else
                        {
                            return new ServiceResponse { IsSucceed = false, Message = "Status Can't be Null." };

                        }

                    }
                    else
                    {
                        return new ServiceResponse { IsSucceed = false, Message = "Sector Officer Record Not Found." };
                    }

                case "BoothMaster":
                    var isBoothExist = await _context.BoothMaster.Where(d => d.BoothMasterId == Convert.ToInt32(updateMasterStatus.Id)).FirstOrDefaultAsync();
                    if (isBoothExist != null)
                    {
                        var electionInfoRecord = await _context.ElectionInfoMaster
      .Where(d => d.StateMasterId == isBoothExist.StateMasterId && d.DistrictMasterId == isBoothExist.DistrictMasterId && d.AssemblyMasterId == isBoothExist.AssemblyMasterId && d.BoothMasterId == isBoothExist.BoothMasterId)
      .FirstOrDefaultAsync();
                        if (electionInfoRecord == null)
                        {
                            if (isBoothExist.AssignedTo == null || isBoothExist.AssignedTo == "")
                            {
                                // when updating False
                                if (updateMasterStatus.IsStatus == false)
                                {

                                    isBoothExist.BoothStatus = updateMasterStatus.IsStatus;
                                    isBoothExist.LocationMasterId = null;
                                    _context.BoothMaster.Update(isBoothExist);
                                    await _context.SaveChangesAsync();
                                    return new ServiceResponse { IsSucceed = true, Message = "Booth is Unmapped from Location and Booth is Deactivated." };

                                }
                                else if (updateMasterStatus.IsStatus == true)
                                {
                                    var isassmblytrue = await _context.AssemblyMaster.AnyAsync(s => s.AssemblyMasterId == isBoothExist.AssemblyMasterId && s.AssemblyStatus == true);
                                    if (isassmblytrue == false)
                                    {
                                        return new ServiceResponse { IsSucceed = false, Message = "Assembly must be active in order to activate Booth." };

                                    }
                                    else
                                    {
                                        isBoothExist.BoothStatus = updateMasterStatus.IsStatus;
                                        _context.BoothMaster.Update(isBoothExist);
                                        await _context.SaveChangesAsync();
                                        return new ServiceResponse { IsSucceed = true, Message = "Booth Activated succcessfully, Kindly Map booth location." };

                                    }
                                }
                                else
                                {
                                    return new ServiceResponse { IsSucceed = false, Message = "Booth Status can't be null." };

                                }


                            }
                            else
                            {
                                return new ServiceResponse { IsSucceed = false, Message = "Booth is allocated to a Sector Officer, Kindly Release Booth First." };
                            }
                        }
                        else
                        {

                            return new ServiceResponse { IsSucceed = false, Message = "Election Info Record found aganist this Booth, thus can't change status" };

                        }

                    }
                    else
                    {
                        return new ServiceResponse
                        {
                            IsSucceed = false,
                        };
                    }

                case "LocationMaster":
                    var locationMaster = await _context.PollingLocationMaster.Where(d => d.LocationMasterId == Convert.ToInt32(updateMasterStatus.Id)).FirstOrDefaultAsync();

                    if (locationMaster != null)
                    {
                        if (updateMasterStatus.IsStatus == false)
                        {
                            var boothhavingLocation = await _context.BoothMaster.Where(b => b.LocationMasterId == Convert.ToInt32(updateMasterStatus.Id)).ToListAsync();

                            if (boothhavingLocation.Count > 0)
                            {
                                //booths having this location id so, check are they assigned to any so 
                                string boothsAssigned = "";

                                foreach (var boothrec in boothhavingLocation)
                                {
                                    if (boothrec.AssignedTo != null)
                                    {
                                        boothsAssigned += boothrec.AssignedTo + ",";
                                    }

                                }
                                if (boothsAssigned != "")
                                {
                                    return new ServiceResponse { IsSucceed = false, Message = "Booths are mapped to Sector Officer (s), kindly release booths first and then location can be inactive." };

                                }
                                else
                                {

                                    locationMaster.Status = updateMasterStatus.IsStatus;
                                    _context.PollingLocationMaster.Update(locationMaster);

                                    await _context.SaveChangesAsync();
                                    return new ServiceResponse { IsSucceed = true, Message = "Status Updated Succesfully." };
                                }

                            }
                            else
                            { // no booths it can be inactive
                                locationMaster.Status = updateMasterStatus.IsStatus;
                                _context.PollingLocationMaster.Update(locationMaster);

                                await _context.SaveChangesAsync();
                                return new ServiceResponse { IsSucceed = true, Message = "Status Updated Succesfully." };



                            }


                        }

                        // Check if updating to true, ensure the district and PC (coz pc related to assembly) is active
                        else if (updateMasterStatus.IsStatus)

                        {
                            locationMaster.Status = updateMasterStatus.IsStatus;
                            _context.PollingLocationMaster.Update(locationMaster);

                            await _context.SaveChangesAsync();
                            return new ServiceResponse { IsSucceed = true, Message = "Status Updated Succesfully." };

                        }
                        else
                        {
                            return new ServiceResponse { IsSucceed = false, Message = "Status cant be empty." };
                        }


                    }
                    else
                    {
                        return new ServiceResponse { IsSucceed = false, Message = "Record Not Found." };
                    }

                case "HelpDesk":

                    var helpdeskRecord = await _context.HelpDeskDetail.FirstOrDefaultAsync(d => d.HelpDeskMasterId == Convert.ToInt32(updateMasterStatus.Id));

                    if (helpdeskRecord != null)
                    {
                        helpdeskRecord.IsStatus = updateMasterStatus.IsStatus;
                        _context.HelpDeskDetail.Update(helpdeskRecord);
                        await _context.SaveChangesAsync();

                        return new ServiceResponse { IsSucceed = true, Message = "HelpDesk Record Updated Successfuly" };

                    }
                    else
                    {
                        return new ServiceResponse { IsSucceed = false, Message = "Record Not Found." };
                    }

                case "BLO":
                    var isBLOExist = await _context.BLOMaster.Where(d => d.BLOMasterId == Convert.ToInt32(updateMasterStatus.Id)).FirstOrDefaultAsync();
                    if (isBLOExist != null)
                    {

                        if (updateMasterStatus.IsStatus == true)
                        {

                            var assemblyActive = await _context.AssemblyMaster.Where(p => p.AssemblyCode == isBLOExist.AssemblyMasterId && p.StateMasterId == isBLOExist.StateMasterId).Select(p => p.AssemblyStatus).FirstOrDefaultAsync();
                            if (assemblyActive == true)
                            {
                                isBLOExist.BLOStatus = updateMasterStatus.IsStatus;
                                _context.BLOMaster.Update(isBLOExist);
                                _context.SaveChanges();
                                return new ServiceResponse { IsSucceed = true, Message = "BLO Status Updated Successfully" };
                            }
                            else
                            {
                                return new ServiceResponse { IsSucceed = false, Message = "Assembly is not Active of this BLO" };

                            }
                        }
                        else if (updateMasterStatus.IsStatus == false)
                        {

                            var boothListBLO = await _context.BoothMaster.Where(p => p.AssignedToBLO == isBLOExist.BLOMasterId.ToString() && p.StateMasterId == isBLOExist.StateMasterId).ToListAsync();

                            if (boothListBLO.Count == 0)
                            {

                                isBLOExist.BLOStatus = updateMasterStatus.IsStatus;
                                _context.BLOMaster.Update(isBLOExist);
                                _context.SaveChanges();
                                return new ServiceResponse { IsSucceed = true, Message = "BLO Status Updated Successfully" };

                            }
                            else
                            {
                                return new ServiceResponse { IsSucceed = false, Message = "Kindly Release Booths of this BLO first in order to deactivate record." };

                            }


                        }
                        else
                        {
                            return new ServiceResponse { IsSucceed = false, Message = "Status Can't be Null." };

                        }

                    }
                    else
                    {
                        return new ServiceResponse { IsSucceed = false, Message = "Sector Officer Record Not Found." };
                    }


                case "PSZONE":
                    var psZone = await _context.PSZone
                        .Where(d => d.PSZoneMasterId == Convert.ToInt32(updateMasterStatus.Id))
                        .FirstOrDefaultAsync();

                    if (psZone == null)
                    {
                        return new ServiceResponse { IsSucceed = false, Message = "Record Not Found." };
                    }

                    if (updateMasterStatus.IsStatus==false)
                    {
                        var assembly = await _context.AssemblyMaster
                            .Where(d => d.AssemblyMasterId == psZone.AssemblyMasterId)
                            .FirstOrDefaultAsync();

                        if (assembly?.AssemblyStatus == true)
                        {
                            return new ServiceResponse { IsSucceed = false, Message = "Assembly is already active. Please deactivate it first." };
                        }
                    }

                    psZone.PSZoneStatus = updateMasterStatus.IsStatus;
                    _context.PSZone.Update(psZone);
                    await _context.SaveChangesAsync();

                    string message = psZone.PSZoneStatus ? "Zone Activated Successfully" : "Zone Deactivated Successfully";
                    return new ServiceResponse { IsSucceed = psZone.PSZoneStatus, Message = message };
           
                case "SPWards":
                    var spWards = await _context.SarpanchWards
                        .Where(d => d.SarpanchWardsMasterId == Convert.ToInt32(updateMasterStatus.Id))
                        .FirstOrDefaultAsync();

                    if (spWards == null)
                    {
                        return new ServiceResponse { IsSucceed = false, Message = "Record Not Found." };
                    }

                    if (updateMasterStatus.IsStatus==false)
                    {
                        var boothMaster = await _context.BoothMaster
                            .Where(d => d.BoothMasterId == spWards.BoothMasterId)
                            .FirstOrDefaultAsync();

                        if (boothMaster?.BoothStatus == true)
                        {
                            return new ServiceResponse { IsSucceed = false, Message = "Booth is already active. Please deactivate it first." };
                        }
                    }

                    spWards.SarpanchWardsStatus = updateMasterStatus.IsStatus;
                    _context.SarpanchWards.Update(spWards);
                    await _context.SaveChangesAsync();

                    string messageSp = spWards.SarpanchWardsStatus ? "Ward Activated Successfully" : "Ward Deactivated Successfully";
                    return new ServiceResponse { IsSucceed = spWards.SarpanchWardsStatus, Message = messageSp };

                default:
                    return new ServiceResponse
                    {
                        IsSucceed = false,
                    };

            }
        }
        #endregion

        #region DeleteMaster
        public async Task<ServiceResponse> DeleteMasterStatus(DeleteMasterStatus updateMasterStatus)
        {
            switch (updateMasterStatus.Type)
            {
                case "StateMaster":

                    var stateRecord = await _context.StateMaster.FirstOrDefaultAsync(d => d.StateMasterId == Convert.ToInt32(updateMasterStatus.Id));

                    if (stateRecord != null)
                    {

                        var districtsActiveOfState = await _context.DistrictMaster
                            .Where(d => d.StateMasterId == stateRecord.StateMasterId)
                            .ToListAsync();

                        if (districtsActiveOfState.Count > 0)
                        {
                            return new ServiceResponse { IsSucceed = false, Message = "Districts are active under this State. Make sure they are Inactive first." };
                        }
                        else
                        {

                            _context.StateMaster.Remove(stateRecord);
                            await _context.SaveChangesAsync();
                            return new ServiceResponse { IsSucceed = true, Message = "State deleted successfully." };

                        }



                    }
                    else
                    {
                        return new ServiceResponse { IsSucceed = false, Message = "Record Not Found." };
                    }

                case "DistrictMaster":
                    var districtId = Convert.ToInt32(updateMasterStatus.Id);
                    var districtRecord = await _context.DistrictMaster.FirstOrDefaultAsync(d => d.DistrictMasterId == districtId);

                    if (districtRecord != null)
                    {
                        var assembliesRecord = await _context.AssemblyMaster.Where(s => s.DistrictMasterId == districtRecord.DistrictMasterId).ToListAsync();
                        if (assembliesRecord.Count > 0)
                        {
                            return new ServiceResponse { IsSucceed = false, Message = "Assemblies Exist aganist this District, can't delete" };

                        }
                        else
                        {


                            _context.DistrictMaster.Remove(districtRecord);
                            await _context.SaveChangesAsync();
                            return new ServiceResponse { IsSucceed = true, Message = "District deleted successfully." };

                        }
                    }
                    else
                    {
                        return new ServiceResponse { IsSucceed = false, Message = "Record Not Found." };
                    }
                case "AssemblyMaster":
                    var assemblyMaster = await _context.AssemblyMaster.Where(d => d.AssemblyMasterId == Convert.ToInt32(updateMasterStatus.Id)).FirstOrDefaultAsync();

                    if (assemblyMaster != null)
                    {
                        //if (updateMasterStatus.IsStatus == false)
                        //{
                        var boothsActiveOfAssembly = await _context.BoothMaster
                            .Where(d => d.StateMasterId == assemblyMaster.StateMasterId && d.DistrictMasterId == assemblyMaster.DistrictMasterId && d.AssemblyMasterId == assemblyMaster.AssemblyMasterId && d.BoothStatus == true)
                            .ToListAsync();

                        if (boothsActiveOfAssembly.Count > 0)
                        {
                            return new ServiceResponse
                            {
                                IsSucceed = false,
                                Message = "Booths are active under this State Assembly. Make sure they are Inactive first"
                            };
                        }
                        else
                        {
                            _context.AssemblyMaster.Remove(assemblyMaster);
                            await _context.SaveChangesAsync();
                            return new ServiceResponse { IsSucceed = true, Message = "Assembly Deleted Succesfully." };

                        }
                        // }



                    }
                    else
                    {
                        return new ServiceResponse { IsSucceed = false, Message = "Record Not Found." };
                    }

                case "SOMaster":
                    var isSOExist = await _context.SectorOfficerMaster.Where(d => d.SOMasterId == Convert.ToInt32(updateMasterStatus.Id)).FirstOrDefaultAsync();
                    if (isSOExist != null)
                    {
                        var boothsAllocated = await _context.BoothMaster.Where(p => p.AssignedTo == isSOExist.SOMasterId.ToString()).ToListAsync();
                        if (boothsAllocated.Count == 0)
                        {
                            // isSOExist.SoStatus = updateMasterStatus.IsStatus;
                            _context.SectorOfficerMaster.Remove(isSOExist);
                            await _context.SaveChangesAsync();
                            return new ServiceResponse { IsSucceed = true, Message = "So Deleted Successfully" };
                        }
                        else
                        {
                            return new ServiceResponse { IsSucceed = false, Message = "Booths Assignedto this SO, kindly release them !" };

                        }
                    }

                    else
                    {
                        return new ServiceResponse { IsSucceed = false, Message = "Sector Officer Record Not Found." };
                    }

                case "BoothMaster":
                    var isBoothExist = await _context.BoothMaster.Where(d => d.BoothMasterId == Convert.ToInt32(updateMasterStatus.Id)).FirstOrDefaultAsync();
                    if (isBoothExist != null)
                    {
                        var electionInfoRecord = await _context.ElectionInfoMaster
      .Where(d => d.StateMasterId == isBoothExist.StateMasterId && d.DistrictMasterId == isBoothExist.DistrictMasterId && d.AssemblyMasterId == isBoothExist.AssemblyMasterId && d.BoothMasterId == isBoothExist.BoothMasterId)
      .FirstOrDefaultAsync();
                        if (electionInfoRecord == null)
                        {
                            if (isBoothExist.AssignedTo == null || isBoothExist.AssignedTo == "")
                            {
                                if (isBoothExist.AssignedToBLO == null || isBoothExist.AssignedToBLO == "")
                                {
                                    _context.BoothMaster.Remove(isBoothExist);
                                    await _context.SaveChangesAsync();
                                    return new ServiceResponse { IsSucceed = true, Message = "Booth is deleted successfully." };

                                }
                                else
                                {
                                    return new ServiceResponse { IsSucceed = false, Message = "Booth is allocated to a BLO, Kindly Release Booth First." };

                                }

                            }
                            else
                            {
                                return new ServiceResponse { IsSucceed = false, Message = "Booth is allocated to a Sector Officer, Kindly Release Booth First." };
                            }
                        }
                        else
                        {

                            return new ServiceResponse { IsSucceed = false, Message = "Election Info Record found aganist this Booth, thus can't deleted" };

                        }

                    }
                    else
                    {
                        return new ServiceResponse
                        {
                            IsSucceed = false,
                        };
                    }

                case "LocationMaster":
                    var locationMaster = await _context.PollingLocationMaster.Where(d => d.LocationMasterId == Convert.ToInt32(updateMasterStatus.Id)).FirstOrDefaultAsync();

                    if (locationMaster != null)
                    {

                        var boothhavingLocation = await _context.BoothMaster.Where(b => b.LocationMasterId == Convert.ToInt32(updateMasterStatus.Id)).ToListAsync();

                        if (boothhavingLocation.Count > 0)
                        {
                            return new ServiceResponse { IsSucceed = true, Message = "Booths mapped with this Location, Kindly Release." };

                        }
                        else
                        {
                            _context.PollingLocationMaster.Remove(locationMaster);
                            await _context.SaveChangesAsync();
                            return new ServiceResponse { IsSucceed = true, Message = "Location Deleted Succesfully." };



                        }






                    }
                    else
                    {
                        return new ServiceResponse { IsSucceed = false, Message = "Record Not Found." };
                    }

                case "HelpDesk":

                    var helpdeskRecord = await _context.HelpDeskDetail.FirstOrDefaultAsync(d => d.HelpDeskMasterId == Convert.ToInt32(updateMasterStatus.Id));

                    if (helpdeskRecord != null)
                    {
                        // helpdeskRecord.IsStatus = updateMasterStatus.IsStatus;
                        _context.HelpDeskDetail.Remove(helpdeskRecord);
                        await _context.SaveChangesAsync();
                        return new ServiceResponse { IsSucceed = true, Message = "HelpDesk Record is deleted successfully." };



                    }
                    else
                    {
                        return new ServiceResponse { IsSucceed = false, Message = "Record Not Found." };
                    }

                case "BLO":
                    var isBLOExist = await _context.BLOMaster.Where(d => d.BLOMasterId == Convert.ToInt32(updateMasterStatus.Id)).FirstOrDefaultAsync();
                    if (isBLOExist != null)
                    {

                        var assemblyActive = await _context.BoothMaster.Where(p => p.AssignedToBLO == isBLOExist.BLOMasterId.ToString()).ToListAsync();
                        if (assemblyActive.Count == 0)
                        {
                            _context.BLOMaster.Remove(isBLOExist);
                            await _context.SaveChangesAsync();
                            return new ServiceResponse { IsSucceed = true, Message = "BLO Record is deleted successfully." };

                        }
                        else
                        {
                            return new ServiceResponse { IsSucceed = false, Message = "Booths assigned to BLO, kindly release first" };

                        }



                    }
                    else
                    {
                        return new ServiceResponse { IsSucceed = false, Message = "Sector Officer Record Not Found." };
                    }
                default:
                    return new ServiceResponse
                    {
                        IsSucceed = false,
                    };

            }
        }
        #endregion

        #region Common method
        private DateTime? ConvertStringToUtcDateTime(string dateString)
        {
            if (string.IsNullOrEmpty(dateString))
            {
                return null;
            }

            DateTime dateTime = DateTime.ParseExact(dateString, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
            var dateTime1 = ConvertToUtc(dateTime);

            return dateTime1;
        }

        private DateTime? ConvertToUtc(DateTime? dateTime)
        {

            if (dateTime.HasValue)
            {
                // Specify the time zone for India (Indian Standard Time, IST)
                TimeZoneInfo indianTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

                // Convert the local time to UTC
                DateTime utcTime = TimeZoneInfo.ConvertTimeToUtc(dateTime.Value, indianTimeZone);

                // Ensure the kind of the resulting DateTime is DateTimeKind.Utc
                return DateTime.SpecifyKind(utcTime, DateTimeKind.Utc);
            }

            return null;
        }


        private DateTime? BharatDateTime()
        {
            DateTime dateTime = DateTime.Now;
            DateTime utcDateTime = DateTime.SpecifyKind(dateTime.ToUniversalTime(), DateTimeKind.Utc);
            TimeSpan istOffset = TimeSpan.FromHours(5) + TimeSpan.FromMinutes(30);
            TimeZoneInfo istTimeZone = TimeZoneInfo.CreateCustomTimeZone("IST", istOffset, "IST", "IST");
            DateTime hiINDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, istTimeZone);

            return DateTime.SpecifyKind(hiINDateTime, DateTimeKind.Utc);
        }
        #endregion

        //public async Task<Response> ResetAccounts()
        //{
        //    return new Response { Status = RequestStatusEnum.OK, Message = "Reset Updated Successfully" };

        //}

        #region State Master

        public async Task<List<StateMaster>> GetState()
        {
            var stateList = await _context.StateMaster.Select(d => new StateMaster
            {
                StateCode = d.StateCode,
                StateName = d.StateName,
                SecondLanguage = d.SecondLanguage,
                StateMasterId = d.StateMasterId,

                StateStatus = d.StateStatus
            }).OrderBy(d => d.StateMasterId)
                .ToListAsync();

            return stateList;
        }

        //public async Task<Response> ResetAccounts(string stateMasterId)
        //{
        //    var soRecords = _context.SectorOfficerMaster
        //                            .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.OTPAttempts >= 5)
        //                            .ToList();

        //    var bloRecords = _context.BLOMaster
        //                          .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.OTPAttempts >=5 )


        //    if (bloRecords.Any())
        //    {
        //        foreach (var blorecord in bloRecords)
        //        {
        //            blorecord.OTPAttempts = 0;
        //        }

        //        _context.UpdateRange(bloRecords);
        //        await _context.SaveChangesAsync();
        //    }


        //    if (soRecords.Any())
        //    {
        //        foreach (var record in soRecords)
        //        {
        //            record.OTPAttempts = 0;
        //        }

        //        _context.UpdateRange(soRecords);
        //        await _context.SaveChangesAsync();

        //        return new Response { Status = RequestStatusEnum.OK, Message = soRecords.Count + " Accounts Reset Successfully" };
        //    }
        //    else
        //    {
        //        return new Response { Status = RequestStatusEnum.BadRequest, Message = "No Accounts to Reset" };
        //    }
        //}


        public async Task<Response> ResetAccounts(string stateMasterId)
        {
            int stateId = Convert.ToInt32(stateMasterId);

            // Fetch records for both tables
            var soRecords = _context.SectorOfficerMaster
                                    .Where(d => d.StateMasterId == stateId && d.OTPAttempts >= 5)
                                    .ToList();

            var bloRecords = _context.BLOMaster
                                     .Where(d => d.StateMasterId == stateId && d.OTPAttempts >= 5)
                                     .ToList();

            // Reset OTP attempts for BLO records if any exist
            if (bloRecords.Any())
            {
                foreach (var blorecord in bloRecords)
                {
                    blorecord.OTPAttempts = 0;
                }
                _context.UpdateRange(bloRecords);
            }

            // Reset OTP attempts for SO records if any exist
            if (soRecords.Any())
            {
                foreach (var record in soRecords)
                {
                    record.OTPAttempts = 0;
                }
                _context.UpdateRange(soRecords);
            }

            // Save changes if there are any records to update
            if (bloRecords.Any() || soRecords.Any())
            {
                await _context.SaveChangesAsync();

                int resetCount = bloRecords.Count + soRecords.Count;
                return new Response { Status = RequestStatusEnum.OK, Message = resetCount + " Accounts Reset Successfully" };
            }

            return new Response { Status = RequestStatusEnum.BadRequest, Message = "No Accounts to Reset" };
        }

        public async Task<Response> UpdateStateById(StateMaster stateMaster)
        {
            var stateMasterRecord = _context.StateMaster.FirstOrDefault(d => d.StateMasterId == stateMaster.StateMasterId);

            if (stateMasterRecord == null)
            {
                return new Response { Status = RequestStatusEnum.NotFound, Message = "State Not Found" + stateMaster.StateName };
            }

            if (stateMaster.StateStatus == false)
            {
                var districtsActiveOfState = _context.DistrictMaster
                    .Where(d => d.StateMasterId == stateMaster.StateMasterId && d.DistrictStatus == true)
                    .ToList();

                if (districtsActiveOfState.Count > 0)
                {
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Districts are active under this State, Make sure they are Inactive First" };
                }
                else
                {
                    stateMasterRecord.StateName = stateMaster.StateName;
                    stateMasterRecord.StateCode = stateMaster.StateCode;
                    stateMasterRecord.StateStatus = stateMaster.StateStatus;
                    stateMasterRecord.StateUpdatedAt = BharatDateTime();
                    stateMasterRecord.IsGenderCapturedinVoterTurnOut = stateMaster.IsGenderCapturedinVoterTurnOut;

                    _context.StateMaster.Update(stateMasterRecord);
                    _context.SaveChanges();

                    return new Response { Status = RequestStatusEnum.OK, Message = "State Updated Successfully" + stateMaster.StateName };
                }
            }
            else if (stateMaster.StateStatus == true)
            {
                stateMasterRecord.StateName = stateMaster.StateName;
                stateMasterRecord.StateCode = stateMaster.StateCode;
                stateMasterRecord.StateStatus = stateMaster.StateStatus;
                stateMasterRecord.StateUpdatedAt = BharatDateTime();
                stateMasterRecord.IsGenderCapturedinVoterTurnOut = stateMaster.IsGenderCapturedinVoterTurnOut;

                _context.StateMaster.Update(stateMasterRecord);
                _context.SaveChanges();

                return new Response { Status = RequestStatusEnum.OK, Message = "State Updated Successfully" + stateMaster.StateName };
            }
            else
            {
                return new Response { Status = RequestStatusEnum.OK, Message = "Status Cant be EMpty" };

            }

        }

        public async Task<Response> AddState(StateMaster stateMaster)
        {
            try
            {

                var stateExist = _context.StateMaster
    .Where(p => (p.StateCode == stateMaster.StateCode || p.StateName == stateMaster.StateName)).FirstOrDefault();


                if (stateExist == null)
                {
                    stateMaster.StateCreatedAt = BharatDateTime();
                    _context.StateMaster.Add(stateMaster);
                    _context.SaveChanges();

                    return new Response { Status = RequestStatusEnum.OK, Message = $"State Added Successfully {stateMaster.StateName}" };
                }
                else
                {

                    return new Response { Status = RequestStatusEnum.BadRequest, Message = $"State Name Already Exists {stateMaster.StateName}" };
                }
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately, logging or other actions.
                return new Response { Status = RequestStatusEnum.BadRequest, Message = ex.Message };
            }
        }
        public async Task<StateMaster> GetStateById(string stateId)
        {
            var stateRecord = await _context.StateMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateId)).FirstOrDefaultAsync();
            return stateRecord;
        }
        #endregion

        #region District Master
        public async Task<List<CombinedMaster>> GetDistrictById(string stateMasterId)
        {
            try
            {
                var isStateActive = _context.StateMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId)).FirstOrDefault();
                if (isStateActive.StateStatus)
                {
                    var stateData = await _context.DistrictMaster
                        .Include(d => d.StateMaster)
                        .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId)).Select(d => new CombinedMaster
                        {
                            StateId = d.StateMaster.StateMasterId,
                            StateName = d.StateMaster.StateName,
                            DistrictId = d.DistrictMasterId,
                            DistrictName = d.DistrictName,
                            SecondLanguage = d.SecondLanguage,
                            DistrictStatus = d.DistrictStatus,
                            DistrictCode = d.DistrictCode

                        })
                        .OrderByDescending(d => d.DistrictName)
                        .ToListAsync();

                    return stateData;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetDistrictById: {ex.Message}");
                return null;
            }
        }
        public async Task<Response> UpdateDistrictById(DistrictMaster districtMaster)
        {
            if (districtMaster != null)
            {

                var isExist = await _context.DistrictMaster.Where(p => p.DistrictCode == districtMaster.DistrictCode && p.StateMasterId == districtMaster.StateMasterId && p.DistrictMasterId != districtMaster.DistrictMasterId).ToListAsync();

                if (isExist.Count == 0)
                {

                    var isExistName = await _context.DistrictMaster.Where(p => p.DistrictName == districtMaster.DistrictName && p.StateMasterId == districtMaster.StateMasterId && p.DistrictMasterId != districtMaster.DistrictMasterId).ToListAsync();
                    if (isExistName.Count == 0)
                    {
                        var districtMasterRecord = _context.DistrictMaster.Where(d => d.DistrictMasterId == districtMaster.DistrictMasterId).FirstOrDefault();

                        if (districtMaster.DistrictStatus == false)
                        {
                            var assembliesActiveOfDistrict = _context.AssemblyMaster
                                .Where(d => d.StateMasterId == districtMaster.StateMasterId && d.DistrictMasterId == districtMaster.DistrictMasterId && d.AssemblyStatus == true)
                                .ToList();

                            if (assembliesActiveOfDistrict.Count > 0)
                            {
                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Assemblies are active under this State District. Make sure they are Inactive first." };


                            }
                            else
                            {
                                districtMasterRecord.DistrictName = districtMaster.DistrictName;
                                districtMasterRecord.DistrictCode = districtMaster.DistrictCode;
                                districtMasterRecord.DistrictStatus = districtMaster.DistrictStatus;
                                districtMasterRecord.DistrictUpdatedAt = BharatDateTime();
                                _context.DistrictMaster.Update(districtMasterRecord);
                                await _context.SaveChangesAsync();
                                return new Response { Status = RequestStatusEnum.OK, Message = "District Updated Successfully" + districtMaster.DistrictName };
                            }
                        }
                        else if (districtMaster.DistrictStatus == true)
                        {
                            var stateactive = _context.StateMaster.Any(s => s.StateMasterId == districtMaster.StateMasterId && s.StateStatus == true);
                            if (stateactive == false)
                            {
                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "State must be active in order to set District status to true." };


                            }
                            else
                            {
                                districtMasterRecord.DistrictName = districtMaster.DistrictName;
                                districtMasterRecord.DistrictCode = districtMaster.DistrictCode;
                                districtMasterRecord.DistrictStatus = districtMaster.DistrictStatus;
                                districtMasterRecord.DistrictUpdatedAt = BharatDateTime();
                                _context.DistrictMaster.Update(districtMasterRecord);
                                await _context.SaveChangesAsync();
                                return new Response { Status = RequestStatusEnum.OK, Message = "District Updated Successfully" + districtMaster.DistrictName };

                            }
                        }
                        else
                        {
                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Status cant be empty" };


                        }

                    }
                    else
                    {


                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "District with Same Name Already Exists in the State: " + string.Join(", ", isExistName.Select(p => $"{p.DistrictName} ({p.DistrictCode})")) };
                    }
                }
                else
                {
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "District with Same Code Already Exists in the State: " + string.Join(", ", isExist.Select(p => $"{p.DistrictName} ({p.DistrictCode})")) };
                }
            }
            else
            {
                return new Response { Status = RequestStatusEnum.NotFound, Message = "District Not Found" + districtMaster.DistrictName };
            }
        }
        public async Task<Response> AddDistrict(DistrictMaster districtMaster)
        {
            try
            {
                var isExist = _context.DistrictMaster.Where(p => (p.DistrictName == districtMaster.DistrictName && p.StateMasterId == districtMaster.StateMasterId)).FirstOrDefault();
                var isStateActive = _context.StateMaster.Where(p => p.StateMasterId == districtMaster.StateMasterId).FirstOrDefault();
                if (isStateActive.StateStatus)
                    if (isExist == null)
                    {
                        var isExistCode = _context.DistrictMaster.Where(p => p.DistrictCode == districtMaster.DistrictCode && p.StateMasterId == districtMaster.StateMasterId).FirstOrDefault();
                        if (isExistCode == null)
                        {
                            districtMaster.DistrictCreatedAt = BharatDateTime(); ;
                            _context.DistrictMaster.Add(districtMaster);
                            _context.SaveChanges();
                            return new Response { Status = RequestStatusEnum.OK, Message = "District Added Successfully " + districtMaster.DistrictName };

                        }
                        else
                        {
                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "District Code Already Exist " + districtMaster.DistrictCode };

                        }

                    }
                    else
                    {

                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "District Name Already Exist " + districtMaster.DistrictName };
                    }
                else
                {
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = isStateActive.StateName + " State is not Active" };


                }
            }
            catch (Exception ex)
            {// Handle the exception appropriately, logging or other actions.
                return new Response { Status = RequestStatusEnum.BadRequest, Message = ex.Message };
            }
        }
        public async Task<DistrictMaster> GetDistrictRecordById(string districtId)
        {
            var districtRecord = await _context.DistrictMaster.Include(d => d.StateMaster).Where(d => d.DistrictMasterId == Convert.ToInt32(districtId))
               .FirstOrDefaultAsync();
            return districtRecord;
        }
        #endregion

        #region Assembly Master
        public async Task<List<CombinedMaster>> GetAssemblies(string stateId, string districtId, string electionTypeId)
        {
            var isStateActive = _context.StateMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateId)).FirstOrDefault();
            var isDistrictActive = _context.DistrictMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateId) && d.DistrictMasterId == Convert.ToInt32(districtId)).FirstOrDefault();
            if (isStateActive.StateStatus && isDistrictActive.DistrictStatus)
            {
                var innerJoin = from asemb in _context.AssemblyMaster.Where(d => d.DistrictMasterId == Convert.ToInt32(districtId)) // outer sequence
                                join dist in _context.DistrictMaster // inner sequence 
                                on asemb.DistrictMasterId equals dist.DistrictMasterId // key selector
                                join state in _context.StateMaster // additional join for StateMaster
                                on dist.StateMasterId equals state.StateMasterId // key selector for StateMaster
                                join elec in _context.ElectionTypeMaster
                                on asemb.ElectionTypeMasterId equals elec.ElectionTypeMasterId
                                where state.StateMasterId == Convert.ToInt32(stateId) && asemb.ElectionTypeMasterId == Convert.ToInt32(electionTypeId) // condition for StateMasterId equal to 21
                                orderby asemb.AssemblyMasterId
                                select new CombinedMaster
                                { // result selector 
                                    StateName = state.StateName,
                                    DistrictId = dist.DistrictMasterId,
                                    DistrictName = dist.DistrictName,
                                    DistrictCode = dist.DistrictCode,
                                    AssemblyId = asemb.AssemblyMasterId,
                                    AssemblyName = asemb.AssemblyName,
                                    SecondLanguage = asemb.SecondLanguage,
                                    AssemblyCode = asemb.AssemblyCode,
                                    IsStatus = asemb.AssemblyStatus,
                                    ElectionTypeMasterId = asemb.ElectionTypeMasterId,
                                    ElectionTypeName = elec.ElectionType
                                };

                return await innerJoin.ToListAsync();
            }
            else
            {
                return null;
            }
        }

        public async Task<List<CombinedMaster>> GetAssembliesByElectionType(string stateId, string districtId, string electionTypeMasterId)
        {
            var isStateActive = _context.StateMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateId)).FirstOrDefault();
            var isDistrictActive = _context.DistrictMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateId) && d.DistrictMasterId == Convert.ToInt32(districtId)).FirstOrDefault();
            if (isStateActive.StateStatus && isDistrictActive.DistrictStatus)
            {
                var innerJoin = from asemb in _context.AssemblyMaster.Where(d => d.DistrictMasterId == Convert.ToInt32(districtId) && d.ElectionTypeMasterId == Convert.ToInt32(electionTypeMasterId)) // outer sequence
                                join dist in _context.DistrictMaster // inner sequence 
                                on asemb.DistrictMasterId equals dist.DistrictMasterId // key selector
                                join state in _context.StateMaster // additional join for StateMaster
                                on dist.StateMasterId equals state.StateMasterId // key selector for StateMaster
                                where state.StateMasterId == Convert.ToInt32(stateId) // condition for StateMasterId equal to 21
                                orderby asemb.AssemblyMasterId
                                select new CombinedMaster
                                { // result selector 
                                    StateName = state.StateName,
                                    DistrictId = dist.DistrictMasterId,
                                    DistrictName = dist.DistrictName,
                                    DistrictCode = dist.DistrictCode,
                                    AssemblyId = asemb.AssemblyMasterId,
                                    AssemblyName = asemb.AssemblyName,
                                    SecondLanguage = asemb.SecondLanguage,
                                    AssemblyCode = asemb.AssemblyCode,
                                    IsStatus = asemb.AssemblyStatus,
                                    ElectionTypeMasterId = asemb.ElectionTypeMasterId
                                };

                return await innerJoin.ToListAsync();
            }
            else
            {
                return null;
            }
        }

        public async Task<Response> UpdateAssembliesById(AssemblyMaster assemblyMaster)
        {
            var assembliesMasterRecords = _context.AssemblyMaster.Where(d => d.AssemblyMasterId == assemblyMaster.AssemblyMasterId).FirstOrDefault();

            if (assembliesMasterRecords != null)
            {

                var isAssemblyCodeExist = await _context.AssemblyMaster.Where(p => p.AssemblyCode == assemblyMaster.AssemblyCode && p.StateMasterId == assemblyMaster.StateMasterId && p.ElectionTypeMasterId == assemblyMaster.ElectionTypeMasterId && p.AssemblyMasterId != assemblyMaster.AssemblyMasterId).ToListAsync();
                if (isAssemblyCodeExist.Count == 0)
                {

                    //var isExistName = await _context.AssemblyMaster.Where(p => p.AssemblyName == assemblyMaster.AssemblyName && p.StateMasterId == assemblyMaster.StateMasterId && p.ElectionTypeMasterId==assemblyMaster.ElectionTypeMasterId && p.AssemblyMasterId != assemblyMaster.AssemblyMasterId).ToListAsync();
                    //if (isExistName.Count == 0)
                    //{
                    var assembliesMasterRecord = _context.AssemblyMaster.Where(d => d.AssemblyMasterId == assemblyMaster.AssemblyMasterId).FirstOrDefault();

                    if (assembliesMasterRecord != null)
                    {
                        if (assemblyMaster.AssemblyStatus == false)
                        {
                            var boothsActiveOfAssembly = _context.BoothMaster
                                .Where(d => d.StateMasterId == assemblyMaster.StateMasterId && d.DistrictMasterId == assemblyMaster.DistrictMasterId && d.AssemblyMasterId == assemblyMaster.AssemblyMasterId && d.BoothStatus == true)
                                .ToList();

                            if (boothsActiveOfAssembly.Count > 0)
                            {

                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Booths are active under this State Assembly. Make sure they are Inactive first." };
                            }
                            else
                            {
                                assembliesMasterRecord.AssemblyName = assemblyMaster.AssemblyName;
                                assembliesMasterRecord.AssemblyCode = assemblyMaster.AssemblyCode;
                                assembliesMasterRecord.AssemblyType = assemblyMaster.AssemblyType;
                                assembliesMasterRecord.AssemblyStatus = assemblyMaster.AssemblyStatus;
                                assembliesMasterRecord.AssemblyUpdatedAt = BharatDateTime();
                                assembliesMasterRecord.ElectionTypeMasterId = assemblyMaster.ElectionTypeMasterId;
                                assembliesMasterRecord.DistrictMasterId = assemblyMaster.DistrictMasterId;
                                assembliesMasterRecord.StateMasterId = assemblyMaster.StateMasterId;
                                assembliesMasterRecord.PCMasterId = assemblyMaster.PCMasterId;
                                assembliesMasterRecord.TotalBooths = assemblyMaster.TotalBooths;
                                _context.AssemblyMaster.Update(assembliesMasterRecord);
                                await _context.SaveChangesAsync();

                                return new Response { Status = RequestStatusEnum.OK, Message = "Assembly Updated Successfully" + assemblyMaster.AssemblyName };
                            }
                        }
                        else if (assemblyMaster.AssemblyStatus == true)
                        {
                            //if (!_context.ParliamentConstituencyMaster.Any(s => s.PCMasterId == assemblyMaster.PCMasterId && s.PcStatus == true) || !_context.DistrictMaster.Any(s => s.DistrictMasterId == assemblyMaster.DistrictMasterId && s.DistrictStatus == true))
                            //{
                            if (!_context.DistrictMaster.Any(s => s.DistrictMasterId == assemblyMaster.DistrictMasterId && s.DistrictStatus == true))
                            {
                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "District must be active in order to activate Assembly." };

                            }
                            else
                            {
                                assembliesMasterRecord.AssemblyName = assemblyMaster.AssemblyName;
                                assembliesMasterRecord.AssemblyCode = assemblyMaster.AssemblyCode;
                                assembliesMasterRecord.AssemblyType = assemblyMaster.AssemblyType;
                                assembliesMasterRecord.AssemblyStatus = assemblyMaster.AssemblyStatus;
                                assembliesMasterRecord.AssemblyUpdatedAt = BharatDateTime();
                                assembliesMasterRecord.DistrictMasterId = assemblyMaster.DistrictMasterId;
                                assembliesMasterRecord.StateMasterId = assemblyMaster.StateMasterId;
                                assembliesMasterRecord.PCMasterId = assemblyMaster.PCMasterId;
                                assembliesMasterRecord.TotalBooths = assemblyMaster.TotalBooths;
                                assembliesMasterRecord.ElectionTypeMasterId = assemblyMaster.ElectionTypeMasterId;
                                _context.AssemblyMaster.Update(assembliesMasterRecord);
                                await _context.SaveChangesAsync();

                                return new Response { Status = RequestStatusEnum.OK, Message = "Assembly Updated Successfully" + assemblyMaster.AssemblyName };
                            }

                        }
                        else
                        {
                            return new Response { Status = RequestStatusEnum.OK, Message = "Status cant be empty" };

                        }



                    }
                    else
                    {
                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Pc record Not Found." };

                    }

                    //}
                    //else
                    //{
                    //    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Assembly with Same Name Already Exists in the State: " + string.Join(", ", isExistName.Select(p => $"{p.AssemblyName} ({p.AssemblyCode})")) };
                    //}


                }
                else
                {
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Assembly with Same Code Already Exists in the selected Election Type: " + string.Join(", ", isAssemblyCodeExist.Select(p => $"{p.AssemblyName} ({p.AssemblyCode})")) };
                }


            }
            else
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Assembly Not Found" + assemblyMaster.AssemblyName };
            }
        }
        public async Task<Response> UpdatePC(ParliamentConstituencyMaster pcMaster)
        {
            if (pcMaster != null)
            {
                var isExist = await _context.ParliamentConstituencyMaster.Where(p => p.PcCodeNo == pcMaster.PcCodeNo && p.StateMasterId == pcMaster.StateMasterId && p.PCMasterId != pcMaster.PCMasterId).ToListAsync();

                if (isExist.Count == 0)
                {

                    var isExistName = await _context.ParliamentConstituencyMaster.Where(p => p.PcName == pcMaster.PcName && p.StateMasterId == pcMaster.StateMasterId && p.PCMasterId != pcMaster.PCMasterId).ToListAsync();
                    if (isExistName.Count == 0)
                    {
                        var pcMasterRecord = _context.ParliamentConstituencyMaster.Where(d => d.PCMasterId == pcMaster.PCMasterId).FirstOrDefault();

                        if (pcMasterRecord != null)
                        {
                            if (pcMaster.PcStatus == false)
                            {
                                var assembliesActiveOfPC = _context.AssemblyMaster
                                    .Where(d => d.StateMasterId == pcMaster.StateMasterId && d.PCMasterId == pcMaster.PCMasterId && d.AssemblyStatus == true)
                                    .ToList();

                                if (assembliesActiveOfPC.Count > 0)
                                {

                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Assemblies are active under this State PC. Make sure they are Inactive first." };
                                }
                                else
                                {
                                    pcMasterRecord.PcName = pcMaster.PcName;
                                    pcMasterRecord.PcCodeNo = pcMaster.PcCodeNo;
                                    pcMasterRecord.PcType = pcMaster.PcType;
                                    pcMasterRecord.PcStatus = pcMaster.PcStatus;
                                    pcMasterRecord.PcUpdatedAt = BharatDateTime();
                                    _context.ParliamentConstituencyMaster.Update(pcMasterRecord);
                                    await _context.SaveChangesAsync();

                                    return new Response { Status = RequestStatusEnum.OK, Message = "PC Updated Successfully" + pcMaster.PcName };
                                }
                            }
                            else if (pcMaster.PcStatus == true)
                            {
                                var s = _context.StateMaster.Any(s => s.StateMasterId == pcMaster.StateMasterId && s.StateStatus == true);
                                if (s == false)
                                {
                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "State must be active in order to activate PC." };

                                }
                                else
                                {
                                    pcMasterRecord.PcName = pcMaster.PcName;
                                    pcMasterRecord.PcCodeNo = pcMaster.PcCodeNo;
                                    pcMasterRecord.PcType = pcMaster.PcType;
                                    pcMasterRecord.PcStatus = pcMaster.PcStatus;
                                    pcMasterRecord.PcUpdatedAt = BharatDateTime();
                                    _context.ParliamentConstituencyMaster.Update(pcMasterRecord);
                                    await _context.SaveChangesAsync();

                                    return new Response { Status = RequestStatusEnum.OK, Message = "PC Updated Successfully" + pcMaster.PcName };

                                }
                            }
                            else
                            {
                                return new Response { Status = RequestStatusEnum.OK, Message = "Status cant be empty" };

                            }



                        }
                        else
                        {
                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Pc record Not Found." };

                        }

                    }
                    else
                    {


                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "PC with Same Name Already Exists in the State: " + string.Join(", ", isExistName.Select(p => $"{p.PcName} ({p.PcCodeNo})")) };
                    }
                }
                else
                {
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "PC with Same Code Already Exists in the State: " + string.Join(", ", isExist.Select(p => $"{p.PcName} ({p.PcCodeNo})")) };
                }
            }
            else
            {
                return new Response { Status = RequestStatusEnum.NotFound, Message = "PC Not Found" + pcMaster.PcName };
            }
        }
        public async Task<Response> AddAssemblies(AssemblyMaster assemblyMaster)
        {
            try
            {
                var assemblieExist = await _context.AssemblyMaster.Where(p => p.AssemblyCode == assemblyMaster.AssemblyCode && p.StateMasterId == assemblyMaster.StateMasterId && p.DistrictMasterId == assemblyMaster.DistrictMasterId && p.ElectionTypeMasterId == assemblyMaster.ElectionTypeMasterId).FirstOrDefaultAsync();

                if (assemblieExist == null)
                {

                    assemblyMaster.AssemblyCreatedAt = BharatDateTime();
                    _context.AssemblyMaster.Add(assemblyMaster);
                    _context.SaveChanges();

                    return new Response { Status = RequestStatusEnum.OK, Message = assemblyMaster.AssemblyName + "Added Successfully" };



                }
                else
                {
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = assemblyMaster.AssemblyName + "Same Assembly Code Already Exists in the selected Election Type" };

                }

            }

            catch (Exception ex)
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = ex.Message };
            }
        }
        public async Task<Response> AddPC(ParliamentConstituencyMaster pcMaster)
        {
            try
            {

                var pcExist = _context.ParliamentConstituencyMaster.Where(p => p.PcName == pcMaster.PcName || p.PcCodeNo == pcMaster.PcCodeNo && p.StateMasterId == pcMaster.StateMasterId && p.ElectionTypeId == pcMaster.ElectionTypeId).FirstOrDefault();

                if (pcExist == null)
                {

                    pcMaster.PcCreatedAt = BharatDateTime();
                    _context.ParliamentConstituencyMaster.Add(pcMaster);
                    _context.SaveChanges();

                    return new Response { Status = RequestStatusEnum.OK, Message = pcMaster.PcName + "Added Successfully" };

                }
                else
                {
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = pcMaster.PcCodeNo + "Same PC Code Already Exists" };

                }

            }

            catch (Exception ex)
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = ex.Message };
            }
        }
        public async Task<AssemblyMaster> GetAssemblyById(string assemblyMasterId)
        {
            var assemblyRecord = await _context.AssemblyMaster.Include(d => d.StateMaster).Include(d => d.DistrictMaster).Include(d => d.ParliamentConstituencyMaster).Include(d => d.ElectionTypeMaster).Where(d => d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId)).FirstOrDefaultAsync();
            return assemblyRecord;
        }

        public async Task<ElectionInfoMaster> GetElectionInfoRecord(int boothMasterId)
        {
            var electionRecord = await _context.ElectionInfoMaster.Where(d => d.BoothMasterId == boothMasterId).FirstOrDefaultAsync();
            return electionRecord;
        }
        //public async Task<booth> GetAssemblyByBoothId(string boothMasterId)
        //{
        //    var assemblyRecord = await _context.BoothMaster.Include(d => Convert.ToInt32(d.BoothMasterId)).FirstOrDefaultAsync();
        //    return assemblyRecord;
        //}
        public async Task<AssemblyMaster> GetAssemblyByCode(string assemblyCode, string stateMasterId)
        {
            var assemblyRecord = await _context.AssemblyMaster
        .Include(d => d.StateMaster)
        .Include(d => d.DistrictMaster)
        .Include(d => d.ParliamentConstituencyMaster)
        .Where(d => d.AssemblyCode == Convert.ToInt32(assemblyCode) && d.StateMasterId == Convert.ToInt32(stateMasterId))
        .Select(d => new AssemblyMaster
        {
            StateMasterId = d.StateMasterId,
            DistrictMasterId = d.DistrictMasterId,
            PCMasterId = d.PCMasterId,
            AssemblyMasterId = d.AssemblyMasterId
        })
        .FirstOrDefaultAsync();

            return assemblyRecord;
        }
        public async Task<AssemblyMaster> GetAssemblyByCodeandState(string assemblyCode, string stateMasterid)
        {
            var assemblyRecord = await _context.AssemblyMaster.Include(d => d.StateMaster).Include(d => d.DistrictMaster).Include(d => d.ParliamentConstituencyMaster).Where(d => d.AssemblyCode == Convert.ToInt32(assemblyCode) && d.StateMasterId == Convert.ToInt32(stateMasterid)).FirstOrDefaultAsync();
            return assemblyRecord;
        }

        public async Task<List<AssemblyMaster>> GetAssemblyByPCId(string stateMasterid, string PcMasterId)
        {

            var asemData = await _context.AssemblyMaster
    .Where(d => d.PCMasterId == Convert.ToInt32(PcMasterId) && d.StateMasterId == Convert.ToInt32(stateMasterid))
    .OrderBy(d => d.PCMasterId)
    .Select(d => new AssemblyMaster
    {
        PCMasterId = d.PCMasterId,
        StateMasterId = d.StateMasterId,
        AssemblyCode = d.AssemblyCode,
        AssemblyName = d.AssemblyName,
        AssemblyType = d.AssemblyType,
        AssemblyStatus = d.AssemblyStatus,
        AssemblyMasterId = d.AssemblyMasterId,
        AssemblyCreatedAt = d.AssemblyCreatedAt
    })
    .ToListAsync();

            return asemData;
        }
        public async Task<List<AssemblyMaster>> GetAssemblyByDistrictId(string stateMasterId, string districtMasterId)
        {

            var asemData = await _context.AssemblyMaster
    .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId))
    .OrderBy(d => d.PCMasterId)
    .Select(d => new AssemblyMaster
    {
        PCMasterId = d.PCMasterId,
        StateMasterId = d.StateMasterId,
        AssemblyCode = d.AssemblyCode,
        AssemblyName = d.AssemblyName,
        AssemblyType = d.AssemblyType,
        AssemblyStatus = d.AssemblyStatus,
        AssemblyCreatedAt = d.AssemblyCreatedAt,
        ElectionTypeMasterId = d.ElectionTypeMasterId
    })
    .ToListAsync();

            return asemData;
        }
        #endregion

        #region SO Master
        public async Task<List<CombinedMaster>> GetSectorOfficersListById(string stateMasterId, string districtMasterId, string assemblyMasterId)
        {
            IQueryable<CombinedMaster> solist = Enumerable.Empty<CombinedMaster>().AsQueryable();
            var isStateActive = _context.StateMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId)).FirstOrDefault();
            var isDistrictActive = _context.DistrictMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId)).FirstOrDefault();
            var isAssemblyActive = _context.AssemblyMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId)).FirstOrDefault();
            if (isStateActive.StateStatus && isDistrictActive.DistrictStatus && isAssemblyActive.AssemblyStatus)
            {
                if (districtMasterId == "0")
                {
                    solist = from so in _context.SectorOfficerMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId)) // outer sequence
                             join asem in _context.AssemblyMaster
                             on so.SoAssemblyCode equals asem.AssemblyCode
                             join pc in _context.ParliamentConstituencyMaster
                             on asem.PCMasterId equals pc.PCMasterId
                             where asem.StateMasterId == Convert.ToInt32(stateMasterId) && asem.AssemblyMasterId == Convert.ToInt32(assemblyMasterId) // key selector
                             join state in _context.StateMaster
                              on pc.StateMasterId equals state.StateMasterId

                             select new CombinedMaster
                             { // result selector 
                                 StateName = state.StateName,
                                 //DistrictId = dist.DistrictMasterId,
                                 //DistrictName = dist.DistrictName,
                                 //DistrictCode = dist.DistrictCode,
                                 PCMasterId = pc.PCMasterId,
                                 PCName = pc.PcName,
                                 AssemblyId = asem.AssemblyMasterId,
                                 AssemblyName = asem.AssemblyName,
                                 AssemblyCode = asem.AssemblyCode,
                                 soName = so.SoName,
                                 soMobile = so.SoMobile,
                                 soDesignation = so.SoDesignation,
                                 soMasterId = so.SOMasterId,
                                 RecentOTP = so.OTP,
                                 OTPExpireTime = so.OTPExpireTime,
                                 OTPAttempts = so.OTPAttempts,
                                 IsStatus = so.SoStatus


                             };
                }
                else
                {
                    solist = from so in _context.SectorOfficerMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId)) // outer sequence
                             join asem in _context.AssemblyMaster
                             on so.SoAssemblyCode equals asem.AssemblyCode
                             join dist in _context.DistrictMaster
                             on asem.DistrictMasterId equals dist.DistrictMasterId
                             where asem.DistrictMasterId == Convert.ToInt32(districtMasterId) && asem.AssemblyMasterId == Convert.ToInt32(assemblyMasterId) // key selector
                             join state in _context.StateMaster
                              on dist.StateMasterId equals state.StateMasterId

                             select new CombinedMaster
                             { // result selector 
                                 StateName = state.StateName,
                                 DistrictId = dist.DistrictMasterId,
                                 DistrictName = dist.DistrictName,
                                 DistrictCode = dist.DistrictCode,
                                 AssemblyId = asem.AssemblyMasterId,
                                 AssemblyName = asem.AssemblyName,
                                 AssemblyCode = asem.AssemblyCode,
                                 soName = so.SoName,
                                 soMobile = so.SoMobile,
                                 soDesignation = so.SoDesignation,
                                 soMasterId = so.SOMasterId,
                                 RecentOTP = so.OTP,
                                 OTPExpireTime = so.OTPExpireTime,
                                 OTPAttempts = so.OTPAttempts,
                                 IsStatus = so.SoStatus


                             };

                }


                return await solist.ToListAsync();
            }
            else
            {
                return null;
            }
        }

        public async Task<SectorOfficerProfile> GetSectorOfficerProfile2(string soId)
        {
            var sectorOfficerProfile = await (from so in _context.SectorOfficerMaster
                                              where so.SOMasterId == Convert.ToInt32(soId)
                                              join state in _context.StateMaster on so.StateMasterId equals state.StateMasterId
                                              join assembly in _context.AssemblyMaster on so.SoAssemblyCode equals assembly.AssemblyCode
                                              join district in _context.DistrictMaster on assembly.DistrictMasterId equals district.DistrictMasterId


                                              where so.SoStatus && so.StateMasterId == state.StateMasterId
                                              select new SectorOfficerProfile
                                              {
                                                  StateName = state.StateName,
                                                  DistrictName = district.DistrictName,
                                                  AssemblyName = assembly.AssemblyName,
                                                  AssemblyCode = assembly.AssemblyCode.ToString(),
                                                  SoName = so.SoName,
                                                  ElectionType = so.ElectionTypeMasterId == 1 ? "LS" : (so.ElectionTypeMasterId == 2 ? "VS" : null),
                                                  BoothNo = _context.BoothMaster.Where(p => p.AssignedTo == soId)
                                                                                   .OrderBy(p => p.BoothCode_No)
                                                                                   .Select(p => p.BoothCode_No.ToString())
                                                                                   .ToList()
                                              }).FirstOrDefaultAsync();

            return sectorOfficerProfile;
        }


        public async Task<SectorOfficerProfile> GetSectorOfficerProfile(string soId)
        {
            var soRecord = _context.SectorOfficerMaster.Where(d => d.SOMasterId == Convert.ToInt32(soId) && d.SoStatus == true).FirstOrDefault();
            var soAssembly = _context.AssemblyMaster.Where(d => d.AssemblyCode == soRecord.SoAssemblyCode && d.StateMasterId == soRecord.StateMasterId).FirstOrDefault();
            var soDistrict = _context.DistrictMaster.Where(d => d.DistrictMasterId == soAssembly.DistrictMasterId && d.StateMasterId == soAssembly.StateMasterId).FirstOrDefault();
            var soState = _context.StateMaster.Where(d => d.StateMasterId == soDistrict.StateMasterId).FirstOrDefault();
            //var soState = _context.ElectionTypeMaster.Where(d => d.ElectionTypeMasterId == soDistrict.StateMasterId).FirstOrDefault();
            SectorOfficerProfile sectorOfficerProfile = new SectorOfficerProfile()
            {
                StateName = soState.StateName,
                DistrictName = soDistrict.DistrictName,
                AssemblyName = soAssembly.AssemblyName,
                AssemblyCode = soAssembly.AssemblyCode.ToString(),
                SoName = soRecord.SoName,
                OfficerRole = "SO",
                //ElectionTypeMasterId=
                //ElectionType = soRecord.ElectionTypeMasterId == 1 ? "LS" : (soRecord.ElectionTypeMasterId == 2 ? "VS" : null),
                //BoothNo = _context.BoothMaster.Where(p => p.AssignedTo == soId).Select(p => p.BoothCode_No.ToString()).ToList().OrderBy(p=>p.)
                BoothNo = _context.BoothMaster.Where(p => p.AssignedTo == soId).OrderBy(p => Convert.ToInt32(p.BoothCode_No)).Select(p => p.BoothCode_No.ToString()).ToList()


            };
            return sectorOfficerProfile;
        }



        public async Task<Response> AddSectorOfficer(SectorOfficerMaster sectorOfficerMaster)
        {
            var isExist = _context.SectorOfficerMaster.Where(d => d.SoMobile == sectorOfficerMaster.SoMobile && d.ElectionTypeMasterId == sectorOfficerMaster.ElectionTypeMasterId && d.StateMasterId == sectorOfficerMaster.StateMasterId).FirstOrDefault();
            var isExistCount = _context.SectorOfficerMaster.Where(d => d.SoMobile == sectorOfficerMaster.SoMobile && d.ElectionTypeMasterId == sectorOfficerMaster.ElectionTypeMasterId).Count();

            if (isExistCount > 2)
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = "SO User " + sectorOfficerMaster.SoName + " " + "Already Exists more than 2 in this this election Type" };
            }


            //     if (isExist == null || isExist.ElectionTypeMasterId != sectorOfficerMaster.ElectionTypeMasterId)
            if (isExist == null)
            {
                var isAssemblyRecord = _context.AssemblyMaster.Where(d => d.AssemblyCode == sectorOfficerMaster.SoAssemblyCode && d.StateMasterId == sectorOfficerMaster.StateMasterId && d.ElectionTypeMasterId == sectorOfficerMaster.ElectionTypeMasterId).FirstOrDefault();
                if (isAssemblyRecord != null && isAssemblyRecord.AssemblyStatus == true)
                {
                    //check number already exists
                    if (isAssemblyRecord.StateMasterId == sectorOfficerMaster.StateMasterId && isAssemblyRecord.AssemblyCode == sectorOfficerMaster.SoAssemblyCode)
                    {

                        sectorOfficerMaster.SoCreatedAt = BharatDateTime();
                        _context.SectorOfficerMaster.Add(sectorOfficerMaster);
                        _context.SaveChanges();
                        return new Response { Status = RequestStatusEnum.OK, Message = "SO User " + sectorOfficerMaster.SoName + " " + "Added Successfully" };
                    }
                    else
                    {
                        if (isExist.ElectionTypeMasterId != sectorOfficerMaster.ElectionTypeMasterId)
                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "For this Election type SO is Already Added Choose Another Election Type" };
                        else
                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Assembly Code is not Valid" };
                    }

                }
                else
                {
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Assembly is not active, kindly make active in order to add Sector Officer." };

                }



            }
            else
            {
                var existSo = _context.SectorOfficerMaster.Where(so => so.SoMobile == sectorOfficerMaster.SoMobile).FirstOrDefault();
                var assembly = _context.AssemblyMaster.Where(d => d.AssemblyCode == existSo.SoAssemblyCode && d.StateMasterId == existSo.StateMasterId).FirstOrDefault();
                if (assembly != null)
                {
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "SO User WIth given Mobile Number already exist" };

                }
                else
                {
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "SO User WIth given Mobile Number : " + sectorOfficerMaster.SoMobile + " " + "Already Exists with following Details " + " " + existSo.SoName + " , " + " AssemblyCode - " + existSo.SoAssemblyCode + " " + "Assembly Name - " + " " + assembly.AssemblyName };

                }


            }
        }

        public async Task<Response> UpdateSectorOfficer(SectorOfficerMaster updatedSectorOfficer)
        {
            var existingSectorOfficer = await _context.SectorOfficerMaster
                                                       .FirstOrDefaultAsync(so => so.SOMasterId == updatedSectorOfficer.SOMasterId);

            if (existingSectorOfficer == null)
            {

                return new Response { Status = RequestStatusEnum.BadRequest, Message = "SO User" + updatedSectorOfficer.SoName + " " + "Not found" };
            }


            if (updatedSectorOfficer.SoStatus == true)
            {
                //check if true (then state,district,assembly acive
                var assemblyActive = _context.AssemblyMaster.Where(p => p.AssemblyCode == existingSectorOfficer.SoAssemblyCode && p.StateMasterId == existingSectorOfficer.StateMasterId && p.ElectionTypeMasterId == existingSectorOfficer.ElectionTypeMasterId).Select(p => p.AssemblyStatus).FirstOrDefault();
                if (assemblyActive == true)
                {
                    // Check if the mobile number is unique among other sector officers (excluding the current one being updated)
                    var isMobileUnique = await _context.SectorOfficerMaster.AnyAsync(so => so.SoMobile == updatedSectorOfficer.SoMobile);
                    if (string.Equals(updatedSectorOfficer.SoMobile, existingSectorOfficer.SoMobile, StringComparison.OrdinalIgnoreCase))
                    {
                        var isAssemblyRecord = _context.AssemblyMaster.Where(d => d.AssemblyCode == existingSectorOfficer.SoAssemblyCode && d.StateMasterId == existingSectorOfficer.StateMasterId && d.ElectionTypeMasterId == existingSectorOfficer.ElectionTypeMasterId).FirstOrDefault();
                        if (isAssemblyRecord != null && isAssemblyRecord.AssemblyStatus == true)
                        {//check election type should be same of assembly
                            if (isAssemblyRecord.ElectionTypeMasterId == updatedSectorOfficer.ElectionTypeMasterId)
                            {

                                if (isAssemblyRecord.StateMasterId == existingSectorOfficer.StateMasterId && isAssemblyRecord.AssemblyCode == existingSectorOfficer.SoAssemblyCode && isAssemblyRecord.ElectionTypeMasterId == existingSectorOfficer.ElectionTypeMasterId)
                                {
                                    existingSectorOfficer.SoName = updatedSectorOfficer.SoName;
                                    existingSectorOfficer.SoMobile = updatedSectorOfficer.SoMobile;
                                    existingSectorOfficer.SoOfficeName = updatedSectorOfficer.SoOfficeName;
                                    existingSectorOfficer.SoAssemblyCode = updatedSectorOfficer.SoAssemblyCode;
                                    existingSectorOfficer.SoDesignation = updatedSectorOfficer.SoDesignation;
                                    existingSectorOfficer.SoStatus = updatedSectorOfficer.SoStatus;
                                    existingSectorOfficer.ElectionTypeMasterId = updatedSectorOfficer.ElectionTypeMasterId;
                                    existingSectorOfficer.SOUpdatedAt = BharatDateTime();

                                    _context.SectorOfficerMaster.Update(existingSectorOfficer);
                                    await _context.SaveChangesAsync();


                                    return new Response { Status = RequestStatusEnum.OK, Message = "SO User " + existingSectorOfficer.SoName + " " + "updated successfully" };

                                }
                                else
                                {
                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Please check State, AssemblyCode & Election Type are not same" };

                                }
                            }
                            else
                            {
                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Election Type invalid for the selected Assembly." };

                            }
                        }
                        else
                        {
                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Assembly is not active !" };

                        }


                    }
                    else
                    {
                        var isAssemblyRecords = _context.AssemblyMaster.Where(d => d.AssemblyCode == existingSectorOfficer.SoAssemblyCode && d.StateMasterId == existingSectorOfficer.StateMasterId && d.ElectionTypeMasterId == existingSectorOfficer.ElectionTypeMasterId).FirstOrDefault();

                        if (isAssemblyRecords.ElectionTypeMasterId == updatedSectorOfficer.ElectionTypeMasterId)
                        {

                            if (isMobileUnique == false)
                            {
                                existingSectorOfficer.SoName = updatedSectorOfficer.SoName;
                                existingSectorOfficer.SoMobile = updatedSectorOfficer.SoMobile;
                                existingSectorOfficer.SoOfficeName = updatedSectorOfficer.SoOfficeName;
                                existingSectorOfficer.SoAssemblyCode = updatedSectorOfficer.SoAssemblyCode;
                                existingSectorOfficer.SoDesignation = updatedSectorOfficer.SoDesignation;
                                existingSectorOfficer.SoStatus = updatedSectorOfficer.SoStatus;
                                existingSectorOfficer.ElectionTypeMasterId = updatedSectorOfficer.ElectionTypeMasterId;
                                existingSectorOfficer.SOUpdatedAt = BharatDateTime();

                                _context.SectorOfficerMaster.Update(existingSectorOfficer);
                                await _context.SaveChangesAsync();
                                return new Response { Status = RequestStatusEnum.OK, Message = "SO User " + existingSectorOfficer.SoName + " " + "updated successfully" };

                            }
                            else
                            {
                                var existSo = _context.SectorOfficerMaster.Where(so => so.SoMobile == updatedSectorOfficer.SoMobile).FirstOrDefault();
                                var assembly = _context.AssemblyMaster.Where(d => d.AssemblyCode == existSo.SoAssemblyCode).FirstOrDefault();

                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "SO User WIth given Mobile Number : " + updatedSectorOfficer.SoMobile + " " + "Already Exists with following Details " + " " + existSo.SoName + " , " + " AssemblyCode - " + existSo.SoAssemblyCode + " " + "Assembly Name - " + " " + assembly.AssemblyName };

                            }
                        }
                        else
                        {
                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Election Type is invalid for the selected Assembly." };

                        }
                    }

                }
                else
                {
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Assembly is not Active" };

                }
            }
            else if (updatedSectorOfficer.SoStatus == false)
            {// if assigned any booths not allowed to deactivate

                var boothListSo = _context.BoothMaster.Where(p => p.AssignedTo == existingSectorOfficer.SOMasterId.ToString() && p.StateMasterId == existingSectorOfficer.StateMasterId).ToList();

                if (boothListSo.Count == 0)
                {
                    //release booths first


                    // Check if the mobile number is unique among other sector officers (excluding the current one being updated)
                    var isMobileUnique = await _context.SectorOfficerMaster.AnyAsync(so => so.SoMobile == updatedSectorOfficer.SoMobile);
                    if (string.Equals(updatedSectorOfficer.SoMobile, existingSectorOfficer.SoMobile, StringComparison.OrdinalIgnoreCase))
                    {
                        var isAssemblyRecords = _context.AssemblyMaster.Where(d => d.AssemblyCode == updatedSectorOfficer.SoAssemblyCode && d.StateMasterId == updatedSectorOfficer.StateMasterId && d.ElectionTypeMasterId == updatedSectorOfficer.ElectionTypeMasterId).FirstOrDefault();

                        if (isAssemblyRecords.ElectionTypeMasterId == updatedSectorOfficer.ElectionTypeMasterId)
                        {

                            existingSectorOfficer.SoName = updatedSectorOfficer.SoName;
                            existingSectorOfficer.SoMobile = updatedSectorOfficer.SoMobile;
                            existingSectorOfficer.SoOfficeName = updatedSectorOfficer.SoOfficeName;
                            existingSectorOfficer.SoAssemblyCode = updatedSectorOfficer.SoAssemblyCode;
                            existingSectorOfficer.SoDesignation = updatedSectorOfficer.SoDesignation;
                            existingSectorOfficer.SoStatus = updatedSectorOfficer.SoStatus;
                            existingSectorOfficer.ElectionTypeMasterId = updatedSectorOfficer.ElectionTypeMasterId;
                            existingSectorOfficer.SOUpdatedAt = BharatDateTime();

                            _context.SectorOfficerMaster.Update(existingSectorOfficer);
                            await _context.SaveChangesAsync();


                            return new Response { Status = RequestStatusEnum.OK, Message = "SO User " + existingSectorOfficer.SoName + " " + "updated successfully" };

                        }
                        else
                        {
                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Election Type is invalid for the selected Assembly." };

                        }
                    }
                    else
                    {
                        if (isMobileUnique == false)
                        {

                            existingSectorOfficer.SoName = updatedSectorOfficer.SoName;
                            existingSectorOfficer.SoMobile = updatedSectorOfficer.SoMobile;
                            existingSectorOfficer.SoOfficeName = updatedSectorOfficer.SoOfficeName;
                            existingSectorOfficer.SoAssemblyCode = updatedSectorOfficer.SoAssemblyCode;
                            existingSectorOfficer.SoDesignation = updatedSectorOfficer.SoDesignation;
                            existingSectorOfficer.SoStatus = updatedSectorOfficer.SoStatus;
                            existingSectorOfficer.SOUpdatedAt = BharatDateTime();
                            existingSectorOfficer.ElectionTypeMasterId = updatedSectorOfficer.ElectionTypeMasterId;
                            _context.SectorOfficerMaster.Update(existingSectorOfficer);
                            await _context.SaveChangesAsync();
                            return new Response { Status = RequestStatusEnum.OK, Message = "SO User " + existingSectorOfficer.SoName + " " + "updated successfully" };

                        }
                        else
                        {
                            var existSo = _context.SectorOfficerMaster.Where(so => so.SoMobile == updatedSectorOfficer.SoMobile).FirstOrDefault();
                            var assembly = _context.AssemblyMaster.Where(d => d.AssemblyCode == existSo.SoAssemblyCode).FirstOrDefault();

                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "SO User WIth given Mobile Number : " + updatedSectorOfficer.SoMobile + " " + "Already Exists with following Details" + existSo.SoName + " AssemblyCode" + existSo.SoAssemblyCode + " ARO Assembly Name" + assembly.AssemblyName };

                        }

                    }

                }
                else
                {
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Kindly Release Booths of this Sector Officer first in order to deactivate record." };

                }


            }
            else
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Status Can't be Empty" };
            }


        }


        public async Task<List<CombinedMaster>> GetBoothListBySoId(string stateMasterId, string districtMasterId, string assemblyMasterId, string soId)
        {

            var boothlist = from bt in _context.BoothMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId) && d.AssignedTo == soId)
                            join asem in _context.AssemblyMaster
                            on bt.AssemblyMasterId equals asem.AssemblyMasterId
                            join dist in _context.DistrictMaster
                            on asem.DistrictMasterId equals dist.DistrictMasterId
                            join state in _context.StateMaster
                             on dist.StateMasterId equals state.StateMasterId

                            select new CombinedMaster
                            {
                                StateId = Convert.ToInt32(stateMasterId),
                                StateName = state.StateName,
                                DistrictId = dist.DistrictMasterId,
                                DistrictName = dist.DistrictName,
                                DistrictCode = dist.DistrictCode,
                                AssemblyId = asem.AssemblyMasterId,
                                AssemblyName = asem.AssemblyName,
                                AssemblyCode = asem.AssemblyCode,
                                BoothMasterId = bt.BoothMasterId,
                                BoothName = bt.BoothName,
                                //BoothAuxy = bt.BoothNoAuxy,
                                BoothAuxy = (bt.BoothNoAuxy == "0") ? string.Empty : bt.BoothNoAuxy,
                                IsStatus = bt.BoothStatus,
                                BoothCode_No = bt.BoothCode_No,
                                IsAssigned = bt.IsAssigned,
                                soMasterId = Convert.ToInt32(soId)


                            };
            var count = boothlist.Count();
            return await boothlist.ToListAsync();
        }
        public async Task<SectorOfficerMaster> GetSOById(string soMasterId)
        {
            var soRecord = await _context.SectorOfficerMaster.Where(d => d.SOMasterId == Convert.ToInt32(soMasterId)).FirstOrDefaultAsync();
            return soRecord;
        }
        #endregion

        #region Booth Master 
        public async Task<List<CombinedMaster>> GetBoothListById(string stateMasterId, string districtMasterId, string assemblyMasterId)
        {
            var isStateActive = _context.StateMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId)).FirstOrDefault();
            var isDistrictActive = _context.DistrictMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId)).FirstOrDefault();
            var isAssemblyActive = _context.AssemblyMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId)).FirstOrDefault();
            if (isStateActive.StateStatus && isDistrictActive.DistrictStatus && isAssemblyActive.AssemblyStatus)
            {

                var boothlist = from bt in _context.BoothMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId)) // outer sequenc)
                                join asem in _context.AssemblyMaster
                                on bt.AssemblyMasterId equals asem.AssemblyMasterId
                                join dist in _context.DistrictMaster
                                on asem.DistrictMasterId equals dist.DistrictMasterId
                                join state in _context.StateMaster
                                 on dist.StateMasterId equals state.StateMasterId
                                join elec in _context.ElectionTypeMaster
                                on bt.ElectionTypeMasterId equals elec.ElectionTypeMasterId

                                select new CombinedMaster
                                {
                                    StateId = Convert.ToInt32(stateMasterId),
                                    DistrictId = dist.DistrictMasterId,
                                    AssemblyId = asem.AssemblyMasterId,
                                    AssemblyName = asem.AssemblyName,
                                    AssemblyCode = asem.AssemblyCode,
                                    BoothMasterId = bt.BoothMasterId,
                                    //BoothName = bt.BoothName + "(" + bt.BoothCode_No + ")",
                                    //BoothName = bt.BoothName + (bt.BoothNoAuxy != "0" ? $"({bt.BoothCode_No}-{bt.BoothNoAuxy})" : $"({bt.BoothCode_No})"),
                                    BoothName = $"{bt.BoothName}{(bt.BoothNoAuxy != "0" ? $"-{bt.BoothNoAuxy}" : "")}({bt.BoothCode_No})",
                                    SecondLanguage = bt.SecondLanguage,
                                    BoothAuxy = bt.BoothNoAuxy,
                                    BoothCode_No = bt.BoothCode_No,
                                    IsAssigned = bt.IsAssigned,
                                    IsStatus = bt.BoothStatus,
                                    LocationMasterId = bt.LocationMasterId,
                                    ElectionTypeMasterId = bt.ElectionTypeMasterId,
                                    ElectionTypeName = elec.ElectionType


                                };
                var sortedBoothList = await boothlist.ToListAsync();

                // Convert string BoothCode_No to integers for sorting
                sortedBoothList = sortedBoothList.OrderBy(d => int.TryParse(d.BoothCode_No, out int code) ? code : int.MaxValue).ToList();

                return sortedBoothList;
            }
            else
            {
                return null;
            }
        }
        public async Task<List<CombinedMaster>> GetBoothListByIdforPSO(string stateMasterId, string districtMasterId, string assemblyMasterId)
        {
            int stateId = Convert.ToInt32(stateMasterId);
            int districtId = Convert.ToInt32(districtMasterId);
            int assemblyId = Convert.ToInt32(assemblyMasterId);

            var isActive = _context.StateMaster
                .Where(s => s.StateMasterId == stateId && s.StateStatus)
                .Join(_context.DistrictMaster,
                    state => state.StateMasterId,
                    district => district.StateMasterId,
                    (state, district) => new { state, district })
                .Where(sd => sd.district.DistrictMasterId == districtId && sd.district.DistrictStatus)
                .Join(_context.AssemblyMaster,
                    sd => sd.district.DistrictMasterId,
                    assembly => assembly.DistrictMasterId,
                    (sd, assembly) => new { sd, assembly })
                .Where(sda => sda.assembly.AssemblyMasterId == assemblyId && sda.assembly.AssemblyStatus)
                .FirstOrDefault();

            if (isActive != null)
            {
                var boothlist = from bt in _context.BoothMaster
                                join asem in _context.AssemblyMaster on bt.AssemblyMasterId equals asem.AssemblyMasterId
                                join dist in _context.DistrictMaster on asem.DistrictMasterId equals dist.DistrictMasterId
                                join state in _context.StateMaster on dist.StateMasterId equals state.StateMasterId
                                join elecinfo in _context.ElectionInfoMaster on bt.BoothMasterId equals elecinfo.BoothMasterId
                                where !_context.PollingStationMaster.Any(p => p.BoothMasterId == bt.BoothMasterId)
                                   && bt.StateMasterId == Convert.ToInt32(stateMasterId)
                                   && bt.DistrictMasterId == Convert.ToInt32(districtMasterId)
                                   && bt.AssemblyMasterId == Convert.ToInt32(assemblyMasterId)
                                select new CombinedMaster
                                {
                                    StateId = Convert.ToInt32(stateMasterId),
                                    DistrictId = dist.DistrictMasterId,
                                    AssemblyId = asem.AssemblyMasterId,
                                    AssemblyName = asem.AssemblyName,
                                    AssemblyCode = asem.AssemblyCode,
                                    BoothMasterId = bt.BoothMasterId,
                                    BoothName = $"{bt.BoothCode_No}-{bt.BoothName}",
                                    BoothAuxy = bt.BoothNoAuxy,
                                    BoothCode_No = bt.BoothCode_No,
                                    IsAssigned = bt.IsAssigned,
                                    IsStatus = bt.BoothStatus,
                                    LocationMasterId = bt.LocationMasterId,
                                    Male = bt.Male,
                                    Female = bt.Female,
                                    Transgender = bt.Transgender,
                                    NoOfPollingAgent = elecinfo.NoOfPollingAgents
                                };
                var sortedBoothList = await boothlist.ToListAsync();

                // Convert string BoothCode_No to integers for sorting
                sortedBoothList = sortedBoothList.OrderBy(d => int.TryParse(d.BoothCode_No, out int code) ? code : int.MaxValue).ToList();

                return sortedBoothList;
            }
            else
            {
                return null;
            }
        }

        public async Task<List<CombinedMaster>> GetUnassignedBoothListById(string stateMasterId, string districtMasterId, string assemblyMasterId)
        {
            var isStateActive = _context.StateMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId)).FirstOrDefault();
            var isDistrictActive = _context.DistrictMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId)).FirstOrDefault();
            var isAssemblyActive = _context.AssemblyMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId)).FirstOrDefault();
            if (isStateActive.StateStatus && isDistrictActive.DistrictStatus && isAssemblyActive.AssemblyStatus)
            {

                var boothlist = from bt in _context.BoothMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId) && d.IsAssigned == false && d.BoothStatus == true) // outer sequenc)
                                join asem in _context.AssemblyMaster
                                on bt.AssemblyMasterId equals asem.AssemblyMasterId
                                join dist in _context.DistrictMaster
                                on asem.DistrictMasterId equals dist.DistrictMasterId
                                join state in _context.StateMaster
                                 on dist.StateMasterId equals state.StateMasterId
                                orderby bt.BoothNoAuxy
                                select new CombinedMaster
                                {
                                    StateId = Convert.ToInt32(stateMasterId),
                                    DistrictId = dist.DistrictMasterId,
                                    AssemblyId = asem.AssemblyMasterId,
                                    AssemblyName = asem.AssemblyName,
                                    AssemblyCode = asem.AssemblyCode,
                                    BoothMasterId = bt.BoothMasterId,
                                    BoothName = bt.BoothName,
                                    //BoothAuxy = bt.BoothNoAuxy,
                                    BoothAuxy = (bt.BoothNoAuxy == "0") ? string.Empty : bt.BoothNoAuxy,
                                    IsAssigned = bt.IsAssigned,
                                    IsStatus = bt.BoothStatus,
                                    BoothCode_No = bt.BoothCode_No


                                };
                var sortedBoothList = await boothlist.ToListAsync();

                // Convert string BoothCode_No to integers for sorting
                sortedBoothList = sortedBoothList.OrderBy(d => int.TryParse(d.BoothCode_No, out int code) ? code : int.MaxValue).ToList();

                return sortedBoothList;
            }
            else
            {
                return null;
            }
        }
        public async Task<List<CombinedMaster>> GetBoothListByAssemblyId(string stateMasterId, string districtMasterId, string assemblyMasterId)
        {

            var boothlist = from bt in _context.BoothMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId)) // outer sequenc)
                            join asem in _context.AssemblyMaster
                            on bt.AssemblyMasterId equals asem.AssemblyMasterId
                            join dist in _context.DistrictMaster
                            on asem.DistrictMasterId equals dist.DistrictMasterId
                            join state in _context.StateMaster
                             on dist.StateMasterId equals state.StateMasterId

                            select new CombinedMaster
                            {
                                StateName = state.StateName,
                                DistrictId = dist.DistrictMasterId,
                                DistrictName = dist.DistrictName,
                                DistrictCode = dist.DistrictCode,
                                AssemblyId = asem.AssemblyMasterId,
                                AssemblyName = asem.AssemblyName,
                                AssemblyCode = asem.AssemblyCode,
                                BoothMasterId = bt.BoothMasterId,
                                BoothName = bt.BoothName,
                                BoothAuxy = bt.BoothNoAuxy


                            };
            var count = boothlist.Count();
            return await boothlist.ToListAsync();
        }
        public async Task<Response> AddBooth(BoothMaster boothMaster)
        {
            try
            {
                if (boothMaster == null)
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Booth master data is null" };
                if (boothMaster.BoothNoAuxy == "0")
                {
                    var checkBoothName = await _context.BoothMaster.AnyAsync(d =>
                                          d.StateMasterId == boothMaster.StateMasterId &&
                                          d.DistrictMasterId == boothMaster.DistrictMasterId &&
                                          d.AssemblyMasterId == boothMaster.AssemblyMasterId &&
                                          d.BoothCode_No.Equals(boothMaster.BoothCode_No));
                    if (checkBoothName is true)
                        return new Response { Status = RequestStatusEnum.BadRequest, Message = $"The booth Code {boothMaster.BoothCode_No} already exists. You can proceed with an auxiliary booth instead." };
                    else
                        boothMaster.BoothCreatedAt = BharatDateTime(); // Assuming BharatDateTime() returns the current date/time.
                    _context.BoothMaster.Add(boothMaster);
                    await _context.SaveChangesAsync();
                    return new Response { Status = RequestStatusEnum.OK, Message = $"Booth {boothMaster.BoothName} added successfully!" };


                }
                else
                {

                    var existingBooths = await _context.BoothMaster.Where(p =>
                                 p.BoothCode_No == boothMaster.BoothCode_No &&
                                  p.StateMasterId == boothMaster.StateMasterId &&
                                  p.AssemblyMasterId == boothMaster.AssemblyMasterId &&
                                  p.ElectionTypeMasterId == boothMaster.ElectionTypeMasterId).ToListAsync();



                    if (existingBooths.Any())
                    {
                        var existingAuxCodes = existingBooths.Select(b => new { b.BoothNoAuxy, b.BoothCode_No });
                        if (existingAuxCodes.Any(c => c.BoothNoAuxy.Equals(boothMaster.BoothNoAuxy) && c.BoothCode_No.Equals(boothMaster.BoothCode_No)))
                        {
                            return new Response { Status = RequestStatusEnum.BadRequest, Message = $"{boothMaster.BoothName} with AuxilaryCode {boothMaster.BoothNoAuxy} and BoothCode {boothMaster.BoothCode_No} already exists" };
                        }
                        else
                        {
                            boothMaster.BoothCreatedAt = BharatDateTime(); // Assuming BharatDateTime() returns the current date/time.
                            _context.BoothMaster.Add(boothMaster);
                            await _context.SaveChangesAsync();
                            return new Response { Status = RequestStatusEnum.OK, Message = $"Booth {boothMaster.BoothName} added successfully!" };
                        }
                    }
                    else
                    {
                        var assemblyActive = await _context.AssemblyMaster
                            .Where(p => p.AssemblyMasterId == boothMaster.AssemblyMasterId)
                            .Select(p => p.AssemblyStatus)
                            .FirstOrDefaultAsync();

                        if (assemblyActive)
                        {
                            if (boothMaster.Male + boothMaster.Female + boothMaster.Transgender == boothMaster.TotalVoters)
                            {
                                boothMaster.BoothCreatedAt = BharatDateTime(); // Assuming BharatDateTime() returns the current date/time.
                                _context.BoothMaster.Add(boothMaster);
                                await _context.SaveChangesAsync();
                                return new Response { Status = RequestStatusEnum.OK, Message = $"Booth {boothMaster.BoothName} added successfully!" };
                            }
                            else
                            {
                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "The total sum of voters does not match the individual counts of Male, Female, and Transgender categories." };
                            }
                        }
                        else
                        {
                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Assembly is not active for this booth. Kindly activate the Assembly in order to Add Booth." };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception details for troubleshooting
                return new Response { Status = RequestStatusEnum.BadRequest, Message = ex.Message };
            }
        }

        //public async Task<Response> UpdateBooth(BoothMaster boothMaster)
        //{
        //    if (boothMaster.BoothName != string.Empty)
        //    {
        //        var existingbooth = await _context.BoothMaster.FirstOrDefaultAsync(so => so.BoothMasterId == boothMaster.BoothMasterId);

        //        if (existingbooth == null)
        //        {
        //            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Booth Record Not Found" };
        //        }
        //        else
        //        {
        //            if (boothMaster.Male + boothMaster.Female + boothMaster.Transgender == boothMaster.TotalVoters)
        //            {

        //                var isExist = await _context.BoothMaster.Where(p => p.BoothCode_No == boothMaster.BoothCode_No && p.StateMasterId == boothMaster.StateMasterId && p.BoothMasterId != boothMaster.BoothMasterId && p.AssemblyMasterId == boothMaster.AssemblyMasterId).ToListAsync();

        //                if (isExist.Count == 0)
        //                {

        //                    if (existingbooth != null)
        //                    {

        //                        var electionInfoRecord = _context.ElectionInfoMaster
        //                                  .Where(d => d.StateMasterId == boothMaster.StateMasterId && d.DistrictMasterId == boothMaster.DistrictMasterId && d.AssemblyMasterId == boothMaster.AssemblyMasterId && d.BoothMasterId == boothMaster.BoothMasterId)
        //                                 .FirstOrDefault();
        //                        if (electionInfoRecord == null)
        //                        {
        //                            // can update

        //                            if (existingbooth.AssignedTo == null || existingbooth.AssignedTo == "")
        //                            {
        //                                // when updating False
        //                                if (boothMaster.BoothStatus == false)
        //                                {
        //                                    existingbooth.LocationMasterId = null;
        //                                    existingbooth.BoothName = boothMaster.BoothName;
        //                                    existingbooth.BoothCode_No = boothMaster.BoothCode_No;
        //                                    existingbooth.BoothNoAuxy = boothMaster.BoothNoAuxy;
        //                                    existingbooth.Longitude = boothMaster.Longitude;
        //                                    existingbooth.Latitude = boothMaster.Latitude;
        //                                    existingbooth.BoothUpdatedAt = BharatDateTime();
        //                                    existingbooth.TotalVoters = boothMaster.TotalVoters;
        //                                    existingbooth.BoothStatus = boothMaster.BoothStatus;
        //                                    existingbooth.Male = boothMaster.Male;
        //                                    existingbooth.Female = boothMaster.Female;
        //                                    existingbooth.Transgender = boothMaster.Transgender;

        //                                    _context.BoothMaster.Update(existingbooth);
        //                                    await _context.SaveChangesAsync();
        //                                    return new Response { Status = RequestStatusEnum.OK, Message = "Booth is Unmapped from Location and Booth is Inactive." };

        //                                }
        //                                //if (boothMaster.BoothStatus && (!_context.AssemblyMaster.Any(s => s.AssemblyMasterId == boothMaster.AssemblyMasterId && s.AssemblyStatus == true)))
        //                                //{

        //                                //    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Assembly must be active in order to activate Booth." };

        //                                //}

        //                                else if (boothMaster.BoothStatus == true)

        //                                {
        //                                    var isassmblytrue = _context.AssemblyMaster.Any(s => s.AssemblyMasterId == boothMaster.AssemblyMasterId && s.AssemblyStatus == true);
        //                                    if (isassmblytrue == false)
        //                                    {
        //                                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Assembly must be active and Booth Location can't be null in order to activate Booth." };

        //                                    }
        //                                    else
        //                                    {

        //                                        //var islocationtrue = _context.PollingLocationMaster.Any(s => s.LocationMasterId == boothMaster.LocationMasterId && s.Status == true);
        //                                        //if(islocationtrue == false)
        //                                        //{
        //                                        //    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Location must be active and Booth Location can't be null in order to activate Booth." };

        //                                        //}
        //                                        //else
        //                                        //{
        //                                        //save
        //                                        existingbooth.BoothName = boothMaster.BoothName;
        //                                        existingbooth.BoothCode_No = boothMaster.BoothCode_No;
        //                                        existingbooth.BoothNoAuxy = boothMaster.BoothNoAuxy;
        //                                        existingbooth.Longitude = boothMaster.Longitude;
        //                                        existingbooth.Latitude = boothMaster.Latitude;
        //                                        existingbooth.BoothUpdatedAt = BharatDateTime();
        //                                        existingbooth.TotalVoters = boothMaster.TotalVoters;
        //                                        existingbooth.BoothStatus = boothMaster.BoothStatus;
        //                                        existingbooth.Male = boothMaster.Male;
        //                                        existingbooth.Female = boothMaster.Female;
        //                                        existingbooth.Transgender = boothMaster.Transgender;
        //                                        existingbooth.AssemblyMasterId = boothMaster.AssemblyMasterId;
        //                                        existingbooth.DistrictMasterId = boothMaster.DistrictMasterId;
        //                                        existingbooth.StateMasterId = boothMaster.StateMasterId;
        //                                        _context.BoothMaster.Update(existingbooth);
        //                                        await _context.SaveChangesAsync();

        //                                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Booth Updated Successfully. Kindly Map your Booth Location." };
        //                                        // }

        //                                    }
        //                                }

        //                                else
        //                                {
        //                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Please Select Active/InActive Status" };

        //                                }

        //                            }
        //                            else
        //                            {
        //                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Booth Is allocated to a Sector Officer, Kindly Release Booth First." };
        //                            }
        //                        }
        //                        else
        //                        {
        //                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Election Info Record found aganist this Booth, thus can't change status" };

        //                        }
        //                    }
        //                    else
        //                    {
        //                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Booth record Not Found." };

        //                    }


        //                }
        //                else
        //                {
        //                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Booth with Same Code Already Exists in the State: " + string.Join(", ", isExist.Select(p => $"{p.BoothName} ({p.BoothCode_No})")) };
        //                }



        //                return new Response { Status = RequestStatusEnum.OK, Message = "Booth" + existingbooth.BoothName.Trim() + " updated successfully!" };
        //            }
        //            else
        //            {
        //                return new Response { Status = RequestStatusEnum.BadRequest, Message = "The total sum of voters does not match the individual counts of Male, Female, and Transgender categories." };

        //            }
        //        }
        //    }
        //    else
        //    {
        //        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Booth Name cannot Be Empty" };

        //    }

        //}

        public async Task<Response> UpdateBooth(BoothMaster boothMaster)
        {
            if (boothMaster.BoothName != string.Empty)
            {
                var existingbooth = await _context.BoothMaster.FirstOrDefaultAsync(so => so.BoothMasterId == boothMaster.BoothMasterId);

                if (existingbooth == null)
                {
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Booth Record Not Found" };
                }
                else
                {
                    if (boothMaster.Male + boothMaster.Female + boothMaster.Transgender == boothMaster.TotalVoters)
                    {

                        //var isExist = await _context.BoothMaster.Where(p => p.BoothCode_No == boothMaster.BoothCode_No && p.StateMasterId == boothMaster.StateMasterId 
                        //&& p.BoothMasterId != boothMaster.BoothMasterId && p.AssemblyMasterId == boothMaster.AssemblyMasterId).ToListAsync();

                        //if (isExist.Count == 0)
                        //{

                        if (existingbooth != null)
                        {
                            if (boothMaster.ElectionTypeMasterId != null)
                            {
                                //get assembly electiontype id
                                // Assuming _context is your DbContext instance and assemblyMaster is defined
                                // DistrictMasterId is the identifier you have and want to filter with

                                var electionAssemblyTypeId = _context.AssemblyMaster
                                    .Where(s => s.AssemblyMasterId == boothMaster.AssemblyMasterId)
                                    .Select(s => s.ElectionTypeMasterId)
                                    .FirstOrDefault(); // Assuming you expect only one result or want the first one

                                // electionTypeId will contain the ElectionPointTypeId if found, otherwise default value (null for reference types)

                                if (boothMaster.ElectionTypeMasterId == electionAssemblyTypeId)
                                {
                                    var electionInfoRecord = _context.ElectionInfoMaster
                                          .Where(d => d.StateMasterId == boothMaster.StateMasterId && d.DistrictMasterId == boothMaster.DistrictMasterId && d.AssemblyMasterId == boothMaster.AssemblyMasterId && d.BoothMasterId == boothMaster.BoothMasterId)
                                        .FirstOrDefault();

                                    //means election_info null,also booth not mapped
                                    if (electionInfoRecord == null && (existingbooth.AssignedTo == null || existingbooth.AssignedTo == ""))
                                    {
                                        if (boothMaster.BoothStatus == false)
                                        {
                                            existingbooth.LocationMasterId = null;
                                            existingbooth.BoothName = boothMaster.BoothName;
                                            existingbooth.BoothCode_No = boothMaster.BoothCode_No;
                                            // existingbooth.BoothNoAuxy = boothMaster.BoothNoAuxy;
                                            existingbooth.ElectionTypeMasterId = boothMaster.ElectionTypeMasterId;
                                            existingbooth.Longitude = boothMaster.Longitude;
                                            existingbooth.Latitude = boothMaster.Latitude;
                                            existingbooth.BoothUpdatedAt = BharatDateTime();
                                            existingbooth.TotalVoters = boothMaster.TotalVoters;
                                            existingbooth.BoothStatus = boothMaster.BoothStatus;
                                            existingbooth.Male = boothMaster.Male;
                                            existingbooth.Female = boothMaster.Female;
                                            existingbooth.Transgender = boothMaster.Transgender;

                                            _context.BoothMaster.Update(existingbooth);
                                            await _context.SaveChangesAsync();
                                            return new Response { Status = RequestStatusEnum.OK, Message = "Booth Record Updaed Sucessfully, and Booth is Unmapped from Location." };

                                        }


                                        else if (boothMaster.BoothStatus == true)

                                        {
                                            var isassmblytrue = _context.AssemblyMaster.Any(s => s.AssemblyMasterId == boothMaster.AssemblyMasterId && s.AssemblyStatus == true);
                                            if (isassmblytrue == false)
                                            {
                                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Assembly must be active and Booth Location can't be null in order to activate Booth." };

                                            }
                                            else
                                            {

                                                existingbooth.BoothName = boothMaster.BoothName;
                                                existingbooth.BoothCode_No = boothMaster.BoothCode_No;
                                                // existingbooth.BoothNoAuxy = boothMaster.BoothNoAuxy;
                                                existingbooth.Longitude = boothMaster.Longitude;
                                                existingbooth.Latitude = boothMaster.Latitude;
                                                existingbooth.BoothUpdatedAt = BharatDateTime();
                                                existingbooth.TotalVoters = boothMaster.TotalVoters;
                                                existingbooth.BoothStatus = boothMaster.BoothStatus;
                                                existingbooth.Male = boothMaster.Male;
                                                existingbooth.Female = boothMaster.Female;
                                                existingbooth.Transgender = boothMaster.Transgender;
                                                existingbooth.AssemblyMasterId = boothMaster.AssemblyMasterId;
                                                existingbooth.DistrictMasterId = boothMaster.DistrictMasterId;
                                                existingbooth.StateMasterId = boothMaster.StateMasterId;
                                                _context.BoothMaster.Update(existingbooth);
                                                await _context.SaveChangesAsync();

                                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Booth Updated Successfully. Kindly Map your Booth Location." };


                                            }
                                        }

                                        else
                                        {
                                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Please Select Active/InActive Status" };

                                        }
                                    }

                                    //means info null, but booth mapped, that case 4 fields can editable--> changed method
                                    else if (electionInfoRecord == null && existingbooth.AssignedTo != null)
                                    {  // can update only 4 fields
                                        if (existingbooth.BoothName == boothMaster.BoothName && existingbooth.BoothCode_No == boothMaster.BoothCode_No &&
                                            existingbooth.DistrictMasterId == boothMaster.DistrictMasterId && existingbooth.AssemblyMasterId == boothMaster.AssemblyMasterId
                                            && existingbooth.BoothStatus == boothMaster.BoothStatus && existingbooth.BoothNoAuxy == boothMaster.BoothNoAuxy && existingbooth.ElectionTypeMasterId == boothMaster.ElectionTypeMasterId)
                                        {

                                            if (boothMaster.BoothStatus == false)
                                            {
                                                existingbooth.LocationMasterId = null;
                                                //existingbooth.BoothStatus = boothMaster.BoothStatus;
                                                existingbooth.BoothUpdatedAt = BharatDateTime();
                                                existingbooth.ElectionTypeMasterId = boothMaster.ElectionTypeMasterId;
                                                existingbooth.TotalVoters = boothMaster.TotalVoters;
                                                existingbooth.Male = boothMaster.Male;
                                                existingbooth.Female = boothMaster.Female;
                                                existingbooth.Transgender = boothMaster.Transgender;
                                                _context.BoothMaster.Update(existingbooth);
                                                await _context.SaveChangesAsync();
                                                //return new Response { Status = RequestStatusEnum.OK, Message = "Booth Updated successfully except BoothName/Number/District/Assembly/Status/Aux Booth is Unmapped from Location and Booth is Inactive." };
                                                return new Response { Status = RequestStatusEnum.OK, Message = "Booth Updated successfully except BoothName/Number/District/Assembly/Status/Aux. Kindly Release Booth in order to update all fields" };

                                            }
                                            else if (boothMaster.BoothStatus == true)

                                            {
                                                var isassmblytrue = _context.AssemblyMaster.Any(s => s.AssemblyMasterId == boothMaster.AssemblyMasterId && s.AssemblyStatus == true);
                                                if (isassmblytrue == false)
                                                {
                                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Assembly must be active and Booth Location can't be null in order to activate Booth." };

                                                }
                                                else
                                                {


                                                    existingbooth.ElectionTypeMasterId = boothMaster.ElectionTypeMasterId;
                                                    existingbooth.BoothUpdatedAt = BharatDateTime();
                                                    existingbooth.TotalVoters = boothMaster.TotalVoters;
                                                    //existingbooth.BoothStatus = boothMaster.BoothStatus;
                                                    existingbooth.Male = boothMaster.Male;
                                                    existingbooth.Female = boothMaster.Female;
                                                    existingbooth.Transgender = boothMaster.Transgender;

                                                    _context.BoothMaster.Update(existingbooth);
                                                    await _context.SaveChangesAsync();
                                                    return new Response { Status = RequestStatusEnum.OK, Message = "Booth Updated successfully except BoothName/Number/District/Assembly/Status/Aux. Kindly Release Booth in order to update all fields" };



                                                }
                                            }
                                            else
                                            {
                                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Please Select Active/InActive Status" };

                                            }
                                        }
                                        else
                                        {
                                            return new Response { Status = RequestStatusEnum.BadRequest, Message = " Kindly Release Booth in order to update fields." };
                                        }
                                    }

                                    else if (electionInfoRecord != null)
                                    {
                                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Election Info Record found aganist this Booth, thus can't change status" };

                                    }
                                    else
                                    {
                                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Problem while Updating Booth" };

                                    }
                                }
                                else
                                {
                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "The election types of Booth and assembly are different." };

                                }
                            }
                            else
                            {
                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Election Type Can't be Null" };

                            }

                            //end
                        }
                        else
                        {
                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Booth record Not Found." };

                        }


                        //}
                        //else
                        //{
                        //    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Booth with Same Code Already Exists in the State: " + string.Join(", ", isExist.Select(p => $"{p.BoothName} ({p.BoothCode_No})")) };
                        //}




                    }
                    else
                    {
                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "The total sum of voters does not match the individual counts of Male, Female, and Transgender categories." };

                    }
                }
            }
            else
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Booth Name cannot Be Empty" };

            }

        }

        public async Task<Response> BoothMapping(List<BoothMaster> boothMasters)
        {
            string anyBoothLocationFalse = "";
            foreach (var boothMaster in boothMasters)

            {
                var existingBooth = _context.BoothMaster.Where(d =>
                        d.StateMasterId == boothMaster.StateMasterId &&
                        d.DistrictMasterId == boothMaster.DistrictMasterId &&
                        d.AssemblyMasterId == boothMaster.AssemblyMasterId && d.BoothMasterId == boothMaster.BoothMasterId && d.ElectionTypeMasterId == boothMaster.ElectionTypeMasterId).FirstOrDefault();
                if (existingBooth != null)
                {
                    //if (existingBooth.LocationMasterId > 0)
                    //{
                    // checking booth must b active and should have location
                    if (existingBooth.BoothStatus == true)
                    {

                        // now check location should eb active
                        //var locationStatus = _context.PollingLocationMaster.Where(p => p.LocationMasterId == existingBooth.LocationMasterId).Select(p => p.Status).FirstOrDefault();
                        //if (locationStatus == false)
                        //{
                        //    anyBoothLocationFalse += existingBooth.BoothCode_No + ",";



                        //}
                        //else
                        //{
                        var soExists = _context.SectorOfficerMaster.Any(p => p.SOMasterId == Convert.ToInt32(boothMaster.AssignedTo));
                        if (soExists == true)
                        {

                            // check that booth i
                            existingBooth.AssignedBy = boothMaster.AssignedBy;
                            existingBooth.AssignedTo = boothMaster.AssignedTo;
                            existingBooth.AssignedOnTime = DateTime.UtcNow;
                            existingBooth.IsAssigned = boothMaster.IsAssigned;
                            _context.BoothMaster.Update(existingBooth);
                            _context.SaveChanges();
                        }
                        else
                        {
                            return new Response { Status = RequestStatusEnum.NotFound, Message = "Sector Officer Not Found" };
                        }
                        // }


                    }
                    else
                    {
                        return new Response { Status = RequestStatusEnum.NotFound, Message = "Booth is Not Active" };

                    }
                    //}
                    //else

                    //{
                    //    return new Response { Status = RequestStatusEnum.NotFound, Message = "Kindly map your Booth Location" };

                    //}
                }
                else
                {
                    return new Response { Status = RequestStatusEnum.NotFound, Message = "Booth Not Found" };

                }
            }
            if (anyBoothLocationFalse == string.Empty)
            {
                return new Response { Status = RequestStatusEnum.OK, Message = "Booths assigned successfully!" };
            }
            else
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Following Booth Number Locations is not active :" + anyBoothLocationFalse + " " + "Kindly Active this Booth's Location, or Add Location if not added." };

                //if (boothMasters.Count > 1)
                //{

                //    return new Response { Status = RequestStatusEnum.OK, Message = "Few Booths assigned successfully! Following Booth's Location is not active :" + anyBoothLocationFalse };
                //}
                //else
                //{
                //    return new Response { Status = RequestStatusEnum.OK, Message = "Following Booth Location is not active :" + anyBoothLocationFalse };

                //}
            }


        }
        public async Task<Response> ReleaseBooth(BoothMaster boothMaster)
        {
            if (boothMaster.BoothMasterId != null)
            {
                if (boothMaster.IsAssigned == false)
                {
                    var electionInfoRecord = await _context.ElectionInfoMaster.FirstOrDefaultAsync(e => e.BoothMasterId == boothMaster.BoothMasterId);
                    if (electionInfoRecord == null)

                    {
                        var existingbooth = await _context.BoothMaster.FirstOrDefaultAsync(so => so.BoothMasterId == boothMaster.BoothMasterId && so.StateMasterId == boothMaster.StateMasterId && so.DistrictMasterId == so.DistrictMasterId && so.AssemblyMasterId == boothMaster.AssemblyMasterId);
                        if (existingbooth == null)
                        {
                            return new Response { Status = RequestStatusEnum.NotFound, Message = "Booth Record not found." };
                        }
                        else
                        {
                            if (existingbooth.IsAssigned == true)
                            {
                                existingbooth.AssignedBy = string.Empty;
                                existingbooth.AssignedTo = string.Empty;
                                existingbooth.IsAssigned = boothMaster.IsAssigned;
                                _context.BoothMaster.Update(existingbooth);
                                await _context.SaveChangesAsync();

                                return new Response { Status = RequestStatusEnum.OK, Message = "Booth " + existingbooth.BoothName.Trim() + " Unassigned successfully!" };
                            }
                            else
                            {
                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Booth " + existingbooth.BoothName.Trim() + " already Unassigned!" };
                            }
                        }

                    }
                    else
                    {
                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Cannot release booth as Event Activity has been performed on it." };
                    }
                }
                else
                {
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Please unassign first!" };


                }
            }
            else
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Record not found!" };

            }
        }
        public async Task<BoothMaster> GetBoothById(string boothMasterId)
        {
            var boothRecord = await _context.BoothMaster.Include(d => d.StateMaster).Include(d => d.DistrictMaster).Include(d => d.AssemblyMaster).Include(d => d.ElectionTypeMaster).Where(d => d.BoothMasterId == Convert.ToInt32(boothMasterId)).FirstOrDefaultAsync();

            return boothRecord;
        }
        #endregion

        #region Event Master

        public async Task<List<EventMaster>> GetEventList()
        {
            //var eventData = await _context.EventMaster.Where(d => d.EventMasterId != 14)
            var eventData = await _context.EventMaster.Where(d => d.EventName != null)
            .OrderBy(d => d.EventSequence) // Add this line for ordering
            .Select(d => new EventMaster
            {
                EventMasterId = d.EventMasterId,
                EventName = d.EventName,
                EventSequence = d.EventSequence,
                Status = d.Status
            })
            .ToListAsync();

            return eventData;
        }
        public async Task<ServiceResponse> UpdateEventStaus(EventMaster eventMaster)
        {
            var isExist = _context.EventMaster.Where(d => d.EventMasterId == eventMaster.EventMasterId).FirstOrDefault();
            if (isExist != null)
            {
                isExist.Status = eventMaster.Status;
                _context.EventMaster.Update(isExist);
                _context.SaveChanges();
                return new ServiceResponse
                {
                    IsSucceed = true,

                };
            }
            else
            {
                return new ServiceResponse
                {
                    IsSucceed = false,
                };
            }
        }
        public async Task<Response> UpdateEventById(EventMaster eventMaster)
        {
            if (eventMaster.EventName != null && eventMaster.EventSequence != null)
            {

                var eventExist = _context.EventMaster.Where(d => d.EventMasterId == eventMaster.EventMasterId).FirstOrDefault();
                if (eventExist != null)
                {
                    eventExist.EventName = eventMaster.EventName;
                    eventExist.Status = eventMaster.Status;
                    eventExist.EventSequence = eventMaster.EventSequence;
                    eventExist.UpdatedAt = BharatDateTime();
                    _context.EventMaster.Update(eventExist);
                    _context.SaveChanges();
                    return new Response { Status = RequestStatusEnum.OK, Message = "Event+" + eventMaster.EventName + " " + "added successfully" };
                }
                else
                {
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Record not Found" };
                }

            }

            else
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Event Name & Sequence cannot Be Empty" };

            }

        }
        public async Task<List<EventWiseBooth>> GetBoothListByEventId(string eventId, string soId)
        {
            var soTotalBooths = _context.BoothMaster.Where(p => p.AssignedTo == soId).ToList();
            List<EventWiseBooth> list = new List<EventWiseBooth>();

            foreach (var boothRecord in soTotalBooths)
            {
                string boothName = "";
                if (boothRecord.BoothNoAuxy == "0" || boothRecord.BoothNoAuxy == "")
                {

                    boothName = boothRecord.BoothName;

                }
                else
                {
                    boothName = boothRecord.BoothName + "-" + boothRecord.BoothNoAuxy;
                }
                var electioInfoRecord = _context.ElectionInfoMaster.FirstOrDefault(d =>
                    d.BoothMasterId == boothRecord.BoothMasterId);

                if (electioInfoRecord is not null)
                {
                    EventWiseBooth eventWiseBooth = new EventWiseBooth()
                    {
                        StateMasterId = boothRecord.StateMasterId,
                        DistrictMasterId = boothRecord.DistrictMasterId,
                        AssemblyMasterId = boothRecord.AssemblyMasterId,
                        BoothMasterId = boothRecord.BoothMasterId,
                        BoothName = boothName,
                        BoothCode = boothRecord.BoothCode_No,
                        EventMasterId = electioInfoRecord.EventMasterId,
                        EventName = _context.EventMaster
                            .Where(d => d.EventMasterId == Convert.ToInt32(eventId))
                            .Select(e => e.EventName)
                            .FirstOrDefault(),
                        UpdateStatus = GetUpdateStatus(eventId, electioInfoRecord),
                        //  Color = GetBoothInfoinPollDetail_test(boothRecord.BoothMasterId)

                    };

                    list.Add(eventWiseBooth);
                }
                else
                {
                    EventWiseBooth eventWiseBooth = new EventWiseBooth()
                    {
                        StateMasterId = boothRecord.StateMasterId,
                        DistrictMasterId = boothRecord.DistrictMasterId,
                        AssemblyMasterId = boothRecord.AssemblyMasterId,
                        BoothMasterId = boothRecord.BoothMasterId,
                        BoothName = boothName,
                        BoothCode = boothRecord.BoothCode_No,
                        EventMasterId = Convert.ToInt32(eventId),
                        EventName = _context.EventMaster
                            .Where(d => d.EventMasterId == Convert.ToInt32(eventId))
                            .Select(e => e.EventName)
                            .FirstOrDefault(),
                        UpdateStatus = false
                    };

                    list.Add(eventWiseBooth);
                }
            }

            return list;
        }

        //public async Task<List<EventActivityWiseBooth>> GetBoothEventActivityById(string soId)
        //{
        //    var soTotalBooths = _context.BoothMaster.Where(p => p.AssignedTo == soId).ToList();
        //    List<EventActivityWiseBooth> list = new List<EventActivityWiseBooth>();

        //    foreach (var boothRecord in soTotalBooths)
        //    {
        //        var electionInfoRecord = _context.ElectionInfoMaster.FirstOrDefault(d =>
        //            d.BoothMasterId == boothRecord.BoothMasterId);

        //        if (electionInfoRecord is not null)
        //        {
        //            List<int> activityIds = new List<int>();

        //            if (electionInfoRecord.IsPartyDispatched == true)
        //                activityIds.Add((int)ElectionEvent.PartyDispatch);

        //            if (electionInfoRecord.IsPartyReached == true)
        //                activityIds.Add((int)ElectionEvent.PartyReached);

        //            if (electionInfoRecord.IsSetupOfPolling == true)
        //                activityIds.Add((int)ElectionEvent.SetupPollingStation);

        //            if (electionInfoRecord.IsMockPollDone == true)
        //                activityIds.Add((int)ElectionEvent.MockPollDone);

        //            if (electionInfoRecord.IsPollStarted == true)
        //                activityIds.Add((int)ElectionEvent.PollStarted);

        //            if (electionInfoRecord.IsVoterTurnOut == true)
        //                activityIds.Add((int)ElectionEvent.VoterTurnOut);

        //            if (electionInfoRecord.VoterInQueue != null)
        //                activityIds.Add((int)ElectionEvent.VoterInQueue);

        //            if (electionInfoRecord.FinalTVoteStatus == true)
        //                activityIds.Add((int)ElectionEvent.FinalVotes);

        //            if (electionInfoRecord.IsPollEnded == true)
        //                activityIds.Add((int)ElectionEvent.PollEnded);

        //            if (electionInfoRecord.IsMCESwitchOff == true)
        //                activityIds.Add((int)ElectionEvent.MCEVM);

        //            if (electionInfoRecord.IsPartyDeparted == true)
        //                activityIds.Add((int)ElectionEvent.PartyDeparted);

        //            if (electionInfoRecord.IsPartyReachedCollectionCenter == true)
        //                activityIds.Add((int)ElectionEvent.PartyReachedCollectionCentre);

        //            if (electionInfoRecord.IsEVMDeposited == true)
        //                activityIds.Add((int)ElectionEvent.EVMDeposited);

        //            EventActivityWiseBooth eventWiseBooth = new EventActivityWiseBooth()
        //            {
        //                StateMasterId = boothRecord.StateMasterId,
        //                DistrictMasterId = boothRecord.DistrictMasterId,
        //                AssemblyMasterId = boothRecord.AssemblyMasterId,
        //                BoothMasterId = boothRecord.BoothMasterId,
        //                BoothName = boothRecord.BoothName,
        //                BoothCode = boothRecord.BoothCode_No,
        //                EventsCompleted = activityIds,
        //                UpdateStatus = false
        //            };

        //            list.Add(eventWiseBooth);
        //        }
        //        else
        //        {
        //            List<int> activityIds1 = new List<int>();
        //            activityIds1.Add(0);
        //            EventActivityWiseBooth eventWiseBooth = new EventActivityWiseBooth()
        //            {
        //                StateMasterId = boothRecord.StateMasterId,
        //                DistrictMasterId = boothRecord.DistrictMasterId,
        //                AssemblyMasterId = boothRecord.AssemblyMasterId,
        //                BoothMasterId = boothRecord.BoothMasterId,
        //                BoothName = boothRecord.BoothName,
        //                BoothCode = boothRecord.BoothCode_No,
        //                EventsCompleted = activityIds1,
        //                UpdateStatus = false
        //            };

        //            list.Add(eventWiseBooth);
        //        }
        //    }

        //    return list;
        //}

        public async Task<List<EventActivityWiseBooth>> GetBoothEventActivityById(string soId)
        {
            List<EventActivityWiseBooth> list = new List<EventActivityWiseBooth>();

            // Establish a connection to the PostgreSQL database
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("Postgres"));
            await connection.OpenAsync();

            // Create a NpgsqlCommand object to execute the function
            var command = new NpgsqlCommand("SELECT * FROM GetBoothEventActivityById(@soId)", connection);
            command.Parameters.AddWithValue("@soId", soId);

            // Execute the command and read the results
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                // Create a new EventActivityWiseBooth object and populate its properties from the reader

                string boothauxy = reader.IsDBNull(8) ? (string?)null : reader.GetString(8); string boothName = "";
                if (boothauxy == null || boothauxy == "0")
                {
                    boothauxy = reader.GetString(4);
                }
                else
                {
                    boothauxy = reader.IsDBNull(4) ? null : reader.GetString(4) + "-" + reader.GetString(8);
                }
                var eventActivityWiseBooth = new EventActivityWiseBooth
                {
                    StateMasterId = reader.GetInt32(0),
                    DistrictMasterId = reader.GetInt32(1),
                    AssemblyMasterId = reader.GetInt32(2),
                    BoothMasterId = reader.GetInt32(3),
                    //BoothName = reader.GetString(4),
                    BoothName = boothauxy,
                    BoothCode = reader.GetString(5),
                    EventsCompleted = reader.GetFieldValue<int[]>(6).ToList(),
                    UpdateStatus = reader.GetBoolean(7)
                };

                // Add the object to the list
                list.Add(eventActivityWiseBooth);
            }

            // Return the list of EventActivityWiseBooth objects
            return list;
        }

        public async Task<List<EventWiseBooth>> GetBoothStatusforARO(string assemblyMasterId, string boothMasterId)
        {
            var soTotalBooths = _context.BoothMaster.FirstOrDefault(d =>
                   d.BoothMasterId == Convert.ToInt32(boothMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId));


            List<EventWiseBooth> eventwiseboothlist = new List<EventWiseBooth>();
            var electioInfoRecord = _context.ElectionInfoMaster.FirstOrDefault(d =>
                   d.BoothMasterId == Convert.ToInt32(boothMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId));

            var event_lits = _context.EventMaster.Where(p => p.Status == true).OrderBy(p => p.EventSequence).ToList();

            if (electioInfoRecord is not null)
            {
                foreach (var eventList in event_lits)
                {
                    string boothName = "";
                    if (soTotalBooths.BoothNoAuxy != "0" && soTotalBooths.BoothNoAuxy != "")
                    {
                        boothName = soTotalBooths.BoothNoAuxy;
                    }
                    if (eventList.EventMasterId == 1)
                    {

                        EventWiseBooth model = new EventWiseBooth()
                        {
                            StateMasterId = soTotalBooths.StateMasterId,
                            DistrictMasterId = soTotalBooths.DistrictMasterId,
                            EventMasterId = eventList.EventMasterId,
                            EventName = eventList.EventName,
                            AssemblyMasterId = soTotalBooths.AssemblyMasterId,
                            BoothMasterId = Convert.ToInt32(soTotalBooths.BoothMasterId),
                            BoothName = soTotalBooths.BoothName + "-" + boothName,
                            BoothCode = soTotalBooths.BoothCode_No,
                            UpdateStatus = electioInfoRecord.IsPartyDispatched ?? false
                        };

                        eventwiseboothlist.Add(model);
                    }
                    else if (eventList.EventMasterId == 2)
                    {
                        EventWiseBooth model = new EventWiseBooth()
                        {
                            StateMasterId = soTotalBooths.StateMasterId,
                            DistrictMasterId = soTotalBooths.DistrictMasterId,
                            EventMasterId = eventList.EventMasterId,
                            EventName = eventList.EventName,
                            AssemblyMasterId = soTotalBooths.AssemblyMasterId,
                            BoothMasterId = Convert.ToInt32(soTotalBooths.BoothMasterId),
                            BoothName = soTotalBooths.BoothName + "-" + boothName,
                            BoothCode = soTotalBooths.BoothCode_No,
                            UpdateStatus = electioInfoRecord.IsPartyReached ?? false
                        };

                        eventwiseboothlist.Add(model);
                    }
                    else if (eventList.EventMasterId == 3)
                    {
                        EventWiseBooth model = new EventWiseBooth()
                        {
                            StateMasterId = soTotalBooths.StateMasterId,
                            DistrictMasterId = soTotalBooths.DistrictMasterId,
                            EventMasterId = eventList.EventMasterId,
                            EventName = eventList.EventName,
                            AssemblyMasterId = soTotalBooths.AssemblyMasterId,
                            BoothMasterId = Convert.ToInt32(soTotalBooths.BoothMasterId),
                            BoothName = soTotalBooths.BoothName + "-" + boothName,
                            BoothCode = soTotalBooths.BoothCode_No,
                            UpdateStatus = electioInfoRecord.IsSetupOfPolling ?? false
                        };
                        eventwiseboothlist.Add(model);
                    }
                    else if (eventList.EventMasterId == 4)
                    {

                        EventWiseBooth model = new EventWiseBooth()
                        {
                            StateMasterId = soTotalBooths.StateMasterId,
                            DistrictMasterId = soTotalBooths.DistrictMasterId,
                            EventMasterId = eventList.EventMasterId,
                            EventName = eventList.EventName,
                            AssemblyMasterId = soTotalBooths.AssemblyMasterId,
                            BoothMasterId = Convert.ToInt32(soTotalBooths.BoothMasterId),
                            BoothName = soTotalBooths.BoothName + "-" + boothName,
                            BoothCode = soTotalBooths.BoothCode_No,
                            UpdateStatus = electioInfoRecord.IsMockPollDone ?? false
                        };
                        eventwiseboothlist.Add(model);
                    }
                    else if (eventList.EventMasterId == 5)
                    {
                        EventWiseBooth model = new EventWiseBooth()
                        {
                            StateMasterId = soTotalBooths.StateMasterId,
                            DistrictMasterId = soTotalBooths.DistrictMasterId,
                            EventMasterId = eventList.EventMasterId,
                            EventName = eventList.EventName,
                            AssemblyMasterId = soTotalBooths.AssemblyMasterId,
                            BoothMasterId = Convert.ToInt32(soTotalBooths.BoothMasterId),
                            BoothName = soTotalBooths.BoothName + "-" + boothName,
                            BoothCode = soTotalBooths.BoothCode_No,
                            UpdateStatus = electioInfoRecord.IsPollStarted ?? false
                        };
                        eventwiseboothlist.Add(model);
                    }
                    else if (eventList.EventMasterId == 6)
                    {
                        bool isQueue = electioInfoRecord.VoterInQueue != null;
                        EventWiseBooth model = new EventWiseBooth()
                        {
                            StateMasterId = soTotalBooths.StateMasterId,
                            DistrictMasterId = soTotalBooths.DistrictMasterId,
                            EventMasterId = eventList.EventMasterId,
                            EventName = eventList.EventName,
                            AssemblyMasterId = soTotalBooths.AssemblyMasterId,
                            BoothMasterId = Convert.ToInt32(soTotalBooths.BoothMasterId),
                            BoothName = soTotalBooths.BoothName + "-" + boothName,
                            BoothCode = soTotalBooths.BoothCode_No,
                            UpdateStatus = isQueue
                        };
                        eventwiseboothlist.Add(model);
                    }
                    else if (eventList.EventMasterId == 7)
                    {
                        bool isQueue = electioInfoRecord.VoterInQueue != null;
                        EventWiseBooth model = new EventWiseBooth()
                        {
                            StateMasterId = soTotalBooths.StateMasterId,
                            DistrictMasterId = soTotalBooths.DistrictMasterId,
                            EventMasterId = eventList.EventMasterId,
                            EventName = eventList.EventName,
                            AssemblyMasterId = soTotalBooths.AssemblyMasterId,
                            BoothMasterId = Convert.ToInt32(soTotalBooths.BoothMasterId),
                            BoothName = soTotalBooths.BoothName + "-" + boothName,
                            BoothCode = soTotalBooths.BoothCode_No,
                            UpdateStatus = isQueue
                        };
                        eventwiseboothlist.Add(model);
                    }
                    else if (eventList.EventMasterId == 8)
                    {
                        //bool isFinalVotes = electioInfoRecord.FinalTVote != null;
                        EventWiseBooth model = new EventWiseBooth()
                        {
                            StateMasterId = soTotalBooths.StateMasterId,
                            DistrictMasterId = soTotalBooths.DistrictMasterId,
                            EventMasterId = eventList.EventMasterId,
                            EventName = eventList.EventName,
                            AssemblyMasterId = soTotalBooths.AssemblyMasterId,
                            BoothMasterId = Convert.ToInt32(soTotalBooths.BoothMasterId),
                            BoothName = soTotalBooths.BoothName + "-" + boothName,
                            BoothCode = soTotalBooths.BoothCode_No,
                            //UpdateStatus = isFinalVotes
                            UpdateStatus = electioInfoRecord.FinalTVoteStatus ?? false
                        };
                        eventwiseboothlist.Add(model);
                    }
                    else if (eventList.EventMasterId == 9)
                    {
                        EventWiseBooth model = new EventWiseBooth()
                        {
                            StateMasterId = soTotalBooths.StateMasterId,
                            DistrictMasterId = soTotalBooths.DistrictMasterId,
                            EventMasterId = eventList.EventMasterId,
                            EventName = eventList.EventName,
                            AssemblyMasterId = soTotalBooths.AssemblyMasterId,
                            BoothMasterId = Convert.ToInt32(soTotalBooths.BoothMasterId),
                            BoothName = soTotalBooths.BoothName + "-" + boothName,
                            BoothCode = soTotalBooths.BoothCode_No,
                            UpdateStatus = electioInfoRecord.IsPollEnded ?? false
                        };
                        eventwiseboothlist.Add(model);
                    }
                    else if (eventList.EventMasterId == 10)
                    {
                        EventWiseBooth model = new EventWiseBooth()
                        {
                            StateMasterId = soTotalBooths.StateMasterId,
                            DistrictMasterId = soTotalBooths.DistrictMasterId,
                            EventMasterId = eventList.EventMasterId,
                            EventName = eventList.EventName,
                            AssemblyMasterId = soTotalBooths.AssemblyMasterId,
                            BoothMasterId = Convert.ToInt32(soTotalBooths.BoothMasterId),
                            BoothName = soTotalBooths.BoothName + "-" + boothName,
                            BoothCode = soTotalBooths.BoothCode_No,
                            UpdateStatus = electioInfoRecord.IsMCESwitchOff ?? false
                        };
                        eventwiseboothlist.Add(model);
                    }
                    else if (eventList.EventMasterId == 11)
                    {
                        EventWiseBooth model = new EventWiseBooth()
                        {
                            StateMasterId = soTotalBooths.StateMasterId,
                            DistrictMasterId = soTotalBooths.DistrictMasterId,
                            EventMasterId = eventList.EventMasterId,
                            EventName = eventList.EventName,
                            AssemblyMasterId = soTotalBooths.AssemblyMasterId,
                            BoothMasterId = Convert.ToInt32(soTotalBooths.BoothMasterId),
                            BoothName = soTotalBooths.BoothName + "-" + boothName,
                            BoothCode = soTotalBooths.BoothCode_No,
                            UpdateStatus = electioInfoRecord.IsPartyDeparted ?? false
                        };
                        eventwiseboothlist.Add(model);
                    }
                    else if (eventList.EventMasterId == 12)
                    {
                        EventWiseBooth model = new EventWiseBooth()
                        {
                            StateMasterId = soTotalBooths.StateMasterId,
                            DistrictMasterId = soTotalBooths.DistrictMasterId,
                            EventMasterId = eventList.EventMasterId,
                            EventName = eventList.EventName,
                            AssemblyMasterId = soTotalBooths.AssemblyMasterId,
                            BoothMasterId = Convert.ToInt32(soTotalBooths.BoothMasterId),
                            BoothName = soTotalBooths.BoothName + "-" + boothName,
                            BoothCode = soTotalBooths.BoothCode_No,
                            UpdateStatus = electioInfoRecord.IsPartyReachedCollectionCenter ?? false
                        };
                        eventwiseboothlist.Add(model);
                    }
                    else if (eventList.EventMasterId == 13)
                    {
                        EventWiseBooth model = new EventWiseBooth()
                        {
                            StateMasterId = soTotalBooths.StateMasterId,
                            DistrictMasterId = soTotalBooths.DistrictMasterId,
                            EventMasterId = eventList.EventMasterId,
                            EventName = eventList.EventName,
                            AssemblyMasterId = soTotalBooths.AssemblyMasterId,
                            BoothMasterId = Convert.ToInt32(soTotalBooths.BoothMasterId),
                            BoothName = soTotalBooths.BoothName + "-" + boothName,
                            BoothCode = soTotalBooths.BoothCode_No,
                            UpdateStatus = electioInfoRecord.IsEVMDeposited ?? false
                        };
                        eventwiseboothlist.Add(model);
                    }

                }

            }
            else

            {
                foreach (var eventList in event_lits)
                {
                    string boothName = "";
                    if (soTotalBooths.BoothNoAuxy != "0" && soTotalBooths.BoothNoAuxy != "")
                    {
                        boothName = soTotalBooths.BoothNoAuxy;
                    }
                    if (eventList.EventMasterId == 1)
                    {
                        EventWiseBooth model = new EventWiseBooth()
                        {
                            StateMasterId = soTotalBooths.StateMasterId,
                            DistrictMasterId = soTotalBooths.DistrictMasterId,
                            EventMasterId = eventList.EventMasterId,
                            EventName = eventList.EventName,
                            AssemblyMasterId = soTotalBooths.AssemblyMasterId,
                            BoothMasterId = Convert.ToInt32(soTotalBooths.BoothMasterId),
                            BoothName = soTotalBooths.BoothName + "-" + boothName,
                            BoothCode = soTotalBooths.BoothCode_No,
                            UpdateStatus = false
                        };

                        eventwiseboothlist.Add(model);
                    }
                    else if (eventList.EventMasterId == 2)
                    {
                        EventWiseBooth model = new EventWiseBooth()
                        {
                            StateMasterId = soTotalBooths.StateMasterId,
                            DistrictMasterId = soTotalBooths.DistrictMasterId,
                            EventMasterId = eventList.EventMasterId,
                            EventName = eventList.EventName,
                            AssemblyMasterId = soTotalBooths.AssemblyMasterId,
                            BoothMasterId = Convert.ToInt32(soTotalBooths.BoothMasterId),
                            BoothName = soTotalBooths.BoothName + "-" + boothName,
                            BoothCode = soTotalBooths.BoothCode_No,
                            UpdateStatus = false
                        };

                        eventwiseboothlist.Add(model);
                    }
                    else if (eventList.EventMasterId == 3)
                    {
                        EventWiseBooth model = new EventWiseBooth()
                        {
                            StateMasterId = soTotalBooths.StateMasterId,
                            DistrictMasterId = soTotalBooths.DistrictMasterId,
                            EventMasterId = eventList.EventMasterId,
                            EventName = eventList.EventName,
                            AssemblyMasterId = soTotalBooths.AssemblyMasterId,
                            BoothMasterId = Convert.ToInt32(soTotalBooths.BoothMasterId),
                            BoothName = soTotalBooths.BoothName + "-" + boothName,
                            BoothCode = soTotalBooths.BoothCode_No,
                            UpdateStatus = false
                        };
                        eventwiseboothlist.Add(model);
                    }
                    else if (eventList.EventMasterId == 4)
                    {

                        EventWiseBooth model = new EventWiseBooth()
                        {
                            StateMasterId = soTotalBooths.StateMasterId,
                            DistrictMasterId = soTotalBooths.DistrictMasterId,
                            EventMasterId = eventList.EventMasterId,
                            EventName = eventList.EventName,
                            AssemblyMasterId = soTotalBooths.AssemblyMasterId,
                            BoothMasterId = Convert.ToInt32(soTotalBooths.BoothMasterId),
                            BoothName = soTotalBooths.BoothName + "-" + boothName,
                            BoothCode = soTotalBooths.BoothCode_No,
                            UpdateStatus = false
                        };
                        eventwiseboothlist.Add(model);
                    }
                    else if (eventList.EventMasterId == 5)
                    {
                        EventWiseBooth model = new EventWiseBooth()
                        {
                            StateMasterId = soTotalBooths.StateMasterId,
                            DistrictMasterId = soTotalBooths.DistrictMasterId,
                            EventMasterId = eventList.EventMasterId,
                            EventName = eventList.EventName,
                            AssemblyMasterId = soTotalBooths.AssemblyMasterId,
                            BoothMasterId = Convert.ToInt32(soTotalBooths.BoothMasterId),
                            BoothName = soTotalBooths.BoothName + "-" + boothName,
                            BoothCode = soTotalBooths.BoothCode_No,
                            UpdateStatus = false
                        };
                        eventwiseboothlist.Add(model);
                    }
                    else if (eventList.EventMasterId == 6)
                    {
                        EventWiseBooth model = new EventWiseBooth()
                        {
                            StateMasterId = soTotalBooths.StateMasterId,
                            DistrictMasterId = soTotalBooths.DistrictMasterId,
                            EventMasterId = eventList.EventMasterId,
                            EventName = eventList.EventName,
                            AssemblyMasterId = soTotalBooths.AssemblyMasterId,
                            BoothMasterId = Convert.ToInt32(soTotalBooths.BoothMasterId),
                            BoothName = soTotalBooths.BoothName + "-" + boothName,
                            BoothCode = soTotalBooths.BoothCode_No,
                            UpdateStatus = false
                        };
                        eventwiseboothlist.Add(model);
                    }
                    else if (eventList.EventMasterId == 7)
                    {

                        EventWiseBooth model = new EventWiseBooth()
                        {
                            StateMasterId = soTotalBooths.StateMasterId,
                            DistrictMasterId = soTotalBooths.DistrictMasterId,
                            EventMasterId = eventList.EventMasterId,
                            EventName = eventList.EventName,
                            AssemblyMasterId = soTotalBooths.AssemblyMasterId,
                            BoothMasterId = Convert.ToInt32(soTotalBooths.BoothMasterId),
                            BoothName = soTotalBooths.BoothName + "-" + boothName,
                            BoothCode = soTotalBooths.BoothCode_No,
                            UpdateStatus = false
                        };
                        eventwiseboothlist.Add(model);
                    }
                    else if (eventList.EventMasterId == 8)
                    {

                        EventWiseBooth model = new EventWiseBooth()
                        {
                            StateMasterId = soTotalBooths.StateMasterId,
                            DistrictMasterId = soTotalBooths.DistrictMasterId,
                            EventMasterId = eventList.EventMasterId,
                            EventName = eventList.EventName,
                            AssemblyMasterId = soTotalBooths.AssemblyMasterId,
                            BoothMasterId = Convert.ToInt32(soTotalBooths.BoothMasterId),
                            BoothName = soTotalBooths.BoothName + "-" + boothName,
                            BoothCode = soTotalBooths.BoothCode_No,
                            UpdateStatus = false
                        };
                        eventwiseboothlist.Add(model);
                    }
                    else if (eventList.EventMasterId == 9)
                    {
                        EventWiseBooth model = new EventWiseBooth()
                        {
                            StateMasterId = soTotalBooths.StateMasterId,
                            DistrictMasterId = soTotalBooths.DistrictMasterId,
                            EventMasterId = eventList.EventMasterId,
                            EventName = eventList.EventName,
                            AssemblyMasterId = soTotalBooths.AssemblyMasterId,
                            BoothMasterId = Convert.ToInt32(soTotalBooths.BoothMasterId),
                            BoothName = soTotalBooths.BoothName + "-" + boothName,
                            BoothCode = soTotalBooths.BoothCode_No,
                            UpdateStatus = false
                        };
                        eventwiseboothlist.Add(model);
                    }
                    else if (eventList.EventMasterId == 10)
                    {
                        EventWiseBooth model = new EventWiseBooth()
                        {
                            StateMasterId = soTotalBooths.StateMasterId,
                            DistrictMasterId = soTotalBooths.DistrictMasterId,
                            EventMasterId = eventList.EventMasterId,
                            EventName = eventList.EventName,
                            AssemblyMasterId = soTotalBooths.AssemblyMasterId,
                            BoothMasterId = Convert.ToInt32(soTotalBooths.BoothMasterId),
                            BoothName = soTotalBooths.BoothName + "-" + boothName,
                            BoothCode = soTotalBooths.BoothCode_No,
                            UpdateStatus = false
                        };
                        eventwiseboothlist.Add(model);
                    }
                    else if (eventList.EventMasterId == 11)
                    {
                        EventWiseBooth model = new EventWiseBooth()
                        {
                            StateMasterId = soTotalBooths.StateMasterId,
                            DistrictMasterId = soTotalBooths.DistrictMasterId,
                            EventMasterId = eventList.EventMasterId,
                            EventName = eventList.EventName,
                            AssemblyMasterId = soTotalBooths.AssemblyMasterId,
                            BoothMasterId = Convert.ToInt32(soTotalBooths.BoothMasterId),
                            BoothName = soTotalBooths.BoothName + "-" + boothName,
                            BoothCode = soTotalBooths.BoothCode_No,
                            UpdateStatus = false
                        };
                        eventwiseboothlist.Add(model);
                    }
                    else if (eventList.EventMasterId == 12)
                    {
                        EventWiseBooth model = new EventWiseBooth()
                        {
                            StateMasterId = soTotalBooths.StateMasterId,
                            DistrictMasterId = soTotalBooths.DistrictMasterId,
                            EventMasterId = eventList.EventMasterId,
                            EventName = eventList.EventName,
                            AssemblyMasterId = soTotalBooths.AssemblyMasterId,
                            BoothMasterId = Convert.ToInt32(soTotalBooths.BoothMasterId),
                            BoothName = soTotalBooths.BoothName + "-" + boothName,
                            BoothCode = soTotalBooths.BoothCode_No,
                            UpdateStatus = false
                        };
                        eventwiseboothlist.Add(model);
                    }
                    else if (eventList.EventMasterId == 13)
                    {
                        EventWiseBooth model = new EventWiseBooth()
                        {
                            StateMasterId = soTotalBooths.StateMasterId,
                            DistrictMasterId = soTotalBooths.DistrictMasterId,
                            EventMasterId = eventList.EventMasterId,
                            EventName = eventList.EventName,
                            AssemblyMasterId = soTotalBooths.AssemblyMasterId,
                            BoothMasterId = Convert.ToInt32(soTotalBooths.BoothMasterId),
                            BoothName = soTotalBooths.BoothName + "-" + boothName,
                            BoothCode = soTotalBooths.BoothCode_No,
                            UpdateStatus = false
                        };
                        eventwiseboothlist.Add(model);
                    }

                }

            }



            return eventwiseboothlist;
        }
        private bool GetUpdateStatus(string eventId, ElectionInfoMaster electioInfoRecord)
        {
            switch (eventId)
            {
                case "1":
                    return electioInfoRecord.IsPartyDispatched ?? false;
                case "2":
                    return electioInfoRecord.IsPartyReached ?? false;
                case "3":
                    return electioInfoRecord.IsSetupOfPolling ?? false;
                case "4":
                    return electioInfoRecord.IsMockPollDone ?? false;
                case "5":
                    return electioInfoRecord.IsPollStarted ?? false;
                case "6":
                    //  return electioInfoRecord.IsVoterTurnOut ?? false;
                    //here logic need when current slot aviable and his entry done is that make color green
                    //and  other warning color...
                    return electioInfoRecord.VoterInQueue != null;

                case "7":
                    return electioInfoRecord.VoterInQueue != null;
                case "8":
                    //return electioInfoRecord.FinalTVote != null && electioInfoRecord.FinalTVote > 0;
                    //return electioInfoRecord.IsPollEnded != null;
                    return electioInfoRecord.FinalTVoteStatus ?? false;

                case "9":
                    return electioInfoRecord.IsPollEnded ?? false;
                case "10":
                    return electioInfoRecord.IsMCESwitchOff ?? false;
                case "11":
                    return electioInfoRecord.IsPartyDeparted ?? false;
                case "12":
                    return electioInfoRecord.IsPartyReachedCollectionCenter ?? false;
                case "13":
                    return electioInfoRecord.IsEVMDeposited ?? false;
                default:
                    return false;
            }
        }
        public async Task<List<TurnOutBoothListStatus>> GetBoothInfoinPollDetail(string soId, string eventid)
        {
            string color = ""; int SortColor = 0; bool UpdateStatus = false;
            var soTotalBooths = await _context.BoothMaster.Where(p => p.AssignedTo == soId).ToListAsync();
            List<TurnOutBoothListStatus> list = new List<TurnOutBoothListStatus>();

            foreach (var boothRecord in soTotalBooths)
            {
                string boothName = "";
                var electioInfoRecord = _context.ElectionInfoMaster.FirstOrDefault(d =>
                    d.BoothMasterId == boothRecord.BoothMasterId);

                if (electioInfoRecord is not null)
                {// mut check if poll ended then no color

                    //if (electioInfoRecord.VoterInQueue == null)
                    if (electioInfoRecord.VoterInQueue == null && electioInfoRecord.IsQueueUndo != true)
                    {
                        var polldetail_byUser = await _context.PollDetails.Where(p => p.BoothMasterId == electioInfoRecord.BoothMasterId).OrderByDescending(p => p.VotesPolledRecivedTime).FirstOrDefaultAsync();
                        var slotsListofTurnOut = await _context.SlotManagementMaster.Where(p => p.StateMasterId == electioInfoRecord.StateMasterId && p.EventMasterId == Convert.ToInt32(eventid)).OrderBy(p => p.SlotManagementId).ToListAsync();
                        var SlotId = await GetSlot(slotsListofTurnOut); // get currentSlotId
                        if (SlotId != null && SlotId != 0) // means current lying in slot added in db
                        {

                            var checkSlotEnterdCurrent = await _context.PollDetails.Where(p => p.BoothMasterId == electioInfoRecord.BoothMasterId && p.SlotManagementId == SlotId).FirstOrDefaultAsync();
                            if (checkSlotEnterdCurrent == null)
                            {
                                // means entry not done of that slot make color red
                                color = ColorEnum.Red.ToString();
                                SortColor = 1;
                                UpdateStatus = false;
                            }
                            else
                            {
                                color = ColorEnum.Green.ToString();
                                SortColor = 2;
                                UpdateStatus = false;
                            }

                        }
                        else
                        {
                            color = ColorEnum.White.ToString();
                            SortColor = 3;
                            //either slot over or slot not available rigt now, wait for next slot
                            /* int getNextSlot = GetNextSlot(slotsListofTurnOut);
                            if (getNextSlot != null && getNextSlot != 0)
                            {
                                // means next slot exists get slot id find entry done or not
                                var checkSlotEnterdnext = _context.PollDetails.Where(p => p.BoothMasterId == electioInfoRecord.BoothMasterId && p.StateMasterId == electioInfoRecord.StateMasterId && p.DistrictMasterId == electioInfoRecord.DistrictMasterId && p.SlotManagementId == getNextSlot).FirstOrDefault();
                                if (checkSlotEnterdnext != null)
                                {
                                    color = ColorEnum.Green.ToString();
                                }
                                else
                                {
                                    color = ColorEnum.Red.ToString();
                                }
                            }
                            else
                            {
                                // there is no more slot
                                color = ColorEnum.Yellow.ToString();
                            }*/
                        }


                    }
                    else
                    {
                        color = ColorEnum.Blue.ToString();
                        SortColor = 4;
                        UpdateStatus = true;


                    }
                }

                else
                {

                    color = ColorEnum.Red.ToString();
                    SortColor = 1;
                    UpdateStatus = false;
                }


                if (boothRecord.BoothNoAuxy == "0" || boothRecord.BoothNoAuxy == "")
                {

                    boothName = boothRecord.BoothName;

                }
                else
                {
                    boothName = boothRecord.BoothName + "-" + boothRecord.BoothNoAuxy;
                }
                TurnOutBoothListStatus eventWiseBooth = new TurnOutBoothListStatus()
                {
                    StateMasterId = boothRecord.StateMasterId,
                    DistrictMasterId = boothRecord.DistrictMasterId,
                    AssemblyMasterId = boothRecord.AssemblyMasterId,
                    BoothMasterId = boothRecord.BoothMasterId,
                    BoothName = boothName,
                    BoothCode = boothRecord.BoothCode_No,
                    EventMasterId = Convert.ToInt32(eventid),
                    EventName = "TurnOut",
                    Color = color,
                    SortColor = SortColor,
                    UpdateStatus = UpdateStatus
                };
                list.Add(eventWiseBooth);
            }



            return list;
        }
        #endregion

        #region PCMaster

        public async Task<List<ParliamentConstituencyMaster>> GetPCList(string stateMasterId)
        {
            var isStateActive = _context.StateMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId)).FirstOrDefault();
            if (isStateActive.StateStatus)
            {

                var pcData = await _context.ParliamentConstituencyMaster
                            .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId))
                            .OrderBy(d => d.PCMasterId)
                            .Select(d => new ParliamentConstituencyMaster
                            {
                                PCMasterId = d.PCMasterId,
                                StateMasterId = d.StateMasterId,
                                PcCodeNo = d.PcCodeNo,
                                PcName = d.PcName,
                                SecondLanguage = d.SecondLanguage,
                                PcType = d.PcType,
                                PcStatus = d.PcStatus
                            })
                            .ToListAsync();

                return pcData;
            }
            else
            {
                return null;
            }
        }
        public async Task<ParliamentConstituencyMaster> GetPCById(string pcMasterId)
        {
            var isPcActive = _context.ParliamentConstituencyMaster.Where(d => d.PCMasterId == Convert.ToInt32(pcMasterId)).FirstOrDefault();
            if (isPcActive is not null)
            {
                return isPcActive;
                //if (isPcActive.PcStatus)
                //{

                //    return isPcActive;
                //}
                //else
                //{
                //    return null;
                //}
            }
            else
            {
                return null;
            }
        }

        #endregion


        #region EventActivity

        public async Task<Response> EventActivity(ElectionInfoMaster electionInfoMaster)
        {
            try
            {
                var electionRecord = await _context.ElectionInfoMaster.Where(d => d.StateMasterId == electionInfoMaster.StateMasterId &&
                         d.DistrictMasterId == electionInfoMaster.DistrictMasterId && d.AssemblyMasterId == electionInfoMaster.AssemblyMasterId &&
                         d.BoothMasterId == electionInfoMaster.BoothMasterId).FirstOrDefaultAsync();


                if (electionRecord != null)
                {

                    _context.ElectionInfoMaster.Update(electionInfoMaster);
                    _context.SaveChanges();
                    return new Response { Status = RequestStatusEnum.OK, Message = "Status Updated Successfully" };
                }
                else
                {
                    var boothExists = await _context.BoothMaster.AnyAsync(p => p.BoothMasterId == electionInfoMaster.BoothMasterId && p.StateMasterId == electionInfoMaster.StateMasterId && p.DistrictMasterId == electionInfoMaster.DistrictMasterId && p.BoothMasterId == electionInfoMaster.BoothMasterId && p.IsAssigned == true);

                    if (boothExists == true)
                    {

                        if (electionInfoMaster.EventMasterId == 1)
                        {
                            electionInfoMaster.PartyDispatchedLastUpdate = BharatDateTime();
                            _context.ElectionInfoMaster.Add(electionInfoMaster);
                            _context.SaveChanges();
                            return new Response { Status = RequestStatusEnum.OK, Message = "Status Added Successfully" };
                        }
                        else
                        {
                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Party Not Dispatched yet" };
                        }

                    }
                    else
                    {
                        return new Response { Status = RequestStatusEnum.NotFound, Message = "Record Not Found, Also Recheck Booth Assigned or not" };
                    }
                }
            }
            catch (Exception ex)
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = ex.Message };
            }


        }
        public async Task<ElectionInfoMaster> EventUpdationStatus(ElectionInfoMaster electionInfoMaster)
        {
            var electionInfoRecord = _context.ElectionInfoMaster.Where(d => d.StateMasterId == electionInfoMaster.StateMasterId
            && d.DistrictMasterId == electionInfoMaster.DistrictMasterId &&
            d.AssemblyMasterId == electionInfoMaster.AssemblyMasterId
            && d.BoothMasterId == electionInfoMaster.BoothMasterId
            ).FirstOrDefault();
            return electionInfoRecord;
        }

        public async Task<VoterTurnOutPolledDetailViewModel> GetLastUpdatedPollDetail(string boothMasterId, int eventmasterid)
        {
            VoterTurnOutPolledDetailViewModel model;
            try
            {
                var boothExists = await _context.BoothMaster.Where(p => p.BoothMasterId == Convert.ToInt32(boothMasterId)).FirstOrDefaultAsync();
                var polldetail = await _context.PollDetails.Where(p => p.BoothMasterId == Convert.ToInt32(boothMasterId) && p.StateMasterId == boothExists.StateMasterId && p.DistrictMasterId == boothExists.DistrictMasterId).OrderByDescending(p => p.VotesPolledRecivedTime).FirstOrDefaultAsync();
                var slotsList = await _context.SlotManagementMaster.Where(p => p.StateMasterId == boothExists.StateMasterId && p.EventMasterId == eventmasterid).OrderBy(p => p.SlotManagementId).ToListAsync();
                var isGenderCptureRequired = await _context.StateMaster.Where(p => p.StateMasterId == boothExists.StateMasterId).Select(p => p.IsGenderCapturedinVoterTurnOut).FirstOrDefaultAsync();

                if (boothExists is not null)
                {
                    var electionInfoRecord = await _context.ElectionInfoMaster.Where(p => p.StateMasterId == boothExists.StateMasterId && p.DistrictMasterId == boothExists.DistrictMasterId && p.AssemblyMasterId == boothExists.AssemblyMasterId && p.BoothMasterId == Convert.ToInt32(boothMasterId)).FirstOrDefaultAsync();
                    if (electionInfoRecord is not null)
                    {
                        if (electionInfoRecord.IsPollStarted == true)
                        {
                            if (slotsList is not null) // any1 slot is there in poll table 
                            {
                                int SlotRecordMasterId = await GetSlot(slotsList);
                                if (SlotRecordMasterId > 0)
                                {
                                    var SlotRecord = await _context.SlotManagementMaster.Where(p => p.SlotManagementId == SlotRecordMasterId).FirstOrDefaultAsync();
                                    if (polldetail is not null)
                                    {
                                        // check whether current time slot already entered or not
                                        var slotlast = slotsList.OrderByDescending(p => p.SlotManagementId).FirstOrDefault();
                                        bool lastslotexceededtime = await TimeExceedLastSlot(slotlast);

                                        if (lastslotexceededtime == false)
                                        {
                                            bool VoterTurnOutAlreadyExistsinSlot = await IsSlotAlreadyEntered(SlotRecord, polldetail.VotesPolledRecivedTime);

                                            if (VoterTurnOutAlreadyExistsinSlot == false)
                                            {
                                                model = new VoterTurnOutPolledDetailViewModel()
                                                {

                                                    BoothMasterId = boothExists.BoothMasterId,
                                                    TotalVoters = boothExists.TotalVoters,
                                                    VotesPolled = polldetail.VotesPolled,
                                                    VotesPolledRecivedTime = polldetail.VotesPolledRecivedTime,
                                                    StartTime = SlotRecord.StartTime,
                                                    EndTime = SlotRecord.EndTime,
                                                    LockTime = SlotRecord.LockTime,
                                                    VoteEnabled = true,
                                                    IsLastSlot = SlotRecord.IsLastSlot,
                                                    Message = "Slot is Available",

                                                };
                                                if (isGenderCptureRequired == true)
                                                {
                                                    model.IsGenderCapturedReqinVT = true;
                                                    model.Male = polldetail.Male.ToString();
                                                    model.Female = polldetail.Female.ToString();
                                                    model.Transgender = polldetail.Transgender.ToString();
                                                    model.TotalAvailableMale = boothExists.Male.ToString();
                                                    model.TotalAvailableFemale = boothExists.Female.ToString();
                                                    model.TotalAvailableTransgender = boothExists.Transgender.ToString();
                                                }
                                                else
                                                {
                                                    model.IsGenderCapturedReqinVT = false;
                                                }

                                            }
                                            else
                                            {
                                                string msg = "";
                                                if (SlotRecord.IsLastSlot == true)
                                                {
                                                    msg = "Voter Turn Out Already Entered For Slot. Please Proceed For Queue.";
                                                }
                                                else
                                                {

                                                    // find next slot and print
                                                    int nextSlotId = await GetNextSlot(slotsList);
                                                    if (nextSlotId != 0)
                                                    {

                                                        var nextSlotRecord = await _context.SlotManagementMaster.Where(p => p.SlotManagementId == nextSlotId).FirstOrDefaultAsync();
                                                        model = new VoterTurnOutPolledDetailViewModel()
                                                        {
                                                            BoothMasterId = boothExists.BoothMasterId,
                                                            TotalVoters = boothExists.TotalVoters,
                                                            VotesPolled = 0,
                                                            VoteEnabled = false,
                                                            Message = "Voter Turn Out Already Entered For Slot. Please enter values in Next Slot :" + nextSlotRecord.EndTime + " " + "T0" + " " + nextSlotRecord.LockTime

                                                        };

                                                        if (isGenderCptureRequired == true)
                                                        {
                                                            model.IsGenderCapturedReqinVT = true;
                                                            model.Male = polldetail.Male.ToString();
                                                            model.Female = polldetail.Female.ToString();
                                                            model.Transgender = polldetail.Transgender.ToString();
                                                            model.TotalAvailableMale = boothExists.Male.ToString();
                                                            model.TotalAvailableFemale = boothExists.Female.ToString();
                                                            model.TotalAvailableTransgender = boothExists.Transgender.ToString();
                                                        }
                                                        else
                                                        {
                                                            model.IsGenderCapturedReqinVT = false;
                                                        }

                                                    }

                                                }

                                                model = new VoterTurnOutPolledDetailViewModel()
                                                {
                                                    BoothMasterId = boothExists.BoothMasterId,
                                                    TotalVoters = boothExists.TotalVoters,
                                                    VotesPolled = polldetail.VotesPolled,
                                                    VotesPolledRecivedTime = polldetail.VotesPolledRecivedTime,
                                                    StartTime = SlotRecord.StartTime,
                                                    EndTime = SlotRecord.EndTime,
                                                    LockTime = SlotRecord.LockTime,
                                                    VoteEnabled = false, // but freeze it if already entered for thi sslot
                                                    IsLastSlot = SlotRecord.IsLastSlot,
                                                    Message = msg
                                                };
                                                if (isGenderCptureRequired == true)
                                                {
                                                    model.IsGenderCapturedReqinVT = true;
                                                    model.Male = polldetail.Male.ToString();
                                                    model.Female = polldetail.Female.ToString();
                                                    model.Transgender = polldetail.Transgender.ToString();
                                                    model.TotalAvailableMale = boothExists.Male.ToString();
                                                    model.TotalAvailableFemale = boothExists.Female.ToString();
                                                    model.TotalAvailableTransgender = boothExists.Transgender.ToString();
                                                }
                                                else
                                                {
                                                    model.IsGenderCapturedReqinVT = false;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            model = new VoterTurnOutPolledDetailViewModel()
                                            {
                                                BoothMasterId = boothExists.BoothMasterId,
                                                TotalVoters = boothExists.TotalVoters,
                                                VotesPolled = polldetail.VotesPolled,
                                                VotesPolledRecivedTime = polldetail.VotesPolledRecivedTime,
                                                StartTime = SlotRecord.StartTime,
                                                EndTime = SlotRecord.EndTime,
                                                LockTime = SlotRecord.LockTime,
                                                IsLastSlot = SlotRecord.IsLastSlot,
                                                VoteEnabled = false,
                                                Message = "Voter Turn Out Closed, Kindly Proceed for Voter in Queue"
                                            };

                                            if (isGenderCptureRequired == true)
                                            {

                                                model.IsGenderCapturedReqinVT = true;
                                                model.Male = polldetail.Male.ToString();
                                                model.Female = polldetail.Female.ToString();
                                                model.Transgender = polldetail.Transgender.ToString();
                                                model.TotalAvailableMale = boothExists.Male.ToString();
                                                model.TotalAvailableFemale = boothExists.Female.ToString();
                                                model.TotalAvailableTransgender = boothExists.Transgender.ToString();

                                            }
                                            else
                                            {
                                                model.IsGenderCapturedReqinVT = false;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        model = new VoterTurnOutPolledDetailViewModel()
                                        {
                                            BoothMasterId = boothExists.BoothMasterId,
                                            TotalVoters = boothExists.TotalVoters,
                                            VotesPolled = 0,
                                            VotesPolledRecivedTime = null,
                                            StartTime = SlotRecord.StartTime,
                                            EndTime = SlotRecord.EndTime,
                                            LockTime = SlotRecord.LockTime,
                                            IsLastSlot = SlotRecord.IsLastSlot,
                                            VoteEnabled = true,
                                            Message = "Slot is Available",
                                        };
                                        if (isGenderCptureRequired == true)
                                        {
                                            model.IsGenderCapturedReqinVT = true;
                                            model.Male = null;
                                            model.Female = null;
                                            model.Transgender = null;
                                            model.TotalAvailableMale = boothExists.Male.ToString();
                                            model.TotalAvailableFemale = boothExists.Female.ToString();
                                            model.TotalAvailableTransgender = boothExists.Transgender.ToString();
                                        }
                                        else
                                        {
                                            model.IsGenderCapturedReqinVT = false;
                                        }
                                    }

                                }
                                else
                                {
                                    var getLastSlotRecord = await _context.SlotManagementMaster.Where(p => p.IsLastSlot == true && p.EventMasterId == 6).FirstOrDefaultAsync();
                                    bool lastslotexceededtime = await TimeExceedLastSlot(getLastSlotRecord);
                                    if (lastslotexceededtime == true)
                                    {
                                        if (polldetail != null)
                                        {
                                            model = new VoterTurnOutPolledDetailViewModel()
                                            {
                                                BoothMasterId = boothExists.BoothMasterId,
                                                TotalVoters = boothExists.TotalVoters,
                                                VotesPolled = polldetail.VotesPolled,
                                                VotesPolledRecivedTime = polldetail.VotesPolledRecivedTime,
                                                VoteEnabled = false,
                                                Message = "Voter Turn Out Closed, Kindly Proceed for Voter in Queue"

                                            };

                                            if (isGenderCptureRequired == true)
                                            {
                                                model.IsGenderCapturedReqinVT = true;
                                                model.Male = polldetail.Male.ToString();
                                                model.Female = polldetail.Female.ToString();
                                                model.Transgender = polldetail.Transgender.ToString();
                                                model.TotalAvailableMale = boothExists.Male.ToString();
                                                model.TotalAvailableFemale = boothExists.Female.ToString();
                                                model.TotalAvailableTransgender = boothExists.Transgender.ToString();
                                            }
                                            else
                                            {
                                                model.IsGenderCapturedReqinVT = false;
                                            }
                                        }
                                        else
                                        {
                                            model = new VoterTurnOutPolledDetailViewModel()
                                            {
                                                BoothMasterId = boothExists.BoothMasterId,
                                                TotalVoters = boothExists.TotalVoters,
                                                VotesPolled = 0,
                                                VotesPolledRecivedTime = null,
                                                VoteEnabled = false,
                                                Message = "Voter Turn Out Closed, You have entered no values in the Slots. Kindly Proceed for Queue."
                                            };
                                            if (isGenderCptureRequired == true)
                                            {
                                                model.IsGenderCapturedReqinVT = true;
                                                model.TotalAvailableMale = boothExists.Male.ToString();
                                                model.TotalAvailableFemale = boothExists.Female.ToString();
                                                model.TotalAvailableTransgender = boothExists.Transgender.ToString();
                                            }
                                            else
                                            {
                                                model.IsGenderCapturedReqinVT = false;
                                            }
                                        }
                                    }
                                    else

                                    {
                                        int nextSlotId = await GetNextSlot(slotsList);
                                        if (nextSlotId != 0)
                                        {

                                            var nextSlotRecord = await _context.SlotManagementMaster.Where(p => p.SlotManagementId == nextSlotId).FirstOrDefaultAsync();
                                            if (polldetail != null)
                                            {
                                                model = new VoterTurnOutPolledDetailViewModel()
                                                {
                                                    BoothMasterId = boothExists.BoothMasterId,
                                                    TotalVoters = boothExists.TotalVoters,
                                                    VotesPolled = polldetail.VotesPolled,
                                                    VotesPolledRecivedTime = polldetail.VotesPolledRecivedTime,
                                                    VoteEnabled = false,
                                                    Message = "Slot not available. Next Slot Duration :" + nextSlotRecord.EndTime + " " + "T0" + " " + nextSlotRecord.LockTime

                                                };
                                                if (isGenderCptureRequired == true)
                                                {
                                                    model.IsGenderCapturedReqinVT = true;
                                                    model.Male = polldetail.Male.ToString();
                                                    model.Female = polldetail.Female.ToString();
                                                    model.Transgender = polldetail.Transgender.ToString();
                                                    model.TotalAvailableMale = boothExists.Male.ToString();
                                                    model.TotalAvailableFemale = boothExists.Female.ToString();
                                                    model.TotalAvailableTransgender = boothExists.Transgender.ToString();
                                                }
                                                else
                                                {
                                                    model.IsGenderCapturedReqinVT = false;
                                                }
                                            }
                                            else
                                            {
                                                model = new VoterTurnOutPolledDetailViewModel()
                                                {
                                                    BoothMasterId = boothExists.BoothMasterId,
                                                    TotalVoters = boothExists.TotalVoters,
                                                    VotesPolled = 0,
                                                    VoteEnabled = false,
                                                    Message = "Slot not available. Next Slot Duration :" + nextSlotRecord.EndTime + " " + "T0" + " " + nextSlotRecord.LockTime

                                                };
                                                if (isGenderCptureRequired == true)
                                                {
                                                    model.IsGenderCapturedReqinVT = true;
                                                    //model.Male = polldetail.Male.ToString();
                                                    //model.Female = polldetail.Female.ToString();
                                                    //model.Transgender = polldetail.Transgender.ToString();
                                                    model.TotalAvailableMale = boothExists.Male.ToString();
                                                    model.TotalAvailableFemale = boothExists.Female.ToString();
                                                    model.TotalAvailableTransgender = boothExists.Transgender.ToString();
                                                }
                                                else
                                                {
                                                    model.IsGenderCapturedReqinVT = false;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            model = new VoterTurnOutPolledDetailViewModel()
                                            {
                                                BoothMasterId = boothExists.BoothMasterId,
                                                TotalVoters = boothExists.TotalVoters,
                                                VotesPolled = 0,
                                                VoteEnabled = false,
                                                Message = "Slot not available"

                                            };
                                            if (isGenderCptureRequired == true)
                                            {
                                                model.IsGenderCapturedReqinVT = true;
                                                //model.Male = polldetail.Male.ToString();
                                                //model.Female = polldetail.Female.ToString();
                                                //model.Transgender = polldetail.Transgender.ToString();
                                                model.TotalAvailableMale = boothExists.Male.ToString();
                                                model.TotalAvailableFemale = boothExists.Female.ToString();
                                                model.TotalAvailableTransgender = boothExists.Transgender.ToString();
                                            }
                                            else
                                            {
                                                model.IsGenderCapturedReqinVT = false;
                                            }

                                        }


                                    }

                                    // check whether last slot entry done or not in polled detail

                                }
                            }

                            else
                            {
                                //no slots in teh database
                                model = new VoterTurnOutPolledDetailViewModel()
                                {
                                    BoothMasterId = boothExists.BoothMasterId,
                                    TotalVoters = 0,
                                    VotesPolled = 0,
                                    VotesPolledRecivedTime = null,
                                    VoteEnabled = false,
                                    Message = "Booth Record Not Found."

                                };
                                if (isGenderCptureRequired == true)
                                {
                                    model.IsGenderCapturedReqinVT = true;

                                }
                                else
                                {
                                    model.IsGenderCapturedReqinVT = false;
                                }
                            }

                        }
                        else
                        {

                            model = new VoterTurnOutPolledDetailViewModel()
                            {
                                BoothMasterId = boothExists.BoothMasterId,
                                TotalVoters = boothExists.TotalVoters,
                                VotesPolled = 0,
                                VotesPolledRecivedTime = null,
                                VoteEnabled = false,
                                Message = "Poll not started, Please try after Poll start."

                            };
                            if (isGenderCptureRequired == true)
                            {
                                model.IsGenderCapturedReqinVT = true;
                                //model.Male = polldetail.Male.ToString();
                                //model.Female = polldetail.Female.ToString();
                                //model.Transgender = polldetail.Transgender.ToString();
                                model.TotalAvailableMale = boothExists.Male.ToString();
                                model.TotalAvailableFemale = boothExists.Female.ToString();
                                model.TotalAvailableTransgender = boothExists.Transgender.ToString();
                            }
                            else
                            {
                                model.IsGenderCapturedReqinVT = false;
                            }
                        }
                    }
                    else
                    {
                        model = new VoterTurnOutPolledDetailViewModel()
                        {
                            BoothMasterId = boothExists.BoothMasterId,
                            TotalVoters = 0,
                            VotesPolled = 0,
                            VotesPolledRecivedTime = null,
                            VoteEnabled = false,
                            Message = "Please Check you Previous Events of this booth,Election Info record Not Found."



                        };


                    }

                }
                else
                {
                    //no record found
                    model = new VoterTurnOutPolledDetailViewModel()
                    {
                        BoothMasterId = boothExists.BoothMasterId,
                        TotalVoters = 0,
                        VotesPolled = 0,
                        VotesPolledRecivedTime = null,
                        VoteEnabled = false,
                        Message = "Slots Not Exist in the database."



                    };
                }
            }
            catch (Exception ex)
            {
                model = new VoterTurnOutPolledDetailViewModel()
                {

                    Message = ex.Message



                };
            }
            return model;
        }
        public bool GetLastSlotEntryDone(int boothMasterId, int stateMasterId, int districtMasterId, int assemblyMasterid, int eventmasterid, int slotMgmtId)
        {
            bool islastentryDone = false;
            var polldetail = _context.PollDetails.Where(p => p.BoothMasterId == boothMasterId && p.StateMasterId == stateMasterId && p.DistrictMasterId == districtMasterId && p.SlotManagementId == slotMgmtId && p.AssemblyMasterId == assemblyMasterid).FirstOrDefault();

            if (polldetail != null)
            {
                // then last entry done
                islastentryDone = true;
            }
            else
            {
                // poll can be started
                islastentryDone = false;


            }


            return islastentryDone;
        }

        public async Task<VotesPolledPercentage> GetVotesPolledPercentage(ClaimsIdentity claimsIdentity)
        {
            var stateMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "StateMasterId")?.Value;
            var assemblyMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "AssemblyMasterId")?.Value;
            var districtMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "DistrictMasterId")?.Value;
            var pcMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "PCMasterId")?.Value;
            var rolesClaim = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
            var roles = rolesClaim?.Value;


            var totalVoters = new List<int?>();
            var sumOfFinalTVote = new List<int?>();
            var finalIsTrue = new List<int?>();


            if (roles == "SuperAdmin" || roles == "ECI" || roles == "StateAdmin")
            {
                var totalVotersSum = _context.BoothMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId)).Sum(p => p.TotalVoters);
                var sumOfFinalTVoteSum = _context.ElectionInfoMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId)).Sum(p => p.FinalTVote);
                var finalIsTrueSum = _context.ElectionInfoMaster.Where(p => p.FinalTVoteStatus == true && p.StateMasterId == Convert.ToInt32(stateMasterId)).Sum(p => p.FinalTVote);


                totalVoters.Add(totalVotersSum);
                sumOfFinalTVote.Add(sumOfFinalTVoteSum);
                finalIsTrue.Add(finalIsTrueSum);
            }
            else if (roles == "DistrictAdmin")
            {

                var totalVotersSum = _context.BoothMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId))
                   .Sum(p => p.TotalVoters);
                var sumOfFinalTVoteSum = _context.ElectionInfoMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId))
                   .Sum(p => p.FinalTVote);
                var finalIsTrueSum = _context.ElectionInfoMaster.Where(d => d.FinalTVoteStatus == true && d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId))
                    .Sum(p => p.FinalTVote);


                totalVoters.Add(totalVotersSum);
                sumOfFinalTVote.Add(sumOfFinalTVoteSum);
                finalIsTrue.Add(finalIsTrueSum);
            }
            //Need to be done
            //else if (roles == "PC")
            //{
            //    var pollDetailCount = _context.PollDetails
            //        .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.PCMasterId == Convert.ToInt32(pcMasterId))
            //        .Sum(d => d.VotesPolled);

            //    var totalVotersCount = _context.BoothMaster
            //        .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId))
            //        .Sum(d => d.TotalVoters);

            //    votesPolledCounts.Add(pollDetailCount);
            //    totalVotesCounts.Add(totalVotersCount);
            //}

            else if (roles == "ARO")
            {

                var totalVotersSum = _context.BoothMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId)).Sum(p => p.TotalVoters);
                var sumOfFinalTVoteSum = _context.ElectionInfoMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId)).Sum(p => p.FinalTVote);
                var finalIsTrueSum = _context.ElectionInfoMaster.Where(d => d.FinalTVoteStatus == true && d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId))
                    .Sum(p => p.FinalTVote);


                totalVoters.Add(totalVotersSum);
                sumOfFinalTVote.Add(sumOfFinalTVoteSum);
                finalIsTrue.Add(finalIsTrueSum);
            }


            //Check if TotalVoters is not zero to avoid division by zero
            if (totalVoters != null && totalVoters.Count != 0)
            {
                //decimal percentage = Math.Round((decimal)(15176192 * 100.0 / 21499804), 1);
                decimal percentage = Math.Round((decimal)(sumOfFinalTVote.FirstOrDefault() * 100.0 / totalVoters.FirstOrDefault()), 1);
                string message = $"Total Voters: {totalVoters.FirstOrDefault()}, Votes Polled: {sumOfFinalTVote.FirstOrDefault()}, Percentage: {percentage}%";

                VotesPolledPercentage votesPolledPercentage = new VotesPolledPercentage()
                {
                    TotalVoters = totalVoters.FirstOrDefault(),
                    VotesPolled = sumOfFinalTVote.FirstOrDefault(),
                    PollPercentage = percentage.ToString(),
                    FinalVote = finalIsTrue.FirstOrDefault(),

                };
                return votesPolledPercentage;
            }
            else
            {
                return null;
            }
        }


        public async Task<EAMS_ACore.Models.Queue> GetVoterInQueue(string boothMasterId)
        {
            EAMS_ACore.Models.Queue model;
            try
            {
                var boothExists = await _context.BoothMaster.Where(p => p.BoothMasterId == Convert.ToInt32(boothMasterId)).FirstOrDefaultAsync();
                //var electionInfoRecord = await _context.ElectionInfoMaster.Where(p => p.BoothMasterId == Convert.ToInt32(boothMasterId) && p.StateMasterId == boothExists.StateMasterId && p.DistrictMasterId == boothExists.DistrictMasterId).FirstOrDefaultAsync();
                var polldetail = await _context.PollDetails.Where(p => p.BoothMasterId == Convert.ToInt32(boothMasterId) && p.StateMasterId == boothExists.StateMasterId && p.DistrictMasterId == boothExists.DistrictMasterId).OrderByDescending(p => p.VotesPolledRecivedTime).FirstOrDefaultAsync();

                if (boothExists is not null)
                {
                    var electionInfoRecord = await _context.ElectionInfoMaster.Where(p => p.StateMasterId == boothExists.StateMasterId && p.DistrictMasterId == boothExists.DistrictMasterId && p.AssemblyMasterId == boothExists.AssemblyMasterId && p.BoothMasterId == Convert.ToInt32(boothMasterId)).FirstOrDefaultAsync();
                    if (electionInfoRecord is not null)
                    {
                        if (electionInfoRecord.VoterInQueue == null)
                        {
                            bool QueueCanStart = await CanQueueStart(electionInfoRecord.BoothMasterId);
                            if (QueueCanStart == true)
                            {
                                bool queueTimeOpen = await QueueTime(Convert.ToInt32(boothMasterId));
                                if (queueTimeOpen == true)
                                {
                                    if (polldetail != null)
                                    {
                                        model = new EAMS_ACore.Models.Queue()
                                        {
                                            BoothMasterId = boothExists.BoothMasterId,
                                            TotalVoters = boothExists.TotalVoters,
                                            VotesPolled = polldetail.VotesPolled,
                                            VotesPolledTime = polldetail.VotesPolledRecivedTime,
                                            RemainingVotes = boothExists.TotalVoters - polldetail.VotesPolled,
                                            VoteEnabled = true,
                                            Message = "Queue is Available"


                                        };
                                    }
                                    else
                                    {
                                        model = new EAMS_ACore.Models.Queue()
                                        {
                                            BoothMasterId = boothExists.BoothMasterId,
                                            TotalVoters = boothExists.TotalVoters,
                                            RemainingVotes = boothExists.TotalVoters - 0,
                                            VoteEnabled = true,
                                            Message = "Queue is Available, You have not entered any value in Voter Turn Out of this Booth."


                                        };
                                    }


                                }
                                else
                                {
                                    model = new EAMS_ACore.Models.Queue()
                                    {

                                        BoothMasterId = boothExists.BoothMasterId,
                                        TotalVoters = boothExists.TotalVoters,
                                        VotesPolledTime = electionInfoRecord.VoterInQueueLastUpdate,
                                        VoteEnabled = false,
                                        RemainingVotes = null,
                                        Message = "Queue will be Open at Specified Time."


                                    };
                                }


                            }
                            else
                            {
                                model = new EAMS_ACore.Models.Queue()
                                {

                                    BoothMasterId = boothExists.BoothMasterId,
                                    TotalVoters = boothExists.TotalVoters,
                                    VotesPolled = electionInfoRecord.VoterInQueue,
                                    VotesPolledTime = electionInfoRecord.VoterInQueueLastUpdate,
                                    VoteEnabled = false,
                                    RemainingVotes = null,
                                    //Message = "Queue will be Open at Specified Time."
                                    Message = "Voter Turn Out Not Updated Yet."

                                };
                            }
                        }
                        else
                        {
                            model = new EAMS_ACore.Models.Queue()
                            {

                                BoothMasterId = boothExists.BoothMasterId,
                                TotalVoters = boothExists.TotalVoters,
                                VotesPolled = electionInfoRecord.VoterInQueue,
                                VotesPolledTime = electionInfoRecord.VoterInQueueLastUpdate,
                                VoteEnabled = false,
                                Message = "Queue Already Done."

                            };
                        }


                    }
                    else
                    {
                        //Polling should not be more than Total Voters!
                        model = new EAMS_ACore.Models.Queue()
                        {
                            BoothMasterId = 0,
                            TotalVoters = null,
                            VotesPolled = null,
                            VotesPolledTime = null,
                            VoteEnabled = false,
                            Message = "Please Update Previous Events of this Booth, Election Info Record Not Found."


                        };
                    }



                }
                else
                {
                    //Polling should not be more than Total Voters!
                    model = new EAMS_ACore.Models.Queue()
                    {
                        BoothMasterId = 0,
                        TotalVoters = null,
                        VotesPolled = null,
                        VotesPolledTime = null,
                        VoteEnabled = false,
                        Message = "Booth record Not Found"


                    };

                }


            }
            catch (Exception ex)
            {
                model = new EAMS_ACore.Models.Queue()
                {

                    Message = ex.Message



                };
            }
            return model;
        }

        public async Task<EAMS_ACore.Models.Queue> GetTotalRemainingVoters(string boothMasterId)
        {
            EAMS_ACore.Models.Queue model = null;
            try
            {
                var boothExists = await _context.BoothMaster.Where(p => p.BoothMasterId == Convert.ToInt32(boothMasterId)).FirstOrDefaultAsync();
                var electionInfoRecord = await _context.ElectionInfoMaster.Where(p => p.StateMasterId == boothExists.StateMasterId && p.DistrictMasterId == boothExists.DistrictMasterId && p.AssemblyMasterId == boothExists.AssemblyMasterId && p.BoothMasterId == Convert.ToInt32(boothMasterId)).FirstOrDefaultAsync();
                var polldetail = await _context.PollDetails.Where(p => p.BoothMasterId == Convert.ToInt32(boothMasterId) && p.StateMasterId == boothExists.StateMasterId && p.DistrictMasterId == boothExists.DistrictMasterId).OrderByDescending(p => p.VotesPolledRecivedTime).FirstOrDefaultAsync();

                if (boothExists is not null)
                {
                    if (polldetail != null)
                    {
                        model = new EAMS_ACore.Models.Queue()
                        {
                            TotalVoters = boothExists.TotalVoters,
                            VotesPolled = polldetail.VotesPolled,
                            RemainingVotes = boothExists.TotalVoters - polldetail.VotesPolled,

                        };
                    }
                    else
                    {
                        model = new EAMS_ACore.Models.Queue()
                        {
                            TotalVoters = boothExists.TotalVoters,
                            VotesPolled = 0,
                            RemainingVotes = boothExists.TotalVoters - 0,
                        };
                    }



                }


            }
            catch (Exception ex)
            {
                model = new EAMS_ACore.Models.Queue()
                {

                    Message = ex.Message



                };
            }
            return model;
        }
        public async Task<Response> AddVoterTurnOut(AddVoterTurnOut addVoterTurnOut)
        {
            PollDetail model;
            try
            {

                var boothExists = await _context.BoothMaster.Where(p => p.BoothMasterId == Convert.ToInt32(addVoterTurnOut.boothMasterId)).FirstOrDefaultAsync();
                var polldetail = await _context.PollDetails.Where(p => p.BoothMasterId == Convert.ToInt32(addVoterTurnOut.boothMasterId) && p.StateMasterId == boothExists.StateMasterId && p.DistrictMasterId == boothExists.DistrictMasterId).OrderByDescending(p => p.VotesPolledRecivedTime).FirstOrDefaultAsync();
                var slotsList = await _context.SlotManagementMaster.Where(p => p.StateMasterId == boothExists.StateMasterId && p.EventMasterId == Convert.ToInt32(addVoterTurnOut.eventid)).OrderBy(p => p.SlotManagementId).ToListAsync();
                var isGenderCptureRequired = await _context.StateMaster.Where(p => p.StateMasterId == boothExists.StateMasterId).Select(p => p.IsGenderCapturedinVoterTurnOut).FirstOrDefaultAsync();

                if (boothExists is not null)
                {
                    var electionInfoRecord = await _context.ElectionInfoMaster.Where(p => p.StateMasterId == boothExists.StateMasterId && p.DistrictMasterId == boothExists.DistrictMasterId && p.AssemblyMasterId == boothExists.AssemblyMasterId && p.BoothMasterId == Convert.ToInt32(addVoterTurnOut.boothMasterId)).FirstOrDefaultAsync();
                    if (electionInfoRecord is not null)
                    {
                        //if (isGenderCptureRequired != null)
                        //{
                        if (electionInfoRecord.IsPollStarted == true)
                        {

                            if (Convert.ToInt32(addVoterTurnOut.voterValue) <= boothExists.TotalVoters)
                            {

                                if (slotsList.Count() > 0) // any1 slot is there in poll table 
                                {
                                    // get end time  and  compare with curent time if current time greater than say proceed for queue
                                    var slotlast = slotsList.OrderByDescending(p => p.SlotManagementId).FirstOrDefault();
                                    bool lastslotexceededtime = await TimeExceedLastSlot(slotlast);
                                    if (lastslotexceededtime == false)
                                    {

                                        int SlotRecordMasterId = await GetSlot(slotsList);
                                        if (SlotRecordMasterId > 0)
                                        {
                                            var SlotRecord = await _context.SlotManagementMaster.Where(p => p.SlotManagementId == SlotRecordMasterId).FirstOrDefaultAsync();
                                            if (polldetail is not null)
                                            {
                                                // check whether current time slot already entered or not

                                                bool VoterTurnOutAlreadyExistsinSlot = await IsSlotAlreadyEntered(SlotRecord, polldetail.VotesPolledRecivedTime);

                                                if (VoterTurnOutAlreadyExistsinSlot == false)
                                                {
                                                    if (Convert.ToInt32(addVoterTurnOut.voterValue) < polldetail.VotesPolled)
                                                    {
                                                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Voter Value cannot be less than Last Votes Polled!" };

                                                    }
                                                    else
                                                    {
                                                        model = new PollDetail()
                                                        {
                                                            SlotManagementId = SlotRecordMasterId,
                                                            StateMasterId = boothExists.StateMasterId,
                                                            DistrictMasterId = boothExists.DistrictMasterId,
                                                            AssemblyMasterId = boothExists.AssemblyMasterId,
                                                            BoothMasterId = Convert.ToInt32(addVoterTurnOut.boothMasterId),
                                                            EventMasterId = Convert.ToInt32(addVoterTurnOut.eventid),
                                                            VotesPolled = Convert.ToInt32(addVoterTurnOut.voterValue),
                                                            VotesPolledRecivedTime = BharatDateTime(),


                                                            PCMasterId = _context.AssemblyMaster.Where(p => p.AssemblyMasterId == boothExists.AssemblyMasterId).Select(p => p.PCMasterId).FirstOrDefault(),

                                                            UserType = "SO"
                                                            //AddedBy=Soid  // find SO or ARO
                                                        };
                                                        if (isGenderCptureRequired == true)
                                                        { // then check male female must have values
                                                            if (Convert.ToInt32(addVoterTurnOut.Male) >= 0 && Convert.ToInt32(addVoterTurnOut.Female) >= 0 && Convert.ToInt32(addVoterTurnOut.Transgender) >= 0)

                                                            {
                                                                if (Convert.ToInt32(addVoterTurnOut.voterValue) == Convert.ToInt32(addVoterTurnOut.Male) + Convert.ToInt32(addVoterTurnOut.Female) + Convert.ToInt32(addVoterTurnOut.Transgender))
                                                                {
                                                                    // male is euql or less than vaailble male,f,yt voters
                                                                    if (Convert.ToInt32(addVoterTurnOut.Male) <= boothExists.Male && Convert.ToInt32(addVoterTurnOut.Female) <= boothExists.Female && Convert.ToInt32(addVoterTurnOut.Transgender) <= boothExists.Transgender)
                                                                    {
                                                                        boothExists.Male.ToString();
                                                                        model.Male = Convert.ToInt32(addVoterTurnOut.Male);
                                                                        model.Female = Convert.ToInt32(addVoterTurnOut.Female); // Assuming this was intended to be Female
                                                                        model.Transgender = Convert.ToInt32(addVoterTurnOut.Transgender);
                                                                    }
                                                                    else
                                                                    {
                                                                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "The tally of votes cast for males, females, and transgender individuals does not match the corresponding available counts." };

                                                                    }

                                                                }
                                                                else
                                                                {
                                                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Voter Value Sum is not equal to Male,Female & Transgender Values" };

                                                                }


                                                            }
                                                            else
                                                            {

                                                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Kindly fill Male Female & transgender in Voter Turn Out" };

                                                            }
                                                        }
                                                        //else
                                                        //{
                                                        //    if (Convert.ToInt32(addVoterTurnOut.Male) >= 0 || Convert.ToInt32(addVoterTurnOut.Female) >= 0 || Convert.ToInt32(addVoterTurnOut.Transgender) >= 0)
                                                        //    {
                                                        //        return new Response { Status = RequestStatusEnum.OK, Message = " Male,Female & Transgender Values Cannot be entered as it is not defined in State!" };

                                                        //    }

                                                        //}
                                                        _context.PollDetails.Add(model);
                                                        electionInfoRecord.FinalTVote = Convert.ToInt32(addVoterTurnOut.voterValue);
                                                        electionInfoRecord.VotingLastUpdate = BharatDateTime();
                                                        _context.ElectionInfoMaster.Update(electionInfoRecord);
                                                        await _context.SaveChangesAsync();
                                                        return new Response { Status = RequestStatusEnum.OK, Message = "Voter Turn Out for " + boothExists.BoothName + " entered successfully!" };
                                                    }
                                                }
                                                else
                                                {
                                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Voter Turn Out for " + boothExists.BoothName + " Already entered !" };

                                                }

                                            }
                                            else
                                            { // as it is insert  in poll detail but check votervalue should be greater than old value
                                              //if (Convert.ToInt32(voterValue) > boothExists.TotalVoters)
                                              //{
                                                model = new PollDetail()
                                                {
                                                    SlotManagementId = SlotRecordMasterId,
                                                    StateMasterId = boothExists.StateMasterId,
                                                    DistrictMasterId = boothExists.DistrictMasterId,
                                                    AssemblyMasterId = boothExists.AssemblyMasterId,
                                                    BoothMasterId = Convert.ToInt32(addVoterTurnOut.boothMasterId),
                                                    EventMasterId = Convert.ToInt32(addVoterTurnOut.eventid),
                                                    VotesPolled = Convert.ToInt32(addVoterTurnOut.voterValue),
                                                    VotesPolledRecivedTime = BharatDateTime(),

                                                    UserType = "SO",
                                                    PCMasterId = _context.AssemblyMaster.Where(p => p.AssemblyMasterId == boothExists.AssemblyMasterId)
    .Select(p => p.PCMasterId)
    .FirstOrDefault(),

                                                    //AddedBy=Soid  // find SO or ARO
                                                };
                                                if (isGenderCptureRequired == true)
                                                { // then check male female must have values
                                                    if (Convert.ToInt32(addVoterTurnOut.Male) >= 0 && Convert.ToInt32(addVoterTurnOut.Female) >= 0 && Convert.ToInt32(addVoterTurnOut.Transgender) >= 0)

                                                    {
                                                        if (Convert.ToInt32(addVoterTurnOut.voterValue) == Convert.ToInt32(addVoterTurnOut.Male) + Convert.ToInt32(addVoterTurnOut.Female) + Convert.ToInt32(addVoterTurnOut.Transgender))
                                                        {
                                                            // male is euql or less than vaailble male,f,yt voters
                                                            if (Convert.ToInt32(addVoterTurnOut.Male) <= boothExists.Male && Convert.ToInt32(addVoterTurnOut.Female) <= boothExists.Female && Convert.ToInt32(addVoterTurnOut.Transgender) <= boothExists.Transgender)
                                                            {
                                                                boothExists.Male.ToString();
                                                                model.Male = Convert.ToInt32(addVoterTurnOut.Male);
                                                                model.Female = Convert.ToInt32(addVoterTurnOut.Female); // Assuming this was intended to be Female
                                                                model.Transgender = Convert.ToInt32(addVoterTurnOut.Transgender);
                                                            }
                                                            else
                                                            {
                                                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "The tally of votes cast for males, females, and transgender individuals does not match the corresponding available counts." };

                                                            }

                                                        }
                                                        else
                                                        {
                                                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Voter Value Sum is not equal to Male,Female & Transgender Values" };

                                                        }

                                                    }
                                                    else
                                                    {
                                                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Kindly fill Male Female & transgender in Voter Turn Out" };

                                                    }
                                                }
                                                //else
                                                //{
                                                //    if (Convert.ToInt32(addVoterTurnOut.Male) >= 0 || Convert.ToInt32(addVoterTurnOut.Female) >= 0 || Convert.ToInt32(addVoterTurnOut.Transgender) >= 0)
                                                //    {
                                                //        return new Response { Status = RequestStatusEnum.OK, Message = " Male,Female & Transgender Values Cannot be entered as it is not defined in State!" };

                                                //    }

                                                //}

                                                _context.PollDetails.Add(model);
                                                electionInfoRecord.FinalTVote = Convert.ToInt32(addVoterTurnOut.voterValue);
                                                electionInfoRecord.VotingLastUpdate = BharatDateTime();
                                                _context.ElectionInfoMaster.Update(electionInfoRecord);
                                                await _context.SaveChangesAsync();
                                                return new Response { Status = RequestStatusEnum.OK, Message = "Voter Turn Out for " + boothExists.BoothName + " entered successfully!" };



                                                //}
                                                //else
                                                //{
                                                //    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Voter Turn Out Value cannot be less than Last Added value" };
                                                //}


                                            }

                                        }
                                        else
                                        {

                                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Slot Not Available" };


                                        }
                                    }
                                    else
                                    {
                                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Voter Turn Out Closed, Kindly Proceed for Voter in Queue" };
                                    }
                                }

                                else
                                {
                                    //no slots in teh database
                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Slots Not Exist in the database" };

                                }

                            }
                            else
                            {
                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Polling should not be more than Total Voters!" };

                            }

                        }
                        else
                        {
                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Poll not started, Please try after Poll start." };

                        }
                        //}
                        //else
                        //{
                        //    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Is Gender Capture not Found in the State" };

                        //}
                    }
                    else
                    {
                        //no record found
                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Election Info record Not Found" };
                    }
                }
                else
                {
                    //no record found
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Booth Record Not Found" };

                }
            }
            catch (Exception ex)
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = ex.Message };
            }

        }

        // for event activity Check Condition
        public async Task<bool> CanPollStart(int boothMasterId, int eventmasterid)
        {
            bool pollCanStart = false;
            var boothExists = await _context.BoothMaster.Where(p => p.BoothMasterId == boothMasterId).FirstOrDefaultAsync();
            var polldetail = await _context.PollDetails.Where(p => p.BoothMasterId == boothMasterId && p.StateMasterId == boothExists.StateMasterId && p.DistrictMasterId == boothExists.DistrictMasterId).OrderByDescending(p => p.VotesPolledRecivedTime).FirstOrDefaultAsync();
            var getLastSlot = await _context.SlotManagementMaster.Where(p => p.StateMasterId == boothExists.StateMasterId && p.EventMasterId == eventmasterid && p.IsLastSlot == true).OrderByDescending(p => p.SlotManagementId).FirstOrDefaultAsync();
            if (polldetail != null)
            {
                // then poll
                pollCanStart = false;
            }
            else
            {
                // poll can be started
                pollCanStart = true;


            }


            return pollCanStart;
        }

        public async Task<bool> CanQueueStart(int boothMasterId)
        {
            bool queueCanStart = false;
            var boothExists = await _context.BoothMaster.Where(p => p.BoothMasterId == boothMasterId).FirstOrDefaultAsync();
            var polldetail = await _context.PollDetails.Where(p => p.BoothMasterId == boothMasterId && p.StateMasterId == boothExists.StateMasterId && p.DistrictMasterId == boothExists.DistrictMasterId).OrderByDescending(p => p.VotesPolledRecivedTime).FirstOrDefaultAsync();

            var slotRecord = await _context.SlotManagementMaster.Where(p => p.StateMasterId == boothExists.StateMasterId && p.EventMasterId == 6).OrderByDescending(p => p.SlotManagementId).FirstOrDefaultAsync();
            if (polldetail != null)
            {
                queueCanStart = true;
            }
            else
            {
                // poll can be started if currenttimme exceeded last slot end time otherwse event cannot be donefor queue 
                //blocking state will happen

                DateTime currentTime = DateTime.Now;
                DateTime endTime = DateTime.ParseExact(slotRecord.EndTime.ToString(), "HH:mm", CultureInfo.InvariantCulture);
                if (currentTime >= endTime)  // 6 PM (18:00)
                {
                    // queue is open
                    queueCanStart = true;
                }

                else
                {
                    // queue cannot open before specified time
                    queueCanStart = false;
                }



            }


            return queueCanStart;
        }

        public async Task<bool> QueueTime(int boothMasterId)
        {
            bool queueCanStart = false;
            var boothExists = await _context.BoothMaster.Where(p => p.BoothMasterId == boothMasterId).FirstOrDefaultAsync();
            //var polldetail = _context.PollDetails.Where(p => p.BoothMasterId == boothMasterId && p.StateMasterId == boothExists.StateMasterId && p.DistrictMasterId == boothExists.DistrictMasterId).OrderByDescending(p => p.VotesPolledRecivedTime).FirstOrDefault();
            var slotRecord = await _context.SlotManagementMaster.Where(p => p.StateMasterId == boothExists.StateMasterId && p.EventMasterId == 6).OrderByDescending(p => p.SlotManagementId).FirstOrDefaultAsync();

            //if required time can be checked in order to open queue
            DateTime currentTime = DateTime.Now;
            //DateTime currentTime = DateTime.SpecifyKind(currentTimes.ToUniversalTime(), DateTimeKind.Utc);
            DateTime strtTime = DateTime.ParseExact(slotRecord.StartTime.ToString(), "HH:mm", CultureInfo.InvariantCulture);
            DateTime endTime = DateTime.ParseExact(slotRecord.EndTime.ToString(), "HH:mm", CultureInfo.InvariantCulture);
            DateTime lockTime = DateTime.ParseExact(slotRecord.LockTime.ToString(), "HH:mm", CultureInfo.InvariantCulture);

            //if (currentTime >= lockTime)
            if (currentTime >= endTime)  // 6 PM (18:00)
            {
                // queue is open
                queueCanStart = true;
            }

            else
            {
                // queue cannot open before specified time
                queueCanStart = false;
            }



            return queueCanStart;
        }
        public async Task<bool> CanFinalValueStart(int boothMasterId)
        {
            bool finalCanStart = false;
            var boothExists = _context.BoothMaster.Where(p => p.BoothMasterId == boothMasterId).FirstOrDefault();
            var electionInfo = _context.ElectionInfoMaster.Where(p => p.BoothMasterId == boothMasterId && p.StateMasterId == boothExists.StateMasterId && p.DistrictMasterId == boothExists.DistrictMasterId).FirstOrDefault();
            // var slotRecord = _context.SlotManagementMaster.Where(p => p.StateMasterId == boothExists.StateMasterId && p.EventMasterId == eventmasterid).OrderByDescending(p => p.SlotManagementId).ToList();
            //if (electionInfo != null && electionInfo.FinalTVote == null)
            if (electionInfo != null && (electionInfo.FinalTVoteStatus == null || electionInfo.FinalTVoteStatus == false))
            {
                finalCanStart = true;
            }
            else
            {
                // final voting can be started
                finalCanStart = false;


            }


            return finalCanStart;
        }
        public async Task<bool> IsPollInterrupted(int boothMasterId)
        {
            bool ispollInterrupted = false;
            var pollInterruptionData = await _context.PollInterruptions.Where(p => p.BoothMasterId == boothMasterId).OrderByDescending(p => p.CreatedAt).FirstOrDefaultAsync();

            if (pollInterruptionData != null && pollInterruptionData.IsPollInterrupted == true && pollInterruptionData.StopTime != null && pollInterruptionData.ResumeTime is null)
            {
                ispollInterrupted = true;
            }
            else if (pollInterruptionData != null && pollInterruptionData.IsPollInterrupted == true && pollInterruptionData.StopTime != null && pollInterruptionData.ResumeTime is not null)
            {
                ispollInterrupted = false;
            }
            else
            {
                ispollInterrupted = false;


            }

            return ispollInterrupted;
        }
        public async Task<FinalViewModel> GetFinalVotes(string boothMasterId)
        {
            FinalViewModel model = null;
            try
            {
                var boothExists = await _context.BoothMaster.Where(p => p.BoothMasterId == Convert.ToInt32(boothMasterId)).FirstOrDefaultAsync();
                //var electionInfoRecord = await _context.ElectionInfoMaster.Where(p => p.BoothMasterId == Convert.ToInt32(boothMasterId) && p.StateMasterId == boothExists.StateMasterId && p.DistrictMasterId == boothExists.DistrictMasterId).FirstOrDefaultAsync();
                var polldetail = await _context.PollDetails.Where(p => p.BoothMasterId == Convert.ToInt32(boothMasterId) && p.StateMasterId == boothExists.StateMasterId && p.DistrictMasterId == boothExists.DistrictMasterId).OrderByDescending(p => p.VotesPolledRecivedTime).FirstOrDefaultAsync();
                int lastVotespolled = 0;
                if (polldetail != null)
                {
                    lastVotespolled = polldetail.VotesPolled;
                }
                if (boothExists is not null)
                {
                    var electionInfoRecord = await _context.ElectionInfoMaster.Where(p => p.StateMasterId == boothExists.StateMasterId && p.DistrictMasterId == boothExists.DistrictMasterId && p.AssemblyMasterId == boothExists.AssemblyMasterId && p.BoothMasterId == Convert.ToInt32(boothMasterId)).FirstOrDefaultAsync();
                    if (electionInfoRecord is not null)
                    {
                        if (electionInfoRecord.VoterInQueue != null)
                        {
                            if (electionInfoRecord.IsPollEnded == false || electionInfoRecord.IsPollEnded == null)
                            {
                                bool FinalCanStart = await CanFinalValueStart(Convert.ToInt32(boothMasterId));
                                if (FinalCanStart == true)
                                {

                                    model = new FinalViewModel()
                                    {
                                        BoothMasterId = boothExists.BoothMasterId,
                                        TotalVoters = boothExists.TotalVoters,
                                        LastVotesPolled = lastVotespolled,
                                        //LastFinalVotesPolled = electionInfoRecord.FinalTVote,
                                        VotesFinalPolledTime = electionInfoRecord.VotingLastUpdate,
                                        VoteEnabled = true,
                                        TotalAvailableMale = boothExists.Male.ToString(),
                                        TotalAvailableFemale = boothExists.Female.ToString(),
                                        TotalAvailableTransgender = boothExists.Transgender.ToString(),
                                        Male = electionInfoRecord.Male.ToString(),
                                        Female = electionInfoRecord.Female.ToString(),
                                        Transgender = electionInfoRecord.Transgender.ToString(),
                                        edc = electionInfoRecord.EDC.ToString(),
                                        Message = "Final Value is Available"


                                    };
                                    // Check the condition
                                    if (electionInfoRecord.FinalTVoteStatus != null)
                                    {
                                        // Assign value if condition is true
                                        model.LastFinalVotesPolled = electionInfoRecord.FinalTVote;
                                    }
                                }

                                else
                                {
                                    model = new FinalViewModel()
                                    {
                                        BoothMasterId = boothExists.BoothMasterId,
                                        TotalVoters = boothExists.TotalVoters,
                                        LastVotesPolled = lastVotespolled,
                                        VotesFinalPolledTime = electionInfoRecord.VotingLastUpdate,
                                        VoteEnabled = true,
                                        TotalAvailableMale = boothExists.Male.ToString(),
                                        TotalAvailableFemale = boothExists.Female.ToString(),
                                        TotalAvailableTransgender = boothExists.Transgender.ToString(),
                                        Male = electionInfoRecord.Male.ToString(),
                                        Female = electionInfoRecord.Female.ToString(),
                                        Transgender = electionInfoRecord.Transgender.ToString(),
                                        Message = "Final Value is Available, Last Entered :" + electionInfoRecord.FinalTVote,
                                        edc = electionInfoRecord.EDC.ToString()
                                    };

                                    // Check the condition
                                    if (electionInfoRecord.FinalTVoteStatus != null)
                                    {
                                        // Assign value if condition is true
                                        model.LastFinalVotesPolled = electionInfoRecord.FinalTVote;
                                    }

                                }

                            }
                            else
                            {

                                model = new FinalViewModel()
                                {
                                    BoothMasterId = boothExists.BoothMasterId,
                                    TotalVoters = boothExists.TotalVoters,
                                    LastVotesPolled = lastVotespolled,
                                    LastFinalVotesPolled = electionInfoRecord.FinalTVote,
                                    TotalAvailableMale = boothExists.Male.ToString(),
                                    TotalAvailableFemale = boothExists.Female.ToString(),
                                    TotalAvailableTransgender = boothExists.Transgender.ToString(),
                                    Male = electionInfoRecord.Male.ToString(),
                                    Female = electionInfoRecord.Female.ToString(),
                                    Transgender = electionInfoRecord.Transgender.ToString(),
                                    VoteEnabled = false,
                                    Message = "Final Value Not Available, Poll Already Ended",
                                    edc = electionInfoRecord.EDC.ToString()
                                };
                                // Check the condition
                                if (electionInfoRecord.FinalTVoteStatus != null)
                                {
                                    // Assign value if condition is true
                                    model.LastFinalVotesPolled = electionInfoRecord.FinalTVote;
                                }
                            }
                        }
                        else
                        {
                            model = new FinalViewModel()
                            {
                                BoothMasterId = boothExists.BoothMasterId,
                                TotalVoters = boothExists.TotalVoters,
                                LastVotesPolled = lastVotespolled,
                                LastFinalVotesPolled = null,
                                VotesFinalPolledTime = null,

                                VoteEnabled = false,
                                Message = "Final Value Not Available",
                                edc = electionInfoRecord.EDC.ToString()


                            };
                        }



                    }
                    else
                    {
                        model = new FinalViewModel()
                        {
                            BoothMasterId = boothExists.BoothMasterId,
                            TotalVoters = boothExists.TotalVoters,
                            LastVotesPolled = null,
                            LastFinalVotesPolled = null,
                            VotesFinalPolledTime = null,
                            TotalAvailableMale = boothExists.Male.ToString(),
                            TotalAvailableFemale = boothExists.Female.ToString(),
                            TotalAvailableTransgender = boothExists.Transgender.ToString(),
                            Male = null,
                            Female = null,
                            Transgender = null,
                            VoteEnabled = false,
                            Message = "Election Info Record Not Found, Pls Check Previous Events.",
                            edc = electionInfoRecord.EDC.ToString()


                        };
                    }



                }



            }
            catch (Exception ex)
            {
                model = new FinalViewModel()
                {

                    Message = ex.Message



                };
            }
            return model;
        }
        public async Task<int> GetSlot(List<SlotManagementMaster> slotLists)
        {
            int slotId = 0;

            DateTime currentTime = DateTime.Now;
            //DateTime currentTime = DateTime.SpecifyKind(currentTimes.ToUniversalTime(), DateTimeKind.Utc);

            foreach (var slot in slotLists)
            {

                DateTime endTime = DateTime.ParseExact(slot.EndTime.ToString(), "HH:mm", CultureInfo.InvariantCulture);
                DateTime lockTime = DateTime.ParseExact(slot.LockTime.ToString(), "HH:mm", CultureInfo.InvariantCulture);


                if (currentTime >= endTime && currentTime <= lockTime)
                {

                    slotId = slot.SlotManagementId;
                    break;
                }
            }

            return slotId;
        }
        public async Task<int> GetNextSlot(List<SlotManagementMaster> slotLists)
        {
            int slotId = 0;

            DateTime currentTime = DateTime.Now;
            //DateTime currentTime = DateTime.SpecifyKind(currentTimes.ToUniversalTime(), DateTimeKind.Utc);

            foreach (var slot in slotLists)
            {

                DateTime endTime = DateTime.ParseExact(slot.EndTime.ToString(), "HH:mm", CultureInfo.InvariantCulture);
                DateTime lockTime = DateTime.ParseExact(slot.LockTime.ToString(), "HH:mm", CultureInfo.InvariantCulture);


                if (currentTime < endTime && currentTime < lockTime)
                {

                    slotId = slot.SlotManagementId;
                    break;
                }
            }

            return slotId;
        }
        public async Task<bool> IsSlotAlreadyEntered(SlotManagementMaster? slotRecord, DateTime? lastReceviedTime)
        {
            bool slotTurnOutValueAlreadyExists = false;
            DateTime endTime = DateTime.ParseExact(slotRecord.EndTime.ToString(), "HH:mm", CultureInfo.InvariantCulture);
            DateTime lockTime = DateTime.ParseExact(slotRecord.LockTime.ToString(), "HH:mm", CultureInfo.InvariantCulture);

            var lastEnteredTime = lastReceviedTime.Value.Hour + ":" + lastReceviedTime.Value.Minute;


            //DateTime lsttime = DateTime.ParseExact(lastEnteredTime, "HH:mm", CultureInfo.InvariantCulture);
            if (lastReceviedTime > endTime && lastReceviedTime < lockTime)
            {
                //  lastEnteredTime is between endTime and lockTime.
                return slotTurnOutValueAlreadyExists = true;

            }
            else
            {
                return slotTurnOutValueAlreadyExists = false;


            }

            return slotTurnOutValueAlreadyExists;
        }

        public async Task<bool> TimeExceedLastSlot(SlotManagementMaster? slotRecord)
        {
            bool lastSlotExceededTime = false;


            DateTime currentTime = DateTime.Now;
            //DateTime currentTime = DateTime.SpecifyKind(currentTimes.ToUniversalTime(), DateTimeKind.Utc);
            DateTime lockTime = DateTime.ParseExact(slotRecord.LockTime.ToString(), "HH:mm", CultureInfo.InvariantCulture);
            if (currentTime > lockTime)
            {

                return lastSlotExceededTime = true;

            }
            else
            {
                return lastSlotExceededTime = false;


            }
            return lastSlotExceededTime;

        }


        //public async Task<List<EventWiseBoothStatus>> EventWiseBoothStatus(string soId)
        //{
        //    var soTotalBooths = await _context.BoothMaster.Where(p => p.AssignedTo == soId).ToListAsync();
        //    //List<BoothMaster> soTotalBooths = new List<BoothMaster>();

        //    List<EventWiseBoothStatus> list = new List<EventWiseBoothStatus>();

        //    int totalPartyDispatched = 0; int totalPartyReached = 0; int totalIsetUpPolling = 0;
        //    int totalmockpoll = 0; int totalpollstarted = 0; int totalvoterturedout = 0;
        //    int totalQueue = 0; int totalfinalvotes = 0;
        //    int totalpollended = 0; int totalmcevm = 0; int totalpartdeparted = 0; int totalpartycollectoncentre = 0;
        //    int totalevmdeposited = 0; int totalPollInterrupted = 0;

        //    int pendingPartyDispatched = 0;
        //    int pendingPartyReached = 0;
        //    int pendingIsetUpPolling = 0;
        //    int pendingMockPoll = 0;
        //    int pendingPollStarted = 0;
        //    int pendingVoterTurnedOut = 0;
        //    int pendingVoterInQueue = 0;
        //    int pendingFinalVotes = 0;
        //    int pendingPollEnded = 0;
        //    int pendingMCEVM = 0;
        //    int pendingPartDeparted = 0;
        //    int pendingPartyCollectOnCentre = 0;
        //    int pendingEVMDeposited = 0;
        //    int pendingInterruption = 0;

        //    //except voterturn out, queue and finl votes

        //    foreach (var boothRecord in soTotalBooths)
        //    {
        //        var electioInfoRecord = await _context.ElectionInfoMaster.FirstOrDefaultAsync(d =>
        //                    d.BoothMasterId == boothRecord.BoothMasterId);
        //        if (electioInfoRecord != null)
        //        {
        //            if (electioInfoRecord.IsPartyDispatched == true)
        //            {
        //                totalPartyDispatched += 1;
        //            }

        //            if (electioInfoRecord.IsPartyReached == true)
        //            {
        //                totalPartyReached += 1;
        //            }
        //            if (electioInfoRecord.IsSetupOfPolling == true)
        //            {
        //                totalIsetUpPolling += 1;
        //            }
        //            if (electioInfoRecord.IsMockPollDone == true)
        //            {
        //                totalmockpoll += 1;
        //            }
        //            if (electioInfoRecord.IsPollStarted == true)
        //            {
        //                totalpollstarted += 1;
        //            }

        //            if (electioInfoRecord.IsVoterTurnOut != null)
        //            {
        //                totalvoterturedout += 1;
        //            }
        //            if (electioInfoRecord.VoterInQueue != null)
        //            {
        //                totalQueue += 1;
        //            }
        //            if (electioInfoRecord.FinalTVoteStatus == true)
        //            {
        //                totalfinalvotes += 1;
        //            }
        //            //if (electioInfoRecord.FinalTVote != null && electioInfoRecord.FinalTVote > 0)
        //            //{
        //            //    totalfinalvotes += 1;
        //            //}
        //            if (electioInfoRecord.IsPollEnded == true)
        //            {
        //                totalpollended += 1;
        //                // totalfinalvotes += 1;
        //            }
        //            if (electioInfoRecord.IsMCESwitchOff == true)
        //            {
        //                totalmcevm += 1;
        //            }
        //            if (electioInfoRecord.IsPartyDeparted == true)
        //            {
        //                totalpartdeparted += 1;
        //            }
        //            if (electioInfoRecord.IsPartyReachedCollectionCenter == true)
        //            {
        //                totalpartycollectoncentre += 1;
        //            }
        //            if (electioInfoRecord.IsEVMDeposited == true)
        //            {
        //                totalevmdeposited += 1;
        //            }

        //        }
        //    }


        //    pendingPartyDispatched = soTotalBooths.Count - totalPartyDispatched;
        //    pendingPartyReached = soTotalBooths.Count - totalPartyReached;
        //    pendingIsetUpPolling = soTotalBooths.Count - totalIsetUpPolling;
        //    pendingMockPoll = soTotalBooths.Count - totalmockpoll;
        //    pendingPollStarted = soTotalBooths.Count - totalpollstarted;
        //    pendingVoterTurnedOut = soTotalBooths.Count - totalvoterturedout;
        //    pendingVoterInQueue = soTotalBooths.Count - totalQueue;
        //    pendingFinalVotes = soTotalBooths.Count - totalfinalvotes;
        //    pendingPollEnded = soTotalBooths.Count - totalpollended;
        //    pendingMCEVM = soTotalBooths.Count - totalmcevm;
        //    pendingPartDeparted = soTotalBooths.Count - totalpartdeparted;
        //    pendingPartyCollectOnCentre = soTotalBooths.Count - totalpartycollectoncentre;
        //    pendingEVMDeposited = soTotalBooths.Count - totalevmdeposited;
        //    var event_lits = await _context.EventMaster.Where(p => p.Status == true).OrderBy(p => p.EventSequence).ToListAsync();
        //    //var event_lits = _context.EventMaster.OrderBy(p => p.EventSequence).ToList();
        //    foreach (var eventid in event_lits)
        //    {
        //        if (eventid.EventMasterId == 1)
        //        {
        //            EventWiseBoothStatus model = new EventWiseBoothStatus();
        //            model.EventMasterId = eventid.EventMasterId;
        //            model.EventName = eventid.EventName;
        //            model.Completed = totalPartyDispatched;
        //            model.Pending = pendingPartyDispatched;
        //            model.TotalBooths = soTotalBooths.Count;
        //            list.Add(model);
        //        }
        //        else if (eventid.EventMasterId == 2)
        //        {
        //            EventWiseBoothStatus model = new EventWiseBoothStatus();
        //            model.EventMasterId = eventid.EventMasterId;
        //            model.EventName = eventid.EventName;
        //            model.Completed = totalPartyReached;
        //            model.Pending = pendingPartyReached;
        //            model.TotalBooths = soTotalBooths.Count;
        //            list.Add(model);
        //        }
        //        else if (eventid.EventMasterId == 3)
        //        {
        //            EventWiseBoothStatus model = new EventWiseBoothStatus();
        //            model.EventMasterId = eventid.EventMasterId;
        //            model.EventName = eventid.EventName;
        //            model.Completed = totalIsetUpPolling;
        //            model.Pending = pendingIsetUpPolling;
        //            model.TotalBooths = soTotalBooths.Count;
        //            list.Add(model);
        //        }
        //        else if (eventid.EventMasterId == 4)
        //        {
        //            EventWiseBoothStatus model = new EventWiseBoothStatus();
        //            model.EventMasterId = eventid.EventMasterId;
        //            model.EventName = eventid.EventName;
        //            model.Completed = totalmockpoll;
        //            model.Pending = pendingMockPoll;
        //            model.TotalBooths = soTotalBooths.Count;
        //            list.Add(model);
        //        }
        //        else if (eventid.EventMasterId == 5)
        //        {
        //            EventWiseBoothStatus model = new EventWiseBoothStatus();
        //            model.EventMasterId = eventid.EventMasterId;
        //            model.EventName = eventid.EventName;
        //            model.Completed = totalpollstarted;
        //            model.Pending = pendingPollStarted;
        //            model.TotalBooths = soTotalBooths.Count;
        //            list.Add(model);
        //        }
        //        else if (eventid.EventMasterId == 6)
        //        {
        //            EventWiseBoothStatus model_turn = new EventWiseBoothStatus();
        //            model_turn.EventMasterId = 6;
        //            model_turn.EventName = eventid.EventName;
        //            model_turn.Completed = totalvoterturedout;
        //            model_turn.Pending = pendingVoterTurnedOut;
        //            model_turn.TotalBooths = soTotalBooths.Count;
        //            list.Add(model_turn);
        //        }
        //        else if (eventid.EventMasterId == 7)
        //        {
        //            EventWiseBoothStatus model_queue = new EventWiseBoothStatus();
        //            model_queue.EventMasterId = 7;
        //            model_queue.EventName = eventid.EventName;
        //            model_queue.Completed = totalQueue;
        //            model_queue.Pending = pendingVoterInQueue;
        //            model_queue.TotalBooths = soTotalBooths.Count;
        //            list.Add(model_queue);
        //        }
        //        else if (eventid.EventMasterId == 8)
        //        {
        //            EventWiseBoothStatus model = new EventWiseBoothStatus();
        //            model.EventMasterId = eventid.EventMasterId;
        //            model.EventName = eventid.EventName;
        //            model.Completed = totalfinalvotes;
        //            model.Pending = pendingFinalVotes;
        //            model.TotalBooths = soTotalBooths.Count;
        //            list.Add(model);
        //        }
        //        else if (eventid.EventMasterId == 9)
        //        {
        //            EventWiseBoothStatus model = new EventWiseBoothStatus();
        //            model.EventMasterId = eventid.EventMasterId;
        //            model.EventName = eventid.EventName;
        //            model.Completed = totalpollended;
        //            model.Pending = pendingPollEnded;
        //            model.TotalBooths = soTotalBooths.Count;
        //            list.Add(model);
        //        }
        //        else if (eventid.EventMasterId == 10)
        //        {
        //            EventWiseBoothStatus model = new EventWiseBoothStatus();
        //            model.EventMasterId = eventid.EventMasterId;
        //            model.EventName = eventid.EventName;
        //            model.Completed = totalmcevm;
        //            model.Pending = pendingMCEVM;
        //            model.TotalBooths = soTotalBooths.Count;
        //            list.Add(model);
        //        }
        //        else if (eventid.EventMasterId == 11)
        //        {
        //            EventWiseBoothStatus model = new EventWiseBoothStatus();
        //            model.EventMasterId = eventid.EventMasterId;
        //            model.EventName = eventid.EventName;
        //            model.Completed = totalpartdeparted;
        //            model.Pending = pendingPartDeparted;
        //            model.TotalBooths = soTotalBooths.Count;
        //            list.Add(model);
        //        }
        //        else if (eventid.EventMasterId == 12)
        //        {
        //            EventWiseBoothStatus model = new EventWiseBoothStatus();
        //            model.EventMasterId = eventid.EventMasterId;
        //            model.EventName = eventid.EventName;
        //            model.Completed = totalpartycollectoncentre;
        //            model.Pending = pendingPartyCollectOnCentre;
        //            model.TotalBooths = soTotalBooths.Count;
        //            list.Add(model);
        //        }
        //        else if (eventid.EventMasterId == 13)
        //        {
        //            EventWiseBoothStatus model = new EventWiseBoothStatus();
        //            model.EventMasterId = eventid.EventMasterId;
        //            model.EventName = eventid.EventName;
        //            model.Completed = totalevmdeposited;
        //            model.Pending = pendingEVMDeposited;
        //            model.TotalBooths = soTotalBooths.Count;
        //            list.Add(model);
        //        }

        //    }



        //    return list;
        //}


        public async Task<List<EventWiseBoothStatus>> EventWiseBoothStatus(string soId)
        {
            var eventWiseBoothStatusList = new List<EventWiseBoothStatus>();

            // Establish a connection to the PostgreSQL database
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("Postgres"));
            await connection.OpenAsync();

            // Create a NpgsqlCommand object to execute the function
            var command = new NpgsqlCommand("SELECT * FROM GetEventWiseBoothStatus(@soId)", connection);
            command.Parameters.AddWithValue("@soId", soId);

            // Execute the command and read the results
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                // Check if any column value is null
                if (!await reader.IsDBNullAsync(0) &&   // Check for EventMasterId
                    !await reader.IsDBNullAsync(1) &&   // Check for EventName
                    !await reader.IsDBNullAsync(2) &&   // Check for Completed
                    !await reader.IsDBNullAsync(3) &&   // Check for Pending
                    !await reader.IsDBNullAsync(4))     // Check for TotalBooths
                {
                    // Create a new EventWiseBoothStatus object and populate its properties from the reader
                    var eventWiseBoothStatus = new EventWiseBoothStatus
                    {
                        EventMasterId = reader.GetInt32(0),
                        EventName = reader.GetString(1),
                        Completed = reader.GetInt32(2),
                        Pending = reader.GetInt32(3),
                        TotalBooths = reader.GetInt32(4)
                    };

                    // Add the object to the list
                    eventWiseBoothStatusList.Add(eventWiseBoothStatus);
                }
            }

            // Return the list of EventWiseBoothStatus objects
            return eventWiseBoothStatusList;

        }


        /// <summary>
        /// on the basis of stateId
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
        //public async Task<List<EventActivityCount>> GetEventListDistrictWiseById(string stateId)
        //{
        //    var allDistricts = await _context.DistrictMaster.Include(d => d.BoothMaster)
        //                                  .Where(d => d.StateMasterId == Convert.ToInt32(stateId))
        //                                  .ToListAsync();


        //    var stateEventList = allDistricts.Select(d => new EventActivityCount
        //    {
        //        Key = GenerateRandomAlphanumericString(6),
        //        MasterId = d.DistrictMasterId,
        //        Name = $"({d.BoothMaster.Count()}) {d.DistrictName}",
        //        Type = "District",
        //        PartyDispatch = 0,
        //        PartyArrived = 0,
        //        SetupPollingStation = 0,
        //        MockPollDone = 0,
        //        PollStarted = 0,
        //        VoterTurnedOut = 0,
        //        VoterTurnOutValue = 0,
        //        FinalVotesValue = 0,
        //        QueueValue = 0,
        //        PollEnded = 0,
        //        MCEVMOff = 0,
        //        PartyDeparted = 0,
        //        PartyReachedAtCollection = 0,
        //        EVMDeposited = 0,
        //        Children = new List<object>()
        //    }).ToList();

        //    var electionInfoByDistrictId = await _context.ElectionInfoMaster
        //        .Where(e => e.StateMasterId == Convert.ToInt32(stateId) && e.IsPartyDispatched == true)
        //        .GroupBy(e => e.DistrictMasterId)
        //        .Select(g => new
        //        {

        //            DistrictMasterId = g.Key,
        //            PartyDispatchCount = g.Sum(e => e.IsPartyDispatched == true ? 1 : 0),
        //            PartyArrivedCount = g.Sum(e => e.IsPartyReached == true ? 1 : 0),
        //            SetupPollingStationCount = g.Sum(e => e.IsSetupOfPolling == true ? 1 : 0),
        //            MockPollDoneCount = g.Sum(e => e.IsMockPollDone == true ? 1 : 0),
        //            PollStartedCount = g.Sum(e => e.IsPollStarted == true ? 1 : 0),
        //            VoterTurnedOutCount = g.Sum(e => e.IsVoterTurnOut == true ? 1 : 0),
        //            FinalVotesValue = g.Sum(e => e.FinalTVote),
        //            QueueValue = g.Sum(e => e.VoterInQueue),
        //            PollEndedCount = g.Sum(e => e.IsPollEnded == true ? 1 : 0),
        //            MCEVMOffCount = g.Sum(e => e.IsMCESwitchOff == true ? 1 : 0),
        //            PartyDepartedCount = g.Sum(e => e.IsPartyDeparted == true ? 1 : 0),
        //            PartyReachedAtCollectionCount = g.Sum(e => e.IsPartyReachedCollectionCenter == true ? 1 : 0),
        //            EVMDepositedCount = g.Sum(e => e.IsEVMDeposited == true ? 1 : 0)
        //        }).ToDictionaryAsync(g => g.DistrictMasterId);

        //    foreach (var districtEvent in stateEventList)
        //    {
        //        if (electionInfoByDistrictId.TryGetValue(districtEvent.MasterId.Value, out var electionInfo))
        //        {
        //            districtEvent.PartyDispatch = electionInfo.PartyDispatchCount;
        //            districtEvent.PartyArrived = electionInfo.PartyArrivedCount;
        //            districtEvent.SetupPollingStation = electionInfo.SetupPollingStationCount;
        //            districtEvent.MockPollDone = electionInfo.MockPollDoneCount;
        //            districtEvent.PollStarted = electionInfo.PollStartedCount;
        //            districtEvent.VoterTurnedOut = electionInfo.VoterTurnedOutCount;
        //            districtEvent.VoterTurnOutValue = await GetPollDetailforDistrictWise(Convert.ToInt32(stateId), districtEvent.MasterId);
        //            districtEvent.FinalVotesValue = electionInfo.FinalVotesValue;
        //            districtEvent.QueueValue = electionInfo.QueueValue;
        //            districtEvent.PollEnded = electionInfo.PollEndedCount;
        //            districtEvent.MCEVMOff = electionInfo.MCEVMOffCount;
        //            districtEvent.PartyDeparted = electionInfo.PartyDepartedCount;
        //            districtEvent.PartyReachedAtCollection = electionInfo.PartyReachedAtCollectionCount;
        //            districtEvent.EVMDeposited = electionInfo.EVMDepositedCount;
        //        }
        //    }

        //    return stateEventList.OrderBy(x => x.Name).ToList();
        //}

        /// <summary>
        /// on the basis of State and District id
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="districtId"></param>
        /// <returns></returns>

        // 3 columns added in below for dashboard.
        //public async Task<List<AssemblyEventActivityCount>> GetEventListAssemblyWiseById(string stateId, string districtId)
        //{
        //    var stateEventList = new List<AssemblyEventActivityCount>();

        //    var allAssemblies = await _context.AssemblyMaster.Include(d => d.BoothMaster)
        //                                       .Where(a => a.DistrictMasterId == Convert.ToInt32(districtId))
        //                                       .ToListAsync();

        //    var getElectionListStateWise = await _context.ElectionInfoMaster
        //                                                .Where(e => e.DistrictMasterId == Convert.ToInt32(districtId) && e.StateMasterId == Convert.ToInt32(stateId))
        //                                                .ToListAsync();

        //    foreach (var assembly in allAssemblies)
        //    {
        //        var assemblyEvents = getElectionListStateWise.Where(e => e.AssemblyMasterId == assembly.AssemblyMasterId);

        //        var stateEvents = new AssemblyEventActivityCount
        //        {
        //            MasterId = assembly.AssemblyMasterId,
        //            StateMasterId = assembly.StateMasterId,
        //            DistrictMasterId = assembly.DistrictMasterId,
        //            Name = $"({assembly.BoothMaster.Count()}){assembly.AssemblyName}",
        //            Type = "Assembly",
        //            PartyDispatch = assemblyEvents.Sum(e => e.IsPartyDispatched.GetValueOrDefault() ? 1 : 0),
        //            PartyArrived = assemblyEvents.Sum(e => e.IsPartyReached.GetValueOrDefault() ? 1 : 0),
        //            SetupPollingStation = assemblyEvents.Sum(e => e.IsSetupOfPolling.GetValueOrDefault() ? 1 : 0),
        //            MockPollDone = assemblyEvents.Sum(e => e.IsMockPollDone.GetValueOrDefault() ? 1 : 0),
        //            PollStarted = assemblyEvents.Sum(e => e.IsPollStarted.GetValueOrDefault() ? 1 : 0),
        //            VoterTurnedOut = assemblyEvents.Sum(e => e.IsVoterTurnOut.GetValueOrDefault() ? 1 : 0),
        //            VoterTurnOutValue = await GetPollDetailById(assembly.AssemblyMasterId, "Assembly"),
        //            FinalVotesValue = await GetFinalVotesById(assembly.AssemblyMasterId, "Assembly"),
        //            QueueValue = await GetQueueById(assembly.AssemblyMasterId, "Assembly"),
        //            PollEnded = assemblyEvents.Sum(e => e.IsPollEnded.GetValueOrDefault() ? 1 : 0),
        //            MCEVMOff = assemblyEvents.Sum(e => e.IsMCESwitchOff.GetValueOrDefault() ? 1 : 0),
        //            PartyDeparted = assemblyEvents.Sum(e => e.IsPartyDeparted.GetValueOrDefault() ? 1 : 0),
        //            PartyReachedAtCollection = assemblyEvents.Sum(e => e.IsPartyReachedCollectionCenter.GetValueOrDefault() ? 1 : 0),
        //            EVMDeposited = assemblyEvents.Sum(e => e.IsEVMDeposited.GetValueOrDefault() ? 1 : 0),
        //            Children = new List<object>()
        //        };

        //        stateEventList.Add(stateEvents);
        //    }

        //    var groupedStateEventList = stateEventList
        //        .GroupBy(e => e.MasterId)
        //        .Select(group => new AssemblyEventActivityCount
        //        {
        //            Key = GenerateRandomAlphanumericString(6),
        //            MasterId = group.Distinct().Select(d => d.MasterId).FirstOrDefault(),
        //            StateMasterId = group.Distinct().Select(d => d.StateMasterId).FirstOrDefault(),
        //            DistrictMasterId = group.Distinct().Select(d => d.DistrictMasterId).FirstOrDefault(),
        //            Name = group.Select(d => d.Name).FirstOrDefault(),
        //            Type = group.Select(d => d.Type).FirstOrDefault(),
        //            PartyDispatch = group.Sum(e => e.PartyDispatch),
        //            PartyArrived = group.Sum(e => e.PartyArrived),
        //            SetupPollingStation = group.Sum(e => e.SetupPollingStation),
        //            MockPollDone = group.Sum(e => e.MockPollDone),
        //            PollStarted = group.Sum(e => e.PollStarted),
        //            VoterTurnedOut = group.Sum(e => e.VoterTurnedOut),
        //            VoterTurnOutValue = group.Sum(e => e.VoterTurnOutValue),
        //            FinalVotesValue = group.Sum(e => e.FinalVotesValue),
        //            QueueValue = group.Sum(e => e.QueueValue),
        //            PollEnded = group.Sum(e => e.PollEnded),
        //            MCEVMOff = group.Sum(e => e.MCEVMOff),
        //            PartyDeparted = group.Sum(e => e.PartyDeparted),
        //            PartyReachedAtCollection = group.Sum(e => e.PartyReachedAtCollection),
        //            EVMDeposited = group.Sum(e => e.EVMDeposited),
        //            Children = new List<object>(),
        //        })
        //        .ToList();

        //    return groupedStateEventList.OrderBy(x => x.Name).ToList();
        //}


        //public async Task<List<AssemblyEventActivityCount>> GetEventListAssemblyWiseByStateId(string stateId)
        //{
        //    var getElectionListStateWise = await _context.ElectionInfoMaster
        //            .Where(d => d.StateMasterId == Convert.ToInt32(stateId))
        //            .ToListAsync();
        //    var stateEventList = new List<AssemblyEventActivityCount>();

        //    foreach (var electionInfo in getElectionListStateWise)
        //    {

        //        var stateEvents = new AssemblyEventActivityCount
        //        {
        //            MasterId = electionInfo.AssemblyMasterId,
        //            StateMasterId = electionInfo.StateMasterId,
        //            DistrictMasterId = electionInfo.DistrictMasterId,
        //            Name = _context.AssemblyMaster
        //                .Where(d => d.AssemblyMasterId == electionInfo.AssemblyMasterId)
        //                .Select(d => d.AssemblyName)
        //                .FirstOrDefault(),
        //            Type = "Assembly",
        //            PartyDispatch = electionInfo.IsPartyDispatched.GetValueOrDefault() ? 1 : 0,
        //            PartyArrived = electionInfo.IsPartyReached.GetValueOrDefault() ? 1 : 0,
        //            SetupPollingStation = electionInfo.IsSetupOfPolling.GetValueOrDefault() ? 1 : 0,
        //            MockPollDone = electionInfo.IsMockPollDone.GetValueOrDefault() ? 1 : 0,
        //            PollStarted = electionInfo.IsPollStarted.GetValueOrDefault() ? 1 : 0,

        //            VoterTurnOutValue = await GetPollDetailById(electionInfo.AssemblyMasterId, "Assembly"),
        //            FinalVotesValue = await GetFinalVotesById(electionInfo.AssemblyMasterId, "Assembly"),
        //            QueueValue = await GetQueueById(electionInfo.AssemblyMasterId, "Assembly"),
        //            PollEnded = electionInfo.IsPollEnded.GetValueOrDefault() ? 1 : 0,
        //            MCEVMOff = electionInfo.IsMCESwitchOff.GetValueOrDefault() ? 1 : 0,
        //            PartyDeparted = electionInfo.IsPartyDeparted.GetValueOrDefault() ? 1 : 0,
        //            PartyReachedAtCollection = electionInfo.IsPartyReachedCollectionCenter.GetValueOrDefault() ? 1 : 0,
        //            EVMDeposited = electionInfo.IsEVMDeposited.GetValueOrDefault() ? 1 : 0,
        //            Children = new List<object>()
        //        };

        //        stateEventList.Add(stateEvents);
        //    }

        //    var groupedStateEventList = stateEventList
        //        .GroupBy(e => e.MasterId)
        //        .Select(group => new AssemblyEventActivityCount
        //        {

        //            Key = GenerateRandomAlphanumericString(6),
        //            MasterId = group.Distinct().Select(d => d.MasterId).FirstOrDefault(),
        //            StateMasterId = group.Distinct().Select(d => d.StateMasterId).FirstOrDefault(),
        //            DistrictMasterId = group.Distinct().Select(d => d.DistrictMasterId).FirstOrDefault(),
        //            Name = group.Select(d => d.Name).FirstOrDefault(),
        //            Type = group.Select(d => d.Type).FirstOrDefault(),
        //            PartyDispatch = group.Sum(e => e.PartyDispatch),
        //            PartyArrived = group.Sum(e => e.PartyArrived),
        //            SetupPollingStation = group.Sum(e => e.SetupPollingStation),
        //            MockPollDone = group.Sum(e => e.MockPollDone),
        //            PollStarted = group.Sum(e => e.PollStarted),

        //            VoterTurnOutValue = group.Sum(e => e.VoterTurnOutValue),
        //            FinalVotesValue = group.Sum(e => e.FinalVotesValue),
        //            QueueValue = group.Sum(e => e.QueueValue),
        //            PollEnded = group.Sum(e => e.PollEnded),
        //            MCEVMOff = group.Sum(e => e.MCEVMOff),
        //            PartyDeparted = group.Sum(e => e.PartyDeparted),
        //            PartyReachedAtCollection = group.Sum(e => e.PartyReachedAtCollection),
        //            EVMDeposited = group.Sum(e => e.EVMDeposited),
        //            Children = new List<object>(),
        //        })
        //        .ToList();

        //    return groupedStateEventList.OrderBy(x => x.Name).ToList();
        //}

        //public async Task<List<EventActivityCount>> GetEventListPCWiseById(string stateId, string userId)
        //{
        //    var eventActivityList = new List<EventActivityCount>();

        //    // Establish a connection to the PostgreSQL database
        //    await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("Postgres"));
        //    await connection.OpenAsync();

        //    var command = new NpgsqlCommand("select * from getdistrictwiseeventlistbyid(@state_master_id)", connection);
        //    command.Parameters.AddWithValue("@state_master_id", Convert.ToInt32(stateId));

        //    // Execute the command and read the results
        //    await using var reader = await command.ExecuteReaderAsync();

        //    while (await reader.ReadAsync())
        //    {
        //        // Create a new EventActivityCount object and populate its properties from the reader
        //        var eventActivityCount = new EventActivityCount
        //        {
        //            Key = GenerateRandomAlphanumericString(6),
        //            MasterId = reader.GetInt32(0),
        //            Name = reader.GetString(1),
        //            Type = "District",
        //            PartyDispatch = reader.IsDBNull(4) ? null : reader.GetString(4),
        //            PartyArrived = reader.IsDBNull(5) ? null : reader.GetString(5),
        //            SetupPollingStation = reader.IsDBNull(6) ? null : reader.GetString(6),
        //            MockPollDone = reader.IsDBNull(7) ? null : reader.GetString(7),
        //            PollStarted = reader.IsDBNull(8) ? null : reader.GetString(8),
        //            PollEnded = reader.IsDBNull(9) ? null : reader.GetString(9),
        //            MCEVMOff = reader.IsDBNull(10) ? null : reader.GetString(10),
        //            PartyDeparted = reader.IsDBNull(11) ? null : reader.GetString(11),
        //            EVMDeposited = reader.IsDBNull(12) ? null : reader.GetString(12),
        //            PartyReachedAtCollection = reader.IsDBNull(13) ? null : reader.GetString(13),
        //            QueueValue = reader.IsDBNull(14) ? null : reader.GetString(14),
        //            FinalVotesValue = reader.IsDBNull(15) ? null : reader.GetString(15),
        //            VoterTurnOutValue = reader.IsDBNull(16) ? null : reader.GetString(16),
        //            Children = new List<object>()
        //        };


        //        // Add the object to the list
        //        eventActivityList.Add(eventActivityCount);
        //    }

        //    return eventActivityList;
        //}



        #region District Based Dashboard Methods
        public async Task<List<EventActivityCount>> GetEventListDistrictWiseById(string stateId)
        {
            var eventActivityList = new List<EventActivityCount>();

            // Establish a connection to the PostgreSQL database
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("Postgres"));
            await connection.OpenAsync();

            var command = new NpgsqlCommand("select * from getdistrictwiseeventlistbyid(@state_master_id)", connection);
            command.Parameters.AddWithValue("@state_master_id", Convert.ToInt32(stateId));

            // Execute the command and read the results
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                // Create a new EventActivityCount object and populate its properties from the reader
                var eventActivityCount = new EventActivityCount
                {
                    Key = GenerateRandomAlphanumericString(6),
                    MasterId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Type = "District",
                    PartyDispatch = reader.IsDBNull(4) ? null : reader.GetString(4),
                    PartyArrived = reader.IsDBNull(5) ? null : reader.GetString(5),
                    SetupPollingStation = reader.IsDBNull(6) ? null : reader.GetString(6),
                    //MockPollDone = reader.IsDBNull(7) ? null : reader.GetString(7)+","+ reader.GetInt32(17),
                    MockPollDone = reader.IsDBNull(7) ? null : reader.GetString(7),
                    PollStarted = reader.IsDBNull(8) ? null : reader.GetString(8),
                    PollEnded = reader.IsDBNull(9) ? null : reader.GetString(9),
                    MCEVMOff = reader.IsDBNull(10) ? null : reader.GetString(10),
                    PartyDeparted = reader.IsDBNull(11) ? null : reader.GetString(11),
                    EVMDeposited = reader.IsDBNull(12) ? null : reader.GetString(12),
                    PartyReachedAtCollection = reader.IsDBNull(13) ? null : reader.GetString(13),
                    QueueValue = reader.IsDBNull(14) ? null : reader.GetString(14),
                    FinalVotesValue = reader.IsDBNull(15) ? null : reader.GetString(15),
                    VoterTurnOutValue = reader.IsDBNull(16) ? null : reader.GetString(16),
                    TotalSo = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                    Children = new List<object>()
                };


                // Add the object to the list
                eventActivityList.Add(eventActivityCount);
            }

            return eventActivityList;
        }
        public async Task<List<AssemblyEventActivityCount>> GetEventListAssemblyWiseById(string stateId, string districtId)
        {
            var eventActivityList = new List<AssemblyEventActivityCount>();

            // Establish a connection to the PostgreSQL database
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("Postgres"));
            await connection.OpenAsync();

            var command = new NpgsqlCommand("SELECT * FROM getassemblywiseeventlistbyid(@state_master_id, @district_master_id)", connection);
            command.Parameters.AddWithValue("@state_master_id", Convert.ToInt32(stateId));
            command.Parameters.AddWithValue("@district_master_id", Convert.ToInt32(districtId));

            // Execute the command and read the results
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                // Create a new AssemblyEventActivityCount object and populate its properties from the reader
                var eventActivityCount = new AssemblyEventActivityCount
                {
                    Key = GenerateRandomAlphanumericString(6), // You need to define this method to generate a random alphanumeric string
                    MasterId = reader.IsDBNull(0) ? (int?)null : reader.GetInt32(0),
                    //Name = reader.IsDBNull(1) ? null : reader.GetString(1),
                    Name = reader.IsDBNull(2) ? ((int?)null).ToString() : reader.GetInt32(2).ToString() + "-" + (reader.IsDBNull(1) ? null : reader.GetString(1)),
                    Type = "Assembly", // Assuming this is the type for assembly
                    StateMasterId = Convert.ToInt32(stateId),
                    DistrictMasterId = Convert.ToInt32(districtId),
                    AssemblyCode = reader.IsDBNull(2) ? (int?)null : reader.GetInt32(2),
                    TotalSoCount = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),
                    PartyDispatch = reader.IsDBNull(4) ? null : reader.GetString(4),
                    PartyArrived = reader.IsDBNull(5) ? null : reader.GetString(5),
                    SetupPollingStation = reader.IsDBNull(6) ? null : reader.GetString(6),
                    MockPollDone = reader.IsDBNull(7) ? null : reader.GetString(7),
                    PollStarted = reader.IsDBNull(8) ? null : reader.GetString(8),
                    PollEnded = reader.IsDBNull(9) ? null : reader.GetString(9),
                    MCEVMOff = reader.IsDBNull(10) ? null : reader.GetString(10),
                    PartyDeparted = reader.IsDBNull(11) ? null : reader.GetString(11),
                    EVMDeposited = reader.IsDBNull(12) ? null : reader.GetString(12),
                    PartyReachedAtCollection = reader.IsDBNull(13) ? null : reader.GetString(13),
                    QueueValue = reader.IsDBNull(14) ? null : reader.GetString(14),
                    FinalVotesValue = reader.IsDBNull(15) ? null : reader.GetString(15),
                    VoterTurnOutValue = reader.IsDBNull(16) ? null : reader.GetString(16),
                    TotalSo = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                    Children = new List<object>()
                };

                // Add the object to the list
                eventActivityList.Add(eventActivityCount);
            }

            return eventActivityList;
        }
        public async Task<List<EventActivityBoothWise>> GetEventListBoothWiseById(string stateId, string districtId, string assemblyId)
        {
            var eventActivityList = new List<EventActivityBoothWise>();

            // Establish a connection to the PostgreSQL database
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("Postgres"));
            await connection.OpenAsync();

            var command = new NpgsqlCommand("SELECT * FROM getboothwiseeventlistbyid(@state_master_id, @district_master_id, @assembly_master_id)", connection);
            command.Parameters.AddWithValue("@state_master_id", Convert.ToInt32(stateId));
            command.Parameters.AddWithValue("@district_master_id", Convert.ToInt32(districtId));
            command.Parameters.AddWithValue("@assembly_master_id", Convert.ToInt32(assemblyId));

            // Execute the command and read the results
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                // Create a new EventActivityBoothWise object and populate its properties from the reader
                var eventActivityBoothWise = new EventActivityBoothWise
                {
                    Key = GenerateRandomAlphanumericString(6), // You need to define this method to generate a random alphanumeric string
                    MasterId = reader.IsDBNull(0) ? (int?)null : reader.GetInt32(0),
                    Name = reader.IsDBNull(1) ? null : reader.GetString(1),
                    Type = "Booth", // Assuming this is the type for booth
                    PartyDispatch = reader.IsDBNull(2) ? (int?)null : reader.GetInt32(2),
                    PartyArrived = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),
                    SetupPollingStation = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4),
                    MockPollDone = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5),
                    PollStarted = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6),
                    PollEnded = reader.IsDBNull(7) ? (int?)null : reader.GetInt32(7),
                    MCEVMOff = reader.IsDBNull(8) ? (int?)null : reader.GetInt32(8),
                    PartyDeparted = reader.IsDBNull(9) ? (int?)null : reader.GetInt32(9),
                    EVMDeposited = reader.IsDBNull(10) ? (int?)null : reader.GetInt32(10),
                    PartyReachedAtCollection = reader.IsDBNull(11) ? (int?)null : reader.GetInt32(11),
                    QueueValue = reader.IsDBNull(12) ? (int?)null : reader.GetInt32(12),
                    FinalVotesValue = reader.IsDBNull(13) ? (int?)null : reader.GetInt32(13),
                    VoterTurnOutValue = reader.IsDBNull(14) ? (int?)null : reader.GetInt32(14),
                    //  AssignedSOId= reader.IsDBNull(15) ? (int?)null : reader.GetInt32(15),
                    AssignedSOName = reader.IsDBNull(16) ? null : reader.GetString(16),
                    AssignedSOMobile = reader.IsDBNull(17) ? null : reader.GetString(17)

                };

                // Add the object to the list
                eventActivityList.Add(eventActivityBoothWise);
            }

            return eventActivityList;
        }


        #endregion

        #region PC based dashboard methods
        /// <summary>
        /// on the basis of stateid and pcid
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="pcId"></param>
        /// <returns></returns>
        public async Task<List<EventActivityCount>> GetEventListPCWiseById(string stateId, string userId)
        {//
            /* var allPCs = await _context.ParliamentConstituencyMaster
                                        .Where(pc => pc.StateMasterId == Convert.ToInt32(stateId))
                                        .ToListAsync();
             //var VoterTurnOutFigure = await GetfromPollDetailById(Convert.ToInt32(stateId), 0, 0, 1);

             var stateEventList = allPCs.Select(pc => new EventActivityCount
             {
                 Key = GenerateRandomAlphanumericString(6),
                 MasterId = pc.PCMasterId,
                 Name = pc.PcName,
                 Type = "Parliament Constituency",
                 PartyDispatch = "0",
                 PartyArrived = "0",
                 SetupPollingStation = "0",
                 MockPollDone = "0",
                 PollStarted = "0",
                 VoterTurnOutValue = "0",
                 FinalVotesValue = "0",
                 QueueValue = "0",
                 PollEnded = "0",
                 MCEVMOff = "0",
                 PartyDeparted = "0",
                 PartyReachedAtCollection = "0",
                 EVMDeposited = "0",
                 Children = new List<object>()
             }).ToList();

             var electionInfoByPCId = await _context.ElectionInfoMaster
                 .Where(e => e.StateMasterId == Convert.ToInt32(stateId) && e.IsPartyDispatched == true)
                 .GroupBy(e => e.PCMasterId)
                 .Select(g => new
                 {
                     PCMasterId = g.Key,
                     PartyDispatchCount = g.Sum(e => e.IsPartyDispatched == true ? 1 : 0),
                     PartyArrivedCount = g.Sum(e => e.IsPartyReached == true ? 1 : 0),
                     SetupPollingStationCount = g.Sum(e => e.IsSetupOfPolling == true ? 1 : 0),
                     MockPollDoneCount = g.Sum(e => e.IsMockPollDone == true ? 1 : 0),
                     PollStartedCount = g.Sum(e => e.IsPollStarted == true ? 1 : 0),
                     VoterTurnedOutCount = g.Sum(e => e.IsVoterTurnOut == true ? 1 : 0),
                     //VoterTurnedOutValue = g.Sum(e => e.IsVoterTurnOut == true ? 1 : 0),
                     FinalVotesValue = g.Sum(e => e.FinalTVote),
                     QueueValue = g.Sum(e => e.VoterInQueue),
                     PollEndedCount = g.Sum(e => e.IsPollEnded == true ? 1 : 0),
                     MCEVMOffCount = g.Sum(e => e.IsMCESwitchOff == true ? 1 : 0),
                     PartyDepartedCount = g.Sum(e => e.IsPartyDeparted == true ? 1 : 0),
                     PartyReachedAtCollectionCount = g.Sum(e => e.IsPartyReachedCollectionCenter == true ? 1 : 0),
                     EVMDepositedCount = g.Sum(e => e.IsEVMDeposited == true ? 1 : 0)
                 }).ToDictionaryAsync(g => g.PCMasterId);

             foreach (var pcEvent in stateEventList)
             {
                 if (electionInfoByPCId.TryGetValue(pcEvent.MasterId.Value, out var electionInfo))
                 {
                     pcEvent.PartyDispatch = electionInfo.PartyDispatchCount.ToString();
                     pcEvent.PartyArrived = electionInfo.PartyArrivedCount.ToString();
                     pcEvent.SetupPollingStation = electionInfo.SetupPollingStationCount.ToString();
                     pcEvent.MockPollDone = electionInfo.MockPollDoneCount.ToString();
                     pcEvent.PollStarted = electionInfo.PollStartedCount.ToString();
                     pcEvent.VoterTurnOutValue = await GetPollDetailforPCWise(Convert.ToInt32(stateId), pcEvent.MasterId);
                     pcEvent.FinalVotesValue = electionInfo.FinalVotesValue.ToString();
                     pcEvent.QueueValue = electionInfo.QueueValue.ToString();
                     pcEvent.PollEnded = electionInfo.PollEndedCount.ToString();
                     pcEvent.MCEVMOff = electionInfo.MCEVMOffCount.ToString();
                     pcEvent.PartyDeparted = electionInfo.PartyDepartedCount.ToString();
                     pcEvent.PartyReachedAtCollection = electionInfo.PartyReachedAtCollectionCount.ToString();
                     pcEvent.EVMDeposited = electionInfo.EVMDepositedCount.ToString();
                 }
             }

             return stateEventList.OrderBy(x => x.Name).ToList();*/


            var eventActivityList = new List<EventActivityCount>();

            // Establish a connection to the PostgreSQL database
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("Postgres"));
            await connection.OpenAsync();

            var command = new NpgsqlCommand("select * from getpcwiseeventlistbyid(@state_master_id)", connection);
            command.Parameters.AddWithValue("@state_master_id", Convert.ToInt32(stateId));

            // Execute the command and read the results
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                // Create a new EventActivityCount object and populate its properties from the reader
                var eventActivityCount = new EventActivityCount
                {
                    Key = GenerateRandomAlphanumericString(6),
                    MasterId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Type = "Parliament Constituency",
                    PartyDispatch = reader.IsDBNull(4) ? null : reader.GetString(4),
                    PartyArrived = reader.IsDBNull(5) ? null : reader.GetString(5),
                    SetupPollingStation = reader.IsDBNull(6) ? null : reader.GetString(6),
                    MockPollDone = reader.IsDBNull(7) ? null : reader.GetString(7),
                    PollStarted = reader.IsDBNull(8) ? null : reader.GetString(8),
                    PollEnded = reader.IsDBNull(9) ? null : reader.GetString(9),
                    MCEVMOff = reader.IsDBNull(10) ? null : reader.GetString(10),
                    PartyDeparted = reader.IsDBNull(11) ? null : reader.GetString(11),
                    EVMDeposited = reader.IsDBNull(12) ? null : reader.GetString(12),
                    PartyReachedAtCollection = reader.IsDBNull(13) ? null : reader.GetString(13),
                    QueueValue = reader.IsDBNull(14) ? null : reader.GetString(14),
                    FinalVotesValue = reader.IsDBNull(15) ? null : reader.GetString(15),
                    VoterTurnOutValue = reader.IsDBNull(16) ? null : reader.GetString(16),
                    TotalSo = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                    Children = new List<object>()
                };


                // Add the object to the list
                eventActivityList.Add(eventActivityCount);
            }

            return eventActivityList;
        }
        public async Task<List<AssemblyEventActivityCountPCWise>> GetEventListAssemblyWiseByPCId(string stateId, string pcId)
        {
            var eventActivityList = new List<AssemblyEventActivityCountPCWise>();

            // Establish a connection to the PostgreSQL database
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("Postgres"));
            await connection.OpenAsync();

            var command = new NpgsqlCommand("SELECT * FROM getpcassemblywiseeventlistbyid(@state_master_id, @pc_master_id)", connection);
            command.Parameters.AddWithValue("@state_master_id", Convert.ToInt32(stateId));
            command.Parameters.AddWithValue("@pc_master_id", Convert.ToInt32(pcId));

            // Execute the command and read the results
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                // Create a new AssemblyEventActivityCount object and populate its properties from the reader
                var eventActivityCount = new AssemblyEventActivityCountPCWise
                {
                    Key = GenerateRandomAlphanumericString(6), // You need to define this method to generate a random alphanumeric string
                    MasterId = reader.IsDBNull(0) ? (int?)null : reader.GetInt32(0),
                    //Name = reader.IsDBNull(1) ? null : reader.GetString(1),
                    Name = reader.IsDBNull(2) ? ((int?)null).ToString() : reader.GetInt32(2).ToString() + "-" + (reader.IsDBNull(1) ? null : reader.GetString(1)),
                    Type = "Assembly", // Assuming this is the type for assembly
                    StateMasterId = Convert.ToInt32(stateId),
                    PCMasterId = Convert.ToInt32(pcId),
                    AssemblyCode = reader.IsDBNull(2) ? (int?)null : reader.GetInt32(2),
                    TotalSoCount = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),
                    PartyDispatch = reader.IsDBNull(4) ? null : reader.GetString(4),
                    PartyArrived = reader.IsDBNull(5) ? null : reader.GetString(5),
                    SetupPollingStation = reader.IsDBNull(6) ? null : reader.GetString(6),
                    MockPollDone = reader.IsDBNull(7) ? null : reader.GetString(7),
                    PollStarted = reader.IsDBNull(8) ? null : reader.GetString(8),
                    PollEnded = reader.IsDBNull(9) ? null : reader.GetString(9),
                    MCEVMOff = reader.IsDBNull(10) ? null : reader.GetString(10),
                    PartyDeparted = reader.IsDBNull(11) ? null : reader.GetString(11),
                    EVMDeposited = reader.IsDBNull(12) ? null : reader.GetString(12),
                    PartyReachedAtCollection = reader.IsDBNull(13) ? null : reader.GetString(13),
                    QueueValue = reader.IsDBNull(14) ? null : reader.GetString(14),
                    FinalVotesValue = reader.IsDBNull(15) ? null : reader.GetString(15),
                    VoterTurnOutValue = reader.IsDBNull(16) ? null : reader.GetString(16),
                    TotalSo = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                    Children = new List<object>()
                };

                // Add the object to the list
                eventActivityList.Add(eventActivityCount);
            }

            return eventActivityList;
        }
        public async Task<List<EventActivityBoothWise>> GetEventListBoothWiseByPCId(string stateId, string pcId, string assemblyId)
        {
            var eventActivityList = new List<EventActivityBoothWise>();

            // Establish a connection to the PostgreSQL database
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("Postgres"));
            await connection.OpenAsync();

            var command = new NpgsqlCommand("SELECT * FROM getboothwiseeventlistbypcid(@state_master_id, @pc_master_id, @assembly_master_id)", connection);
            command.Parameters.AddWithValue("@state_master_id", Convert.ToInt32(stateId));
            command.Parameters.AddWithValue("@pc_master_id", Convert.ToInt32(pcId));
            command.Parameters.AddWithValue("@assembly_master_id", Convert.ToInt32(assemblyId));

            // Execute the command and read the results
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                // Create a new EventActivityBoothWise object and populate its properties from the reader
                var eventActivityBoothWise = new EventActivityBoothWise
                {
                    Key = GenerateRandomAlphanumericString(6), // You need to define this method to generate a random alphanumeric string
                    MasterId = reader.IsDBNull(0) ? (int?)null : reader.GetInt32(0),
                    Name = reader.IsDBNull(1) ? null : reader.GetString(1),
                    Type = "Booth", // Assuming this is the type for booth
                    PartyDispatch = reader.IsDBNull(2) ? (int?)null : reader.GetInt32(2),
                    PartyArrived = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),
                    SetupPollingStation = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4),
                    MockPollDone = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5),
                    PollStarted = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6),
                    PollEnded = reader.IsDBNull(7) ? (int?)null : reader.GetInt32(7),
                    MCEVMOff = reader.IsDBNull(8) ? (int?)null : reader.GetInt32(8),
                    PartyDeparted = reader.IsDBNull(9) ? (int?)null : reader.GetInt32(9),
                    EVMDeposited = reader.IsDBNull(10) ? (int?)null : reader.GetInt32(10),
                    PartyReachedAtCollection = reader.IsDBNull(11) ? (int?)null : reader.GetInt32(11),
                    QueueValue = reader.IsDBNull(12) ? (int?)null : reader.GetInt32(12),
                    FinalVotesValue = reader.IsDBNull(13) ? (int?)null : reader.GetInt32(13),
                    VoterTurnOutValue = reader.IsDBNull(14) ? (int?)null : reader.GetInt32(14),
                    //  AssignedSOId= reader.IsDBNull(15) ? (int?)null : reader.GetInt32(15),
                    AssignedSOName = reader.IsDBNull(16) ? null : reader.GetString(16),
                    AssignedSOMobile = reader.IsDBNull(17) ? null : reader.GetString(17)

                };

                // Add the object to the list
                eventActivityList.Add(eventActivityBoothWise);
            }

            return eventActivityList;
        }

        #endregion





        public async Task<int> GetPollDetailforDistrictWise(int stateId, int? districtId)
        {
            int lastpolldetail = 0;


            var lastpolldetailCount = _context.PollDetails
                                     .Where(d => d.StateMasterId == stateId && d.DistrictMasterId == districtId)
                                     .GroupBy(d => d.BoothMasterId)
                                     .Select(group => new
                                     {
                                         BoothMasterId = group.Key,
                                         TotalVotesPolled = group.OrderByDescending(d => d.VotesPolledRecivedTime).FirstOrDefault().VotesPolled // Summing only the first record of each group
                                     })
                                     .Sum(result => result.TotalVotesPolled);
            if (lastpolldetailCount == null)
            {
                lastpolldetail = 0;
            }
            else
            {
                lastpolldetail = lastpolldetailCount;
            }



            return lastpolldetail;
        }

        public async Task<string> GetPollDetailforPCWise(int stateId, int? pcMasterId)
        {
            int lastpolldetail = 0;


            var lastpolldetailCount = _context.PollDetails
                                     .Where(d => d.StateMasterId == stateId && d.PCMasterId == pcMasterId)
                                     .GroupBy(d => d.BoothMasterId)
                                     .Select(group => new
                                     {
                                         BoothMasterId = group.Key,
                                         TotalVotesPolled = group.OrderByDescending(d => d.VotesPolledRecivedTime).FirstOrDefault().VotesPolled // Summing only the first record of each group
                                     })
                                     .Sum(result => result.TotalVotesPolled);
            if (lastpolldetailCount == null)
            {
                lastpolldetail = 0;
            }
            else
            {
                lastpolldetail = lastpolldetailCount;
            }



            return lastpolldetail.ToString();
        }

        public async Task<int> GetPollDetailById(int assemblyId, string type)
        {
            int lastpolldetail = 0;
            if (assemblyId != 0 && assemblyId != null && type == "Assembly")
            {

                var lastpolldetailCount = _context.PollDetails
                                         .Where(d => d.AssemblyMasterId == assemblyId)
                                         .GroupBy(d => d.BoothMasterId)
                                         .Select(group => new
                                         {
                                             BoothMasterId = group.Key,
                                             TotalVotesPolled = group.OrderByDescending(d => d.VotesPolledRecivedTime).FirstOrDefault().VotesPolled // Summing only the first record of each group
                                         })
                                         .Sum(result => result.TotalVotesPolled);
                if (lastpolldetailCount == null)
                {
                    lastpolldetail = 0;
                }
                else
                {
                    lastpolldetail = lastpolldetailCount;
                }
            }
            else if (assemblyId != 0 && assemblyId != null && type == "District")
            {





            }


            return lastpolldetail;
        }
        public async Task<int> GetFinalVotesById(int keyMasterId, string type)
        {
            int finalVotes = 0;
            if (keyMasterId != 0 && type == "Assembly")
            {
                var finalVotesValue = await _context.ElectionInfoMaster
                    .Where(p => p.AssemblyMasterId == keyMasterId)
                    .Select(p => p.FinalTVote)
                    .FirstOrDefaultAsync();

                finalVotes = finalVotesValue ?? 0;
            }

            return finalVotes;
        }

        public async Task<int> GetQueueById(int keyMasterId, string type)
        {
            int getQueue = 0; // Initialize to a non-nullable default value
            var getQueueValue = await _context.ElectionInfoMaster
                .Where(p => p.AssemblyMasterId == keyMasterId)
                .Select(p => p.VoterInQueue)
                .FirstOrDefaultAsync();

            getQueue = getQueueValue ?? 0; // Use the null-coalescing operator to provide a default value if getQueueValue is null

            return getQueue;
        }


        public async Task<int> TestMethod(int stateId, int districtId, int assemblyId, int pcMasterId)
        {
            int lastpolldetail = 0;
            if (stateId != 0 && stateId != null && districtId == 0 && assemblyId == 0 && pcMasterId == 0)
            {

                var lastpolldetailCount = _context.PollDetails
                                         .Where(d => d.StateMasterId == stateId)
                                         .GroupBy(d => d.BoothMasterId)
                                         .Select(group => new
                                         {
                                             BoothMasterId = group.Key,
                                             TotalVotesPolled = group.OrderByDescending(d => d.VotesPolledRecivedTime).FirstOrDefault().VotesPolled // Summing only the first record of each group
                                         })
                                         .Sum(result => result.TotalVotesPolled);
                if (lastpolldetailCount == null)
                {
                    lastpolldetail = 0;
                }
                else
                {
                    lastpolldetail = lastpolldetailCount;
                }
            }
            else if (stateId != 0 && stateId != null && districtId == 0 && assemblyId == 0 && pcMasterId == 1)
            {





            }


            return lastpolldetail;
        }


        public async Task<int?> GetfromPollDetail(int boothMasterId)
        {
            int? lastpolldetail = 0;
            var lastpolldetailValue = await _context.PollDetails.Where(p => p.BoothMasterId == boothMasterId).OrderByDescending(p => p.VotesPolledRecivedTime).Select(p => p.VotesPolled).FirstOrDefaultAsync();

            if (lastpolldetailValue == null)
            {
                lastpolldetail = 0;
            }
            else
            {
                lastpolldetail = lastpolldetailValue;
            }
            return lastpolldetail;
        }
        public async Task<int?> GetfromFinalVotes(int boothMasterId)
        {
            int? finalVotes = 0;
            var finalVotesValue = _context.ElectionInfoMaster.FirstOrDefault(p => p.BoothMasterId == boothMasterId).FinalTVote;
            if (finalVotesValue == null)
            {
                finalVotes = 0;
            }
            else
            {
                finalVotes = finalVotesValue;
            }
            return finalVotes;
        }
        public async Task<int?> GetfromQueue(int boothMasterId)
        {
            int? getQueue = 0;
            var getQueueValue = _context.ElectionInfoMaster.FirstOrDefault(p => p.BoothMasterId == boothMasterId).VoterInQueue;
            if (getQueueValue == null)
            {
                getQueue = 0;
            }
            else
            {
                getQueue = getQueueValue;
            }
            return getQueue;
        }

        private string GenerateRandomAlphanumericString(int length)
        {
            const string lowercaseChars = "abcdefghijklmnopqrstuvwxyz";
            const string uppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numericChars = "0123456789";

            var random = new Random();
            char[] randomArray = new char[length];

            // Ensure at least one lowercase, one uppercase, and one numeric character
            randomArray[0] = lowercaseChars[random.Next(lowercaseChars.Length)];
            randomArray[1] = uppercaseChars[random.Next(uppercaseChars.Length)];
            randomArray[2] = numericChars[random.Next(numericChars.Length)];

            // Fill the rest of the array with random characters
            for (int i = 3; i < length; i++)
            {
                string allChars = lowercaseChars + uppercaseChars + numericChars;
                randomArray[i] = allChars[random.Next(allChars.Length)];
            }

            // Shuffle the array to randomize the positions of the characters
            for (int i = length - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);
                char temp = randomArray[i];
                randomArray[i] = randomArray[j];
                randomArray[j] = temp;
            }

            return new string(randomArray);
        }


        #endregion

        #region SendDashBoardCount 
        public async Task<List<DashboardConnectedUser>> DashboardConnectedUser(DahboardMastersId dashboardMastersId, string roleType)
        {
            string aroType = "ARO";
            string districtPCType = "DistrictPC";
            string stateAdminType = "StateAdmin";
            DateTime today = DateTime.UtcNow.Date; // Get today's date

            if (roleType == aroType)
            {
                var aroRecords = _context.DashboardConnectedUser
                    .Where(d => d.StateMasterId == Convert.ToInt32(dashboardMastersId.StateMasterId) &&
                                d.DistrictMasterId == Convert.ToInt32(dashboardMastersId.DistrictMasterId) &&
                                d.AssemblyMasterId == Convert.ToInt32(dashboardMastersId.AssemblyMasterId) &&
                                d.Role == roleType &&
                                d.UserConnectedTime.HasValue &&
                                d.UserConnectedTime.Value.Date == today)
                    .ToList();

                return aroRecords;
            }
            else if (roleType == districtPCType)
            {
                var districtPCWiseRecords = _context.DashboardConnectedUser
                    .Where(d => d.Role == "DistrictAdmin" &&
                                d.StateMasterId == Convert.ToInt32(dashboardMastersId.StateMasterId) &&
                                d.DistrictMasterId == Convert.ToInt32(dashboardMastersId.DistrictMasterId) &&
                                d.UserConnectedTime.HasValue &&
                                d.UserConnectedTime.Value.Date == today)
                    .ToList();
                return districtPCWiseRecords;
            }
            else if (roleType == stateAdminType)
            {
                var stateRecords = _context.DashboardConnectedUser
                    .Where(d => d.StateMasterId == Convert.ToInt32(dashboardMastersId.StateMasterId) &&
                                (d.Role == "StateAdmin" || d.Role == "SuperAdmin" || d.Role == "ECI") && d.UserConnectedTime.Value.Date == today.Date)
                    .ToList();
                return stateRecords;
            }
            else
            {
                return null;
            }

        }
        //#region GetDasboard Count
        //public async Task<DashBoardRealTimeCount> GetDashBoardCount(ClaimsIdentity claimsIdentity)
        //{
        //    var rolesClaim = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
        //    var roles = rolesClaim?.Value;

        //    if (IsSuperAdminOrECI(roles))
        //    {
        //        return await GetDashboardCountForSuperAdminOrECI(roles, claimsIdentity);
        //    }
        //    else if (IsStateAdmin(roles))
        //    {
        //        return await GetDashboardCountForStateAdmin(roles, claimsIdentity);

        //    }
        //    else
        //    {
        //        return await GetDashboardCountForUserRole(claimsIdentity);
        //    }
        //}

        //private bool IsSuperAdminOrECI(string roles)
        //{
        //    return roles == "SuperAdmin" || roles == "ECI";
        //}
        //private bool IsStateAdmin(string roles)
        //{
        //    return roles == "StateAdmin";
        //}

        //private async Task<DashBoardRealTimeCount> GetDashboardCountForSuperAdminOrECI(string roles, ClaimsIdentity claimsIdentity)
        //{
        //    var electionInfoList = await _context.ElectionInfoMaster.ToListAsync();
        //    var stateMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "StateMasterId")?.Value;

        //    int totalBothCount = await _context.BoothMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId)).CountAsync();

        //    return BuildDashboardCount(electionInfoList, totalBothCount, claimsIdentity);
        //}
        //private async Task<DashBoardRealTimeCount> GetDashboardCountForStateAdmin(string roles, ClaimsIdentity claimsIdentity)
        //{
        //    var stateMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "StateMasterId")?.Value;

        //    var electionInfoList = await _context.ElectionInfoMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId)).ToListAsync();
        //    int totalBothCount = await _context.BoothMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId)).CountAsync();

        //    return BuildDashboardCount(electionInfoList, totalBothCount, claimsIdentity);
        //}

        //private async Task<DashBoardRealTimeCount> GetDashboardCountForUserRole(ClaimsIdentity claimsIdentity)
        //{
        //    // Extract necessary claims
        //    var stateMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "StateMasterId")?.Value;
        //    var assemblyMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "AssemblyMasterId")?.Value;
        //    var districtMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "DistrictMasterId")?.Value;
        //    var pcMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "PCMasterId")?.Value;
        //    var rolesClaim = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
        //    var roles = rolesClaim?.Value;
        //    // Use the extracted claims to filter records
        //    var electionInfoList = await GetFilteredElectionInfoList(stateMasterId, assemblyMasterId, districtMasterId, pcMasterId);
        //    int totalBothCount = await GetFilteredBoothCount(stateMasterId, assemblyMasterId, districtMasterId, pcMasterId);

        //    return BuildDashboardCount(electionInfoList, totalBothCount, claimsIdentity);
        //}

        //private async Task<List<ElectionInfoMaster>> GetFilteredElectionInfoList(string stateMasterId, string assemblyMasterId, string districtMasterId, string pcMasterId)
        //{
        //    if (assemblyMasterId is not ("0") && districtMasterId is not ("0"))
        //    {
        //        return await _context.ElectionInfoMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId)).ToListAsync();
        //    }
        //    else if (districtMasterId is not ("0"))
        //    {
        //        return await _context.ElectionInfoMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId)).ToListAsync();

        //    }
        //    else if (pcMasterId is not ("0"))
        //    {
        //        return await _context.ElectionInfoMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.PCMasterId == Convert.ToInt32(pcMasterId)).ToListAsync();
        //    }
        //    else if (assemblyMasterId is not ("0") && pcMasterId is not ("0"))
        //    {
        //        return await _context.ElectionInfoMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.PCMasterId == Convert.ToInt32(pcMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId)).ToListAsync();
        //    }
        //    else
        //    {
        //        return null;
        //    }

        //}
        //private async Task<int> GetFilteredBoothCount(string stateMasterId, string assemblyMasterId, string districtMasterId, string pcMasterId)
        //{

        //    if (assemblyMasterId is not ("0") && districtMasterId is not ("0"))
        //    {
        //        return await _context.BoothMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId)).CountAsync();
        //    }
        //    else if (districtMasterId is not ("0"))
        //    {
        //        return await _context.BoothMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId)).CountAsync();

        //    }
        //    else if (pcMasterId is not ("0"))
        //    {
        //        return await _context.BoothMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.AssemblyMaster.PCMasterId == Convert.ToInt32(pcMasterId)).CountAsync();
        //    }
        //    else if (assemblyMasterId is not ("0") && pcMasterId is not ("0"))
        //    {
        //        return await _context.BoothMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.AssemblyMaster.PCMasterId == Convert.ToInt32(pcMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId)).CountAsync();
        //    }
        //    else
        //    {
        //        return 0;
        //    }

        //}

        //private DashBoardRealTimeCount BuildDashboardCount(List<ElectionInfoMaster> electionInfoList, int totalBothCount, ClaimsIdentity claimsIdentity)
        //{
        //    var dashboardCount = new DashBoardRealTimeCount
        //    {
        //        Total = totalBothCount,
        //        Events = new List<EventCount>()
        //    };

        //    AddEventCount(dashboardCount, "PartyDispatch", e => e.IsPartyDispatched == true, electionInfoList);
        //    AddEventCount(dashboardCount, "PartyArrived", e => e.IsPartyReached == true, electionInfoList);
        //    AddEventCount(dashboardCount, "SetupPollingStation", e => e.IsSetupOfPolling == true, electionInfoList);
        //    AddEventCount(dashboardCount, "MockPollDone", e => e.IsMockPollDone == true, electionInfoList);
        //    AddEventCount(dashboardCount, "PollStarted", e => e.IsPollStarted == true, electionInfoList);
        //    AddVotePolledCount(dashboardCount, "VotesPolled", electionInfoList, claimsIdentity);
        //    AddQueueCount(dashboardCount, "VoterInQueue", electionInfoList, claimsIdentity);
        //    //AddEventCount(dashboardCount, "FinalVoteDone", e => e.FinalTVoteStatus == true, electionInfoList);
        //    AddFinalVotesCount(dashboardCount, "FinalVoteDone", electionInfoList, claimsIdentity);
        //    AddEventCount(dashboardCount, "PollEnded", e => e.IsPollEnded == true, electionInfoList);
        //    AddEventCount(dashboardCount, "EVMVVPATOff", e => e.IsMCESwitchOff == true, electionInfoList);
        //    AddEventCount(dashboardCount, "PartyDeparted", e => e.IsPartyDeparted == true, electionInfoList);
        //    AddEventCount(dashboardCount, "PartyReachedAtCollection", e => e.IsPartyReachedCollectionCenter == true, electionInfoList);
        //    AddEventCount(dashboardCount, "EVMDeposited", e => e.IsEVMDeposited == true, electionInfoList);
        //    return dashboardCount;
        //}

        //private void AddEventCount(DashBoardRealTimeCount dashboardCount, string eventName, Func<ElectionInfoMaster, bool> condition, List<ElectionInfoMaster> electionInfoList)
        //{
        //    var count = electionInfoList.Count(condition);
        //    dashboardCount.Events.Add(new EventCount { EventName = eventName, Count = count });
        //}
        //private async void AddVotePolledCount(DashBoardRealTimeCount dashboardCount, string eventName, List<ElectionInfoMaster> electionInfoList, ClaimsIdentity claimsIdentity)
        //{
        //    var stateMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "StateMasterId")?.Value;
        //    var assemblyMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "AssemblyMasterId")?.Value;
        //    var districtMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "DistrictMasterId")?.Value;
        //    var pcMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "PCMasterId")?.Value;
        //    var rolesClaim = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
        //    var roles = rolesClaim?.Value;
        //    var votesPolledCounts = new List<int?>();
        //    var totalVotesCounts = new List<int?>();
        //    var votesPolledPercentage = new List<int?>();
        //    var finalVotesCount = new List<int?>();


        //    if (roles == "SuperAdmin" || roles == "ECI" || roles == "StateAdmin")
        //    {
        //        var pollDetailCount = _context.PollDetails
        //                             .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId))
        //                             .GroupBy(d => d.BoothMasterId)
        //                             .Select(group => new
        //                             {
        //                                 BoothMasterId = group.Key,
        //                                 TotalVotesPolled = group.OrderByDescending(d => d.VotesPolledRecivedTime).FirstOrDefault().VotesPolled // Summing only the first record of each group
        //                             })
        //                             .Sum(result => result.TotalVotesPolled);


        //        var totalVotersCount = _context.BoothMaster
        //            .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId))
        //            .Sum(d => d.TotalVoters);

        //        votesPolledCounts.Add(pollDetailCount);
        //        totalVotesCounts.Add(totalVotersCount);
        //    }
        //    else if (roles == "DistrictAdmin")
        //    {
        //        var pollDetailCount = _context.PollDetails
        //            .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId))
        //            .GroupBy(d => d.BoothMasterId)
        //                             .Select(group => new
        //                             {
        //                                 BoothMasterId = group.Key,
        //                                 TotalVotesPolled = group.OrderByDescending(d => d.VotesPolledRecivedTime).FirstOrDefault().VotesPolled // Summing only the first record of each group
        //                             })
        //                             .Sum(result => result.TotalVotesPolled);
        //        var totalVotersCount = _context.BoothMaster
        //            .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId))
        //            .Sum(d => d.TotalVoters);

        //        var finalVotes = _context.ElectionInfoMaster.Where(d => d.FinalTVoteStatus == true && d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId)).Sum(d => d.FinalTVote);

        //        finalVotesCount.Add(finalVotes);
        //        votesPolledCounts.Add(pollDetailCount);
        //        totalVotesCounts.Add(totalVotersCount);
        //    }
        //    else if (roles == "PC")
        //    {
        //        List<int?> totalVotersCounts = new List<int?>();
        //        var pollDetailCount = _context.PollDetails
        //            .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.PCMasterId == Convert.ToInt32(pcMasterId))
        //           .OrderByDescending(d => d.VotesPolledRecivedTime)
        //                             .GroupBy(d => d.BoothMasterId)
        //                             .Select(group => new
        //                             {
        //                                 BoothMasterId = group.Key,
        //                                 TotalVotesPolled = group.FirstOrDefault().VotesPolled // Summing only the first record of each group
        //                             })
        //                             .Sum(result => result.TotalVotesPolled);

        //        var AssemblyList = _context.AssemblyMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId)).Select(p => p.PCMasterId).ToList();
        //        foreach (var asmId in AssemblyList)
        //        {
        //            var totalVotersCount = _context.BoothMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.AssemblyMasterId == asmId)
        //           .Sum(d => d.TotalVoters);
        //            totalVotersCounts.Add(totalVotersCount);
        //        }



        //        var finalVotes = _context.ElectionInfoMaster.Where(d => d.FinalTVoteStatus == true && d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId)).Sum(d => d.FinalTVote);

        //        finalVotesCount.Add(finalVotes);

        //        votesPolledCounts.Add(pollDetailCount);
        //        totalVotesCounts.Add(totalVotersCounts.Sum());
        //    }

        //    else if (roles == "ARO")
        //    {
        //        var pollDetailCount = _context.PollDetails
        //            .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId))
        //            .GroupBy(d => d.BoothMasterId)
        //                             .Select(group => new
        //                             {
        //                                 BoothMasterId = group.Key,
        //                                 TotalVotesPolled = group.OrderByDescending(d => d.VotesPolledRecivedTime).FirstOrDefault().VotesPolled // Summing only the first record of each group
        //                             })
        //                             .Sum(result => result.TotalVotesPolled);

        //        var totalVotersCount = _context.BoothMaster
        //            .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId))
        //            .Sum(d => d.TotalVoters);
        //        votesPolledCounts.Add(pollDetailCount);
        //        totalVotesCounts.Add(totalVotersCount);
        //    }

        //    dashboardCount.Events.Add(new EventCount
        //    {
        //        EventName = eventName,
        //        VotesPolledPercentage = Math.Round((decimal)(votesPolledCounts.FirstOrDefault() * 100.0 / totalVotesCounts.FirstOrDefault()), 1),
        //        VotesPolledCount = votesPolledCounts.Sum(),
        //        TotalVotersCount = totalVotesCounts.Sum(),

        //    });
        //}


        //private async void AddQueueCount(DashBoardRealTimeCount dashboardCount, string eventName, List<ElectionInfoMaster> electionInfoList, ClaimsIdentity claimsIdentity)
        //{
        //    var stateMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "StateMasterId")?.Value;
        //    var assemblyMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "AssemblyMasterId")?.Value;
        //    var districtMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "DistrictMasterId")?.Value;
        //    var pcMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "PCMasterId")?.Value;
        //    var rolesClaim = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
        //    var roles = rolesClaim?.Value;

        //    var queueVotesCount = new List<int?>();

        //    //  var votesPolledResult = await GetVotesPolledPercentage(claimsIdentity);

        //    if (roles == "SuperAdmin" || roles == "ECI" || roles == "StateAdmin")
        //    {

        //        var queueVotes = _context.ElectionInfoMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId)).Sum(d => d.VoterInQueue);

        //        queueVotesCount.Add(queueVotes);

        //    }
        //    else if (roles == "DistrictAdmin")
        //    {
        //        var queueVotes = _context.ElectionInfoMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId)).Sum(d => d.VoterInQueue);
        //        queueVotesCount.Add(queueVotes);

        //    }
        //    else if (roles == "PC")
        //    {

        //        var queueVotes = _context.ElectionInfoMaster.Where(d => d.PCMasterId == Convert.ToInt32(pcMasterId) && d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId)).Sum(d => d.VoterInQueue);
        //        queueVotesCount.Add(queueVotes);
        //    }

        //    else if (roles == "ARO")
        //    {

        //        var queueVotes = _context.ElectionInfoMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId)).Sum(d => d.VoterInQueue);

        //        queueVotesCount.Add(queueVotes);
        //    }

        //    dashboardCount.Events.Add(new EventCount
        //    {
        //        EventName = eventName,
        //        FinalVotesCount = queueVotesCount.Sum()

        //    });
        //}

        //private async void AddFinalVotesCount(DashBoardRealTimeCount dashboardCount, string eventName, List<ElectionInfoMaster> electionInfoList, ClaimsIdentity claimsIdentity)
        //{
        //    var stateMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "StateMasterId")?.Value;
        //    var assemblyMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "AssemblyMasterId")?.Value;
        //    var districtMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "DistrictMasterId")?.Value;
        //    var pcMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "PCMasterId")?.Value;
        //    var rolesClaim = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
        //    var roles = rolesClaim?.Value;
        //    var votesPolledCounts = new List<int?>();
        //    var totalVotesCounts = new List<int?>();
        //    var votesPolledPercentage = new List<int?>();
        //    var finalVotesCount = new List<int?>();

        //    //  var votesPolledResult = await GetVotesPolledPercentage(claimsIdentity);

        //    if (roles == "SuperAdmin" || roles == "ECI" || roles == "StateAdmin")
        //    {
        //        var totalVotersCount = _context.BoothMaster
        //            .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId))
        //            .Sum(d => d.TotalVoters);
        //        var finalVotes = _context.ElectionInfoMaster.Where(d => d.FinalTVoteStatus == true && d.StateMasterId == Convert.ToInt32(stateMasterId)).Sum(d => d.FinalTVote);
        //        finalVotesCount.Add(finalVotes);
        //        totalVotesCounts.Add(totalVotersCount);
        //    }
        //    else if (roles == "DistrictAdmin")
        //    {

        //        var totalVotersCount = _context.BoothMaster
        //            .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId))
        //            .Sum(d => d.TotalVoters);

        //        var finalVotes = _context.ElectionInfoMaster.Where(d => d.FinalTVoteStatus == true && d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId)).Sum(d => d.FinalTVote);

        //        finalVotesCount.Add(finalVotes);

        //        totalVotesCounts.Add(totalVotersCount);
        //    }
        //    else if (roles == "PC")
        //    {
        //        List<int?> totalVotersCounts = new List<int?>();
        //        var AssemblyList = _context.AssemblyMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId)).Select(p => p.PCMasterId).ToList();
        //        foreach (var asmId in AssemblyList)
        //        {
        //            var totalVotersCount = _context.BoothMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.AssemblyMasterId == asmId)
        //           .Sum(d => d.TotalVoters);
        //            totalVotersCounts.Add(totalVotersCount);
        //        }


        //        var finalVotes = _context.ElectionInfoMaster.Where(d => d.FinalTVoteStatus == true && d.StateMasterId == Convert.ToInt32(stateMasterId) && d.PCMasterId == Convert.ToInt32(pcMasterId)).Sum(d => d.FinalTVote);

        //        finalVotesCount.Add(finalVotes);

        //        totalVotesCounts.Add(totalVotersCounts.Sum());
        //    }

        //    else if (roles == "ARO")
        //    {

        //        var totalVotersCount = _context.BoothMaster
        //            .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId))
        //            .Sum(d => d.TotalVoters);

        //        var finalVotes = _context.ElectionInfoMaster.Where(d => d.FinalTVoteStatus == true && d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId)).Sum(d => d.FinalTVote);

        //        finalVotesCount.Add(finalVotes);

        //        totalVotesCounts.Add(totalVotersCount);
        //    }

        //    dashboardCount.Events.Add(new EventCount
        //    {
        //        EventName = eventName,

        //        TotalVotersCount = totalVotesCounts.Sum(),
        //        FinalVotesCount = finalVotesCount.Sum(),
        //        FinalVotesPercentage = Math.Round((decimal)(finalVotesCount.FirstOrDefault() * 100.0 / totalVotesCounts.FirstOrDefault()), 1),
        //    });
        //}
        //#endregion

        #endregion

        #region SlotManagement
        public async Task<Response> AddEventSlot(List<SlotManagementMaster> slotManagement)
        {
            var stateMasterIds = slotManagement.Select(d => new { d.StateMasterId, d.EventMasterId }).FirstOrDefault();
            var deleteRecord = _context.SlotManagementMaster
                .Where(d => d.StateMasterId == stateMasterIds.StateMasterId && d.EventMasterId == stateMasterIds.EventMasterId)
                .ToList();
            if (deleteRecord != null)
            {
                _context.SlotManagementMaster.RemoveRange(deleteRecord);
            }

            _context.SlotManagementMaster.AddRange(slotManagement);
            _context.SaveChanges();

            return new Response()
            {
                Status = RequestStatusEnum.OK,
                Message = $"Slot Added Successfully"

            };
        }

        public async Task<List<SlotManagementMaster>> GetEventSlotList(int stateMasterId, int eventId)
        {
            var slotList = await _context.SlotManagementMaster.Where(d => d.StateMasterId == stateMasterId && d.EventMasterId == eventId).ToListAsync();
            return slotList;
        }
        #endregion

        #region UserList
        public async Task<List<UserList>> GetUserList(string soName, string type)
        {
            var users = await _context.SectorOfficerMaster
            .Where(u => EF.Functions.Like(u.SoName.ToUpper(), "%" + soName.ToUpper() + "%"))
            .OrderBy(u => u.SOMasterId)
            .Select(d => new UserList
            {
                Name = d.SoName,
                MobileNumber = d.SoMobile,
                UserType = type
            })
            .ToListAsync();
            return users;
        }

        #endregion

        #region PollInterruption Interruption
        public async Task<Response> AddPollInterruption(PollInterruption PollInterruptionData)
        {
            _context.PollInterruptions.Add(PollInterruptionData);
            _context.SaveChanges();
            return new Response { Status = RequestStatusEnum.OK, Message = "Poll Interruption Added Successfully." };

        }
        public async Task<PollInterruption> GetPollInterruptionData(string boothMasterId)
        {
            var pollInterruptionRecord = await _context.PollInterruptions.Where(d => d.BoothMasterId == Convert.ToInt32(boothMasterId)).OrderByDescending(p => p.PollInterruptionId).FirstOrDefaultAsync();
            return pollInterruptionRecord;
        }
        public async Task<List<PollInterruptionHistoryModel>> GetPollInterruptionHistoryById(string boothMasterId)
        {
            var query = from pollInterruption in _context.PollInterruptions
                        join assemblyMaster in _context.AssemblyMaster on pollInterruption.AssemblyMasterId equals assemblyMaster.AssemblyMasterId
                        join boothMaster in _context.BoothMaster on new { BoothMasterId = pollInterruption.BoothMasterId, AssemblyMasterId = assemblyMaster.AssemblyMasterId } equals new { BoothMasterId = boothMaster.BoothMasterId, AssemblyMasterId = boothMaster.AssemblyMasterId }
                        where boothMaster.BoothMasterId == Convert.ToInt32(boothMasterId)
                        orderby pollInterruption.CreatedAt descending
                        select new
                        {
                            pollInterruption.PollInterruptionId,
                            pollInterruption.BoothMasterId,
                            boothMaster.BoothName,
                            boothMaster.StateMasterId,
                            boothMaster.DistrictMasterId,
                            pollInterruption.PCMasterId,

                            pollInterruption.OldBU,
                            pollInterruption.OldCU,
                            pollInterruption.NewCU,
                            pollInterruption.NewBU,

                            pollInterruption.IsPollInterrupted,
                            pollInterruption.InterruptionType,
                            pollInterruption.StopTime,
                            pollInterruption.ResumeTime,
                            pollInterruption.CreatedAt,
                            pollInterruption.Remarks
                        };

            var result = await query.ToListAsync();

            return result.Select(p => new PollInterruptionHistoryModel
            {
                PollInterruptionId = p.PollInterruptionId,
                BoothMasterId = p.BoothMasterId,
                BoothName = p.BoothName,
                StateMasterId = p.StateMasterId,
                DistrictMasterId = p.DistrictMasterId,
                PCMasterId = p.PCMasterId,
                IsPollInterrupted = p.IsPollInterrupted,
                InterruptionReason = Enum.GetName(typeof(InterruptionReason), p.InterruptionType),
                StopTime = p.StopTime,
                ResumeTime = p.ResumeTime,
                OldCU = p.OldCU,
                NewCU = p.NewCU,
                OldBU = p.OldBU,
                NewBU = p.NewBU,
                CreatedAt = p.CreatedAt,
                Remarks = p.Remarks
                // Add other properties as needed
            }).ToList();
        }
        public async Task<List<PollInterruptionDashboard>> GetPollInterruptionDashboard(ClaimsIdentity claimsIdentity)
        {
            List<PollInterruptionDashboard> finalResult = new List<PollInterruptionDashboard>();
            IQueryable<PollInterruptionDashboard> result = null;
            if (claimsIdentity != null)
            {
                Claim stateMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "StateMasterId");
                Claim districtMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "DistrictMasterId");
                Claim assemblyMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "AssemblyMasterId");
                Claim pcMasterid = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "PCMasterId");

                string sid = stateMasterId.Value; string aid = assemblyMasterId.Value; string did = districtMasterId.Value; string pcid = "";
                if (pcMasterid != null)
                {
                    pcid = pcMasterid.Value;
                }
                //Claim role = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "roles");
                var roles = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
                //var roles = claimsIdentity.Claims(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
                if (roles == "ARO" || roles == "SubARO")
                {
                    result = from pi in _context.PollInterruptions
                             join di in _context.DistrictMaster on pi.DistrictMasterId equals di.DistrictMasterId
                             join am in _context.AssemblyMaster on pi.AssemblyMasterId equals am.AssemblyMasterId
                             join bm in _context.BoothMaster on new { pi.BoothMasterId, am.AssemblyMasterId } equals new { bm.BoothMasterId, bm.AssemblyMasterId }
                             where pi.StateMasterId == Convert.ToInt16(sid) && pi.AssemblyMasterId == Convert.ToInt16(aid)
                             orderby pi.AssemblyMasterId, pi.BoothMasterId, pi.CreatedAt descending
                             select new PollInterruptionDashboard
                             {
                                 PollInterruptionMasterId = pi.PollInterruptionId,
                                 StateMasterId = pi.StateMasterId,
                                 DistrictMasterId = pi.DistrictMasterId,
                                 AssemblyMasterId = pi.AssemblyMasterId,
                                 AssemblyName = am.AssemblyName,
                                 BoothMasterId = bm.BoothMasterId,
                                 PCMasterId = pi.PCMasterId,
                                 BoothName = bm.BoothName + "(" + bm.BoothCode_No + ")",
                                 CreatedAt = pi.CreatedAt,
                                 InterruptionType = pi.InterruptionType,
                                 StopTime = pi.StopTime,
                                 ResumeTime = pi.ResumeTime,
                                 isPollInterrupted = pi.IsPollInterrupted

                             } into distinctResult
                             group distinctResult by new { distinctResult.AssemblyMasterId, distinctResult.BoothMasterId } into groupedResult
                             select groupedResult.OrderByDescending(r => r.CreatedAt).First();

                }

                else if (roles == "StateAdmin")
                {
                    result = from pi in _context.PollInterruptions
                             join di in _context.DistrictMaster on pi.DistrictMasterId equals di.DistrictMasterId
                             join am in _context.AssemblyMaster on pi.AssemblyMasterId equals am.AssemblyMasterId
                             join bm in _context.BoothMaster on new { pi.BoothMasterId, am.AssemblyMasterId } equals new { bm.BoothMasterId, bm.AssemblyMasterId }
                             where pi.StateMasterId == Convert.ToInt16(sid)
                             orderby pi.AssemblyMasterId, pi.BoothMasterId, pi.CreatedAt descending
                             select new PollInterruptionDashboard
                             {
                                 PollInterruptionMasterId = pi.PollInterruptionId,
                                 StateMasterId = pi.StateMasterId,
                                 DistrictMasterId = pi.DistrictMasterId,
                                 AssemblyMasterId = pi.AssemblyMasterId,
                                 AssemblyName = am.AssemblyName,
                                 BoothMasterId = bm.BoothMasterId,
                                 PCMasterId = pi.PCMasterId,
                                 BoothName = bm.BoothName + "(" + bm.BoothCode_No + ")",
                                 CreatedAt = pi.CreatedAt,
                                 InterruptionType = pi.InterruptionType,
                                 StopTime = pi.StopTime,
                                 ResumeTime = pi.ResumeTime,
                                 isPollInterrupted = pi.IsPollInterrupted

                             } into distinctResult
                             group distinctResult by new { distinctResult.AssemblyMasterId, distinctResult.BoothMasterId } into groupedResult
                             select groupedResult.OrderByDescending(r => r.CreatedAt).First();

                }

                else if (roles == "ECI" || roles == "SuperAdmin")
                {
                    result = from pi in _context.PollInterruptions
                             join di in _context.DistrictMaster on pi.DistrictMasterId equals di.DistrictMasterId
                             join am in _context.AssemblyMaster on pi.AssemblyMasterId equals am.AssemblyMasterId
                             join bm in _context.BoothMaster on new { pi.BoothMasterId, am.AssemblyMasterId } equals new { bm.BoothMasterId, bm.AssemblyMasterId }
                             //  where pi.StateMasterId == Convert.ToInt16(stateMasterId.Value)
                             orderby pi.AssemblyMasterId, pi.BoothMasterId, pi.CreatedAt descending
                             select new PollInterruptionDashboard
                             {
                                 PollInterruptionMasterId = pi.PollInterruptionId,
                                 StateMasterId = pi.StateMasterId,
                                 DistrictMasterId = pi.DistrictMasterId,
                                 AssemblyMasterId = pi.AssemblyMasterId,
                                 AssemblyName = am.AssemblyName,
                                 BoothMasterId = bm.BoothMasterId,
                                 PCMasterId = pi.PCMasterId,
                                 BoothName = bm.BoothName + "(" + bm.BoothCode_No + ")",
                                 CreatedAt = pi.CreatedAt,
                                 InterruptionType = pi.InterruptionType,
                                 StopTime = pi.StopTime,
                                 ResumeTime = pi.ResumeTime,
                                 isPollInterrupted = pi.IsPollInterrupted

                             } into distinctResult
                             group distinctResult by new { distinctResult.AssemblyMasterId, distinctResult.BoothMasterId } into groupedResult
                             select groupedResult.OrderByDescending(r => r.CreatedAt).First();

                }
                else if (roles == "DistrictAdmin")
                {
                    result = from pi in _context.PollInterruptions
                             join di in _context.DistrictMaster on pi.DistrictMasterId equals di.DistrictMasterId
                             join am in _context.AssemblyMaster on pi.AssemblyMasterId equals am.AssemblyMasterId
                             join bm in _context.BoothMaster on new { pi.BoothMasterId, am.AssemblyMasterId } equals new { bm.BoothMasterId, bm.AssemblyMasterId }
                             where pi.StateMasterId == Convert.ToInt16(sid) && pi.DistrictMasterId == Convert.ToInt16(did)
                             orderby pi.AssemblyMasterId, pi.BoothMasterId, pi.CreatedAt descending
                             select new PollInterruptionDashboard
                             {
                                 PollInterruptionMasterId = pi.PollInterruptionId,
                                 StateMasterId = pi.StateMasterId,
                                 DistrictMasterId = pi.DistrictMasterId,
                                 AssemblyMasterId = pi.AssemblyMasterId,
                                 AssemblyName = am.AssemblyName,
                                 BoothMasterId = bm.BoothMasterId,
                                 PCMasterId = pi.PCMasterId,
                                 BoothName = bm.BoothName + "(" + bm.BoothCode_No + ")",
                                 CreatedAt = pi.CreatedAt,
                                 InterruptionType = pi.InterruptionType,
                                 StopTime = pi.StopTime,
                                 ResumeTime = pi.ResumeTime,
                                 isPollInterrupted = pi.IsPollInterrupted

                             } into distinctResult
                             group distinctResult by new { distinctResult.AssemblyMasterId, distinctResult.BoothMasterId } into groupedResult
                             select groupedResult.OrderByDescending(r => r.CreatedAt).First();

                }

                else if (roles == "PC")
                {
                    result = from pi in _context.PollInterruptions
                             join di in _context.DistrictMaster on pi.DistrictMasterId equals di.DistrictMasterId
                             join am in _context.AssemblyMaster on pi.AssemblyMasterId equals am.AssemblyMasterId
                             join bm in _context.BoothMaster on new { pi.BoothMasterId, am.AssemblyMasterId } equals new { bm.BoothMasterId, bm.AssemblyMasterId }
                             where pi.StateMasterId == Convert.ToInt16(sid) && pi.PCMasterId == Convert.ToInt16(pcid)
                             orderby pi.AssemblyMasterId, pi.BoothMasterId, pi.CreatedAt descending
                             select new PollInterruptionDashboard
                             {
                                 PollInterruptionMasterId = pi.PollInterruptionId,
                                 StateMasterId = pi.StateMasterId,
                                 DistrictMasterId = pi.DistrictMasterId,
                                 AssemblyMasterId = pi.AssemblyMasterId,
                                 AssemblyName = am.AssemblyName,
                                 BoothMasterId = bm.BoothMasterId,
                                 PCMasterId = pi.PCMasterId,
                                 BoothName = bm.BoothName + "(" + bm.BoothCode_No + ")",
                                 CreatedAt = pi.CreatedAt,
                                 InterruptionType = pi.InterruptionType,
                                 StopTime = pi.StopTime,
                                 ResumeTime = pi.ResumeTime,
                                 isPollInterrupted = pi.IsPollInterrupted

                             } into distinctResult
                             group distinctResult by new { distinctResult.AssemblyMasterId, distinctResult.BoothMasterId } into groupedResult
                             select groupedResult.OrderByDescending(r => r.CreatedAt).First();

                }

                else if (roles == "SO")
                {
                    Claim soId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "SoId");
                    if (soId != null)
                    {
                        string soMasterId = soId.Value;
                        result = from pi in _context.PollInterruptions
                                 join di in _context.DistrictMaster on pi.DistrictMasterId equals di.DistrictMasterId
                                 join am in _context.AssemblyMaster on pi.AssemblyMasterId equals am.AssemblyMasterId
                                 join bm in _context.BoothMaster on new { pi.BoothMasterId, am.AssemblyMasterId } equals new { bm.BoothMasterId, bm.AssemblyMasterId }
                                 where pi.StateMasterId == Convert.ToInt16(sid) && bm.AssignedTo == soMasterId && bm.DistrictMasterId == Convert.ToInt16(did) && bm.AssemblyMasterId == Convert.ToInt16(aid)
                                 orderby pi.AssemblyMasterId, pi.BoothMasterId, pi.CreatedAt descending
                                 select new PollInterruptionDashboard
                                 {
                                     PollInterruptionMasterId = pi.PollInterruptionId,
                                     StateMasterId = pi.StateMasterId,
                                     DistrictMasterId = pi.DistrictMasterId,
                                     AssemblyMasterId = pi.AssemblyMasterId,
                                     AssemblyName = am.AssemblyName,
                                     BoothMasterId = bm.BoothMasterId,
                                     PCMasterId = pi.PCMasterId,
                                     BoothName = bm.BoothName,
                                     CreatedAt = pi.CreatedAt,
                                     InterruptionType = pi.InterruptionType,
                                     StopTime = pi.StopTime,
                                     ResumeTime = pi.ResumeTime,
                                     isPollInterrupted = pi.IsPollInterrupted

                                 } into distinctResult
                                 group distinctResult by new { distinctResult.AssemblyMasterId, distinctResult.BoothMasterId } into groupedResult
                                 select groupedResult.OrderByDescending(r => r.CreatedAt).First();
                    }

                }
            }

            // Execute the query and retrieve the results
            finalResult = result.ToList();



            return finalResult;





            //return finalResult;
        }
        public async Task<int> GetPollInterruptionDashboardCount(ClaimsIdentity claimsIdentity)
        {
            List<PollInterruptionDashboard> finalResult = new List<PollInterruptionDashboard>();
            int pollCount = 0;
            if (claimsIdentity != null)
            {
                Claim stateMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "StateMasterId");
                Claim districtMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "DistrictMasterId");
                Claim assemblyMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "AssemblyMasterId");
                Claim pcMasterid = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "PCMasterId");

                string sid = stateMasterId.Value; string aid = assemblyMasterId.Value; string did = districtMasterId.Value; string pcid = "";
                if (pcMasterid != null)
                {
                    pcid = pcMasterid.Value;
                }
                //Claim role = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "roles");
                var roles = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
                //var roles = claimsIdentity.Claims(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
                if (roles == "ARO" || roles == "SubARO")
                {

                    var latestRecordsCount = await _context.PollInterruptions
                           .Where(pi => pi.StateMasterId == Convert.ToInt32(sid) && pi.AssemblyMasterId == Convert.ToInt16(aid))
                           .GroupBy(pi => new { pi.AssemblyMasterId, pi.BoothMasterId })
                           .Select(groupedResult => groupedResult.OrderByDescending(r => r.CreatedAt).FirstOrDefault())
                            .Where(pi => pi.IsPollInterrupted == true)
                           .CountAsync();
                    pollCount = latestRecordsCount;
                }
                else if (roles == "StateAdmin")
                {

                    var latestRecordsCount = await _context.PollInterruptions
                           .Where(pi => pi.StateMasterId == Convert.ToInt32(sid))
                           .GroupBy(pi => new { pi.AssemblyMasterId, pi.BoothMasterId })
                           .Select(groupedResult => groupedResult.OrderByDescending(r => r.CreatedAt).FirstOrDefault())
                            .Where(pi => pi.IsPollInterrupted == true)
                           .CountAsync();
                    pollCount = latestRecordsCount;
                }
                else if (roles == "ECI" || roles == "SuperAdmin")
                {

                    var latestRecordsCount = await _context.PollInterruptions
                           .Where(pi => pi.StateMasterId == Convert.ToInt32(sid))
                           .GroupBy(pi => new { pi.AssemblyMasterId, pi.BoothMasterId })
                           .Select(groupedResult => groupedResult.OrderByDescending(r => r.CreatedAt).FirstOrDefault())
                            .Where(pi => pi.IsPollInterrupted == true)
                           .CountAsync();
                    pollCount = latestRecordsCount;

                }
                else if (roles == "DistrictAdmin")
                {

                    var latestRecordsCount = await _context.PollInterruptions
                           .Where(pi => pi.StateMasterId == Convert.ToInt32(sid) && pi.DistrictMasterId == Convert.ToInt16(did))
                           .GroupBy(pi => new { pi.AssemblyMasterId, pi.BoothMasterId })
                           .Select(groupedResult => groupedResult.OrderByDescending(r => r.CreatedAt).FirstOrDefault())
                            .Where(pi => pi.IsPollInterrupted == true)
                           .CountAsync();
                    pollCount = latestRecordsCount;
                }
                else if (roles == "PC")
                {

                    var latestRecordsCount = await _context.PollInterruptions
                           .Where(pi => pi.StateMasterId == Convert.ToInt32(sid) && pi.PCMasterId == Convert.ToInt16(pcid))
                           .GroupBy(pi => new { pi.AssemblyMasterId, pi.BoothMasterId })
                           .Select(groupedResult => groupedResult.OrderByDescending(r => r.CreatedAt).FirstOrDefault())
                            .Where(pi => pi.IsPollInterrupted == true)
                           .CountAsync();
                    pollCount = latestRecordsCount;
                }


            }




            return pollCount;





            //return finalResult;
        }


        public async Task<BoothMaster> GetBoothRecord(int boothMasterId)
        {
            var boothRecord = await _context.BoothMaster.Where(d => d.BoothMasterId == boothMasterId).FirstOrDefaultAsync();
            return boothRecord;
        }
        public async Task<List<PollInterruptionDashboard>> GetBoothListBySoIdfoInterruption(ClaimsIdentity claimsIdentity)
        {
            List<PollInterruptionDashboard> finalResult = new List<PollInterruptionDashboard>();
            IQueryable<PollInterruptionDashboard> result = null;
            if (claimsIdentity != null)
            {
                Claim stateMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "StateMasterId");
                Claim districtMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "DistrictMasterId");
                Claim assemblyMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "AssemblyMasterId");
                Claim pcMasterid = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "PCMasterId");

                string sid = stateMasterId.Value; string aid = assemblyMasterId.Value; string did = districtMasterId.Value; string pcid = "";
                var roles = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;

                if (roles == "SO")
                {
                    Claim soMasterid = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "SoId"); string soId = soMasterid.Value;
                    result = from pi in _context.PollInterruptions
                             join di in _context.DistrictMaster on pi.DistrictMasterId equals di.DistrictMasterId
                             join am in _context.AssemblyMaster on pi.AssemblyMasterId equals am.AssemblyMasterId
                             join bm in _context.BoothMaster on new { pi.BoothMasterId, am.AssemblyMasterId } equals new { bm.BoothMasterId, bm.AssemblyMasterId }
                             where bm.AssignedTo == soId
                             orderby pi.AssemblyMasterId, pi.BoothMasterId, pi.CreatedAt descending
                             select new PollInterruptionDashboard
                             {
                                 PollInterruptionMasterId = pi.PollInterruptionId,
                                 StateMasterId = pi.StateMasterId,
                                 DistrictMasterId = pi.DistrictMasterId,
                                 AssemblyMasterId = pi.AssemblyMasterId,
                                 AssemblyName = am.AssemblyName,
                                 BoothMasterId = bm.BoothMasterId,
                                 PCMasterId = pi.PCMasterId,
                                 BoothName = bm.BoothName,
                                 CreatedAt = pi.CreatedAt,
                                 InterruptionType = pi.InterruptionType,
                                 StopTime = pi.StopTime,
                                 ResumeTime = pi.ResumeTime,
                                 isPollInterrupted = pi.IsPollInterrupted

                             } into distinctResult
                             group distinctResult by new { distinctResult.AssemblyMasterId, distinctResult.BoothMasterId } into groupedResult
                             select groupedResult.OrderByDescending(r => r.CreatedAt).First();
                }

            }

            var count = finalResult.Count();
            return await result.ToListAsync();

        }


        #endregion

        #region Polling Station Master and Methods

        public async Task<bool> PollingStationRecord(int boothMasterId)
        {
            bool isFreshEntry = false;
            var pSRecord = await _context.PollingStationMaster.Where(d => d.BoothMasterId == boothMasterId).ToListAsync();
            if (pSRecord.Count > 0)
            {
                isFreshEntry = false;
            }
            else
            {
                isFreshEntry = true;
            }
            return isFreshEntry;
        }

        public async Task<bool> GetPollingStationRecordById(int? psMasterId)
        {
            bool boothFreezed = false;
            var isPsFreezed = await _context.PollingStationMaster.Where(d => d.PollingStationMasterId == psMasterId).Select(p => p.Freezed).FirstOrDefaultAsync();
            if (isPsFreezed == true)
            {
                boothFreezed = true;
            }
            else
            {
                boothFreezed = false;
            }
            return boothFreezed;
        }
        public async Task<List<PSoFomCombined>> GetPsoFormDetail(string stateId, string districtId, string assemblyMasterId)
        {
            var isStateActive = _context.StateMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateId)).FirstOrDefault();
            var isDistrictActive = _context.DistrictMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateId) && d.DistrictMasterId == Convert.ToInt32(districtId)).FirstOrDefault();
            if (isStateActive.StateStatus && isDistrictActive.DistrictStatus)
            {
                var innerJoin = from asemb in _context.AssemblyMaster.Where(d => d.DistrictMasterId == Convert.ToInt32(districtId)) // outer sequence
                                join dist in _context.DistrictMaster // inner sequence 
                                on asemb.DistrictMasterId equals dist.DistrictMasterId // key selector
                                join pc in _context.ParliamentConstituencyMaster
                                on asemb.PCMasterId equals pc.PCMasterId

                                join state in _context.StateMaster // additional join for StateMaster
                                on dist.StateMasterId equals state.StateMasterId // key selector for StateMaster
                                where state.StateMasterId == Convert.ToInt32(stateId) && asemb.AssemblyMasterId == Convert.ToInt32(assemblyMasterId) && dist.DistrictMasterId == Convert.ToInt32(districtId)
                                orderby asemb.AssemblyMasterId
                                select new PSoFomCombined
                                { // result selector 
                                    StateMasterId = state.StateMasterId,
                                    StateName = state.StateName,
                                    StateCode = state.StateCode,
                                    DistrictMasterId = dist.DistrictMasterId,
                                    DistrictName = dist.DistrictName,
                                    DistrictCode = dist.DistrictCode,
                                    AssemblyMasterId = asemb.AssemblyMasterId,
                                    AssemblyName = asemb.AssemblyName,
                                    AssemblyCode = asemb.AssemblyCode.ToString(),
                                    PCMasterId = pc.PCMasterId,
                                    PcCodeNo = pc.PcCodeNo,
                                    PCName = pc.PcName


                                };

                return await innerJoin.ToListAsync();
            }
            else
            {
                return null;

            }
        }
        public async Task<Response> AddPSOForm(PollingStationMaster pollingStationMaster, ClaimsIdentity claimsIdentity)
        {
            var stateMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "StateMasterId")?.Value;
            var assemblyMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "AssemblyMasterId")?.Value;
            var districtMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "DistrictMasterId")?.Value;
            var rolesClaim = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
            var roles = rolesClaim?.Value;


            if (roles == "ARO" && stateMasterId == pollingStationMaster.StateMasterId.ToString() && assemblyMasterId == pollingStationMaster.AssemblyMasterId.ToString() && districtMasterId == pollingStationMaster.DistrictMasterId.ToString())
            {
                _context.PollingStationMaster.AddRange(pollingStationMaster);
                _context.SaveChanges();


                return new Response() { Status = RequestStatusEnum.OK, Message = $"Polling Station Data Addded Successfully." };
            }
            else
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Invalid User." };
            }




        }
        public async Task<List<PSoFomListView>> GetPSOlistbyARO(string stateMasterId, string districtMasterId, string AssemblyMasterId)
        {
            var psBoothRecord = from bt in _context.BoothMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(AssemblyMasterId)) // outer sequence
                                join ps in _context.PollingStationMaster
                                on bt.BoothMasterId equals ps.BoothMasterId

                                select new PSoFomListView
                                { // result selector 

                                    BoothMasterId = bt.BoothMasterId,
                                    BoothCodeNo = bt.BoothCode_No,
                                    BoothName = bt.BoothName,
                                    BoothNoAuxy = bt.BoothNoAuxy,
                                    Freeze = ps.Freezed,
                                    PollingStationMasterId = ps.PollingStationMasterId

                                };

            return await psBoothRecord.ToListAsync();
        }
        public async Task<List<PSFormViewModel>> GetPSFormRecordbyPSId(string PollingStationMasterId)
        {
            var pollingStationMasters = await _context.PollingStationMaster
                .Include(psm => psm.PollingStationGender)
                .Where(d =>
                    d.PollingStationMasterId == Convert.ToInt32(PollingStationMasterId)).ToListAsync();

            // Mapping PollingStationMaster entities to PSFormViewModel
            var psFormViewModels = pollingStationMasters.Select(psm => new PSFormViewModel
            {
                StateMasterId = psm.StateMasterId,
                PCasterId = psm.PCasterId,
                DistrictMasterId = psm.DistrictMasterId,
                AssemblyMasterId = psm.AssemblyMasterId,
                BoothMasterId = psm.BoothMasterId,
                StateName = psm.StateName,
                StateCode = psm.StateCode,
                DistrictName = psm.DistrictName,
                DistrictCode = psm.DistrictCode,
                ParliamentaryConstituencyNo = psm.ParliamentaryConstituencyNo,
                ParliamentaryConstituencyName = psm.ParliamentaryConstituencyName,
                AssemblySegmentNo = psm.AssemblySegmentNo,
                AssemblySegmentName = psm.AssemblySegmentName,
                PollingStationNo = psm.PollingStationNo,
                PollingStationName = psm.PollingStationName,
                PollingStationAuxy = psm.PollingStationAuxy,
                TotalCUsUsed = psm.TotalCUsUsed,
                TotalBUsUsed = psm.TotalBUsUsed,
                TotalVVPATUsed = psm.TotalVVPATUsed,
                EVMReplaced = psm.EVMReplaced,
                EVMReplacementTime = psm.EVMReplacementTime,
                EVMReplacementReason = psm.EVMReplacementReason,
                VVPATReplaced = psm.VVPATReplaced,
                VVPATReplacementTime = psm.VVPATReplacementTime,
                VVPATReplacementReason = psm.VVPATReplacementReason,
                PollingAgents = psm.PollingAgents,
                pollingStationGenderViewModel = psm.PollingStationGender.Select(psg => new PSGenderViewModel
                {
                    PollingStationGenderId = psg.PollingStationGenderId,
                    PollingStationMasterId = psg.PollingStationMasterId,
                    Male = psg.Male,
                    Female = psg.Female,
                    ThirdGender = psg.ThirdGender,
                    Type = psg.Type,
                    Total = psg.Total
                }).ToList(),
                VisuallyImpaired = psm.VisuallyImpaired,
                HearingImpaired = psm.HearingImpaired,
                LocoMotive = psm.LocoMotive,
                PWDDisabilityOthers = psm.PWDDisabilityOthers,
                DummyBSB = psm.DummyBSB,
                WHC = psm.WHC,
                WBF = psm.WBF,
                VotePolledEPIC = psm.VotePolledEPIC,
                VotePolledOtherDocument = psm.VotePolledOtherDocument,
                TenderedVote = psm.TenderedVote,
                ChallengedVote = psm.ChallengedVote,
                ProxyVote = psm.ProxyVote,
                IsWebCastingDone = psm.IsWebCastingDone,
                IsWebCastingOperatorAvailable = psm.IsWebCastingOperatorAvailable,
                WebCastingName = psm.WebCastingName,
                WebCastingMobileNumber = psm.WebCastingMobileNumber,
                PSManagedByPwD = psm.PSManagedByPwD,
                PinkPSIsManagedByWomen = psm.PinkPSIsManagedByWomen,
                IsModelStation = psm.IsModelStation,
                IPresidingOfficerAgree = psm.IPresidingOfficerAgree,
                Freezed = psm.Freezed,
                PollingStationMasterId = psm.PollingStationMasterId
            }).ToList();

            return psFormViewModels;
        }
        public async Task<Response> UpdatePSoForm(PollingStationMaster pollingStationMaster, ClaimsIdentity claimsIdentity)
        {
            var stateMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "StateMasterId")?.Value;
            var assemblyMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "AssemblyMasterId")?.Value;
            var districtMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "DistrictMasterId")?.Value;
            var rolesClaim = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
            var roles = rolesClaim?.Value;

            if (roles == "ARO" && stateMasterId == pollingStationMaster.StateMasterId.ToString() && assemblyMasterId == pollingStationMaster.AssemblyMasterId.ToString() && districtMasterId == pollingStationMaster.DistrictMasterId.ToString())
            {
                var psRecord = _context.PollingStationMaster.Include(psm => psm.PollingStationGender)
                                                            .Where(d => d.PollingStationMasterId == pollingStationMaster.PollingStationMasterId).FirstOrDefault();
                //var psRecord = _context.PollingStationMaster.Where(d => d.PollingStationMasterId == pollingStationMaster.PollingStationMasterId).FirstOrDefault();

                if (psRecord != null)
                {
                    psRecord.TotalCUsUsed = pollingStationMaster.TotalCUsUsed;
                    psRecord.TotalVVPATUsed = pollingStationMaster.TotalVVPATUsed;
                    psRecord.EVMReplaced = pollingStationMaster.EVMReplaced;
                    psRecord.EVMReplacementTime = pollingStationMaster.EVMReplacementTime;
                    psRecord.EVMReplacementReason = pollingStationMaster.EVMReplacementReason;
                    psRecord.VVPATReplaced = pollingStationMaster.VVPATReplaced;
                    psRecord.VVPATReplacementTime = pollingStationMaster.VVPATReplacementTime;
                    psRecord.VVPATReplacementReason = pollingStationMaster.VVPATReplacementReason;
                    psRecord.PollingAgents = pollingStationMaster.PollingAgents;
                    psRecord.HearingImpaired = pollingStationMaster.HearingImpaired;
                    psRecord.LocoMotive = pollingStationMaster.LocoMotive;
                    psRecord.PWDDisabilityOthers = pollingStationMaster.PWDDisabilityOthers;
                    psRecord.DummyBSB = pollingStationMaster.DummyBSB;
                    psRecord.WHC = pollingStationMaster.WHC;
                    psRecord.WBF = pollingStationMaster.WBF;
                    psRecord.VotePolledEPIC = pollingStationMaster.VotePolledEPIC;
                    psRecord.VotePolledOtherDocument = pollingStationMaster.VotePolledOtherDocument;
                    psRecord.TenderedVote = pollingStationMaster.TenderedVote;
                    psRecord.ChallengedVote = pollingStationMaster.ChallengedVote;
                    psRecord.VisuallyImpaired = pollingStationMaster.VisuallyImpaired;
                    psRecord.ProxyVote = pollingStationMaster.ProxyVote;
                    psRecord.IsWebCastingDone = pollingStationMaster.IsWebCastingDone;
                    psRecord.IsWebCastingOperatorAvailable = pollingStationMaster.IsWebCastingOperatorAvailable;
                    psRecord.WebCastingName = pollingStationMaster.WebCastingName;
                    psRecord.WebCastingMobileNumber = pollingStationMaster.WebCastingMobileNumber;
                    psRecord.PSManagedByPwD = pollingStationMaster.PSManagedByPwD;
                    psRecord.PinkPSIsManagedByWomen = pollingStationMaster.PinkPSIsManagedByWomen;
                    psRecord.IsModelStation = pollingStationMaster.IsModelStation;
                    psRecord.IPresidingOfficerAgree = pollingStationMaster.IPresidingOfficerAgree;
                    psRecord.Freezed = pollingStationMaster.Freezed;
                    psRecord.AssemblySegmentName = pollingStationMaster.AssemblySegmentName;
                    psRecord.DistrictName = pollingStationMaster.DistrictName;
                    psRecord.ParliamentaryConstituencyName = pollingStationMaster.ParliamentaryConstituencyName;


                    if (psRecord.PollingStationGender != null)
                    {
                        foreach (var psg in psRecord.PollingStationGender)
                        {
                            var newPSGender = pollingStationMaster.PollingStationGender.FirstOrDefault(n => n.PollingStationGenderId == psg.PollingStationGenderId);
                            if (newPSGender != null)
                            {
                                psg.Male = newPSGender.Male;
                                psg.Female = newPSGender.Female;
                                psg.ThirdGender = newPSGender.ThirdGender;
                                psg.Type = newPSGender.Type;
                                psg.Total = newPSGender.Total;
                            }
                        }
                    }


                    _context.PollingStationMaster.Update(psRecord);



                    _context.PollingStationGender.RemoveRange(psRecord.PollingStationGender);
                    _context.PollingStationGender.AddRange(pollingStationMaster.PollingStationGender);

                    await _context.SaveChangesAsync();



                    return new Response() { Status = RequestStatusEnum.OK, Message = $"Polling Station Data Updated Successfully." };
                }
                else
                {
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Polling Station Master Record not Found." };
                }
            }
            else
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Invalid User." };
            }
        }
        #endregion



        #region Location master for Super Admin

        public async Task<List<LocationModelList>> GetLocationMasterforALL(BoothReportModel model)
        {
            var isStateActive = _context.StateMaster.Where(d => d.StateMasterId == model.StateMasterId).FirstOrDefault();

            var isAssemblyActive = _context.AssemblyMaster.Where(d => d.StateMasterId == model.StateMasterId && d.DistrictMasterId == model.DistrictMasterId && d.AssemblyMasterId == model.AssemblyMasterId).FirstOrDefault();
            // dist sts, asem
            if (model.StateMasterId is not 0 && model.DistrictMasterId is not 0 && model.AssemblyMasterId is not 0 && model.PCMasterId is 0)
            {
                var isDistrictActive = _context.DistrictMaster.Where(d => d.StateMasterId == model.StateMasterId && d.DistrictMasterId == model.DistrictMasterId).FirstOrDefault();
                if (isStateActive.StateStatus && isDistrictActive.DistrictStatus && isAssemblyActive.AssemblyStatus)
                {
                    List<LocationModelList> locationModelLists = new List<LocationModelList>();
                    var locations = from lc in _context.PollingLocationMaster.Where(d => d.AssemblyMasterId == model.AssemblyMasterId && d.StateMasterId == d.StateMasterId && d.DistrictMasterId == d.DistrictMasterId)
                                    join asem in _context.AssemblyMaster
                                     on lc.AssemblyMasterId equals asem.AssemblyMasterId
                                    join dist in _context.DistrictMaster
                                    on asem.DistrictMasterId equals dist.DistrictMasterId
                                    join state in _context.StateMaster
                                     on dist.StateMasterId equals state.StateMasterId
                                    join pc in _context.ParliamentConstituencyMaster
                                    //on lc.PCMasterId equals pc.PCMasterId
                                    on asem.PCMasterId equals pc.PCMasterId

                                    select new LocationModelList
                                    {
                                        StateMasterId = state.StateMasterId,
                                        PCMasterId = pc.PCMasterId,
                                        PCName = pc.PcName,
                                        DistrictMasterId = dist.DistrictMasterId,
                                        AssemblyMasterId = asem.AssemblyMasterId,
                                        StateName = state.StateName,
                                        DistrictName = dist.DistrictName,
                                        AssemblyName = asem.AssemblyName,
                                        LocationMasterId = lc.LocationMasterId,
                                        LocationName = lc.LocationName,
                                        LocationCode = lc.LocationCode,
                                        LocationLatitude = lc.LocationLatitude,
                                        LocationLongitude = lc.LocationLongitude,
                                        IsStatus = lc.Status,
                                        CreatedOn = lc.CreatedOn,

                                    };

                    foreach (var location in locations)
                    {
                        var boothNumbers = _context.BoothMaster
        .Where(d => d.LocationMasterId == location.LocationMasterId)
        .Select(d => int.Parse(d.BoothCode_No))
        .ToList();


                        location.BoothNumbers = string.Join(", ", boothNumbers);
                        locationModelLists.Add(location);
                    }
                    //var sortedlocationList = await locationModelLists.ToListAsync();
                    return locationModelLists;
                }
                else
                {
                    return null;

                }
            }
            //state,pc,ase.m
            else if (model.StateMasterId is not 0 && model.DistrictMasterId is 0 && model.AssemblyMasterId is not 0 && model.PCMasterId is not 0)
            {
                var isPCRecord = _context.ParliamentConstituencyMaster.Where(d => d.StateMasterId == model.StateMasterId && d.PCMasterId == model.PCMasterId).FirstOrDefault();
                if (isPCRecord != null)
                {
                    List<LocationModelList> locationModelLists = new List<LocationModelList>();
                    var locations = from lc in _context.PollingLocationMaster.Where(d => d.AssemblyMasterId == model.AssemblyMasterId && d.StateMasterId == d.StateMasterId && d.DistrictMasterId == d.DistrictMasterId)
                                    join asem in _context.AssemblyMaster
                                     on lc.AssemblyMasterId equals asem.AssemblyMasterId
                                    join dist in _context.DistrictMaster
                                    on asem.DistrictMasterId equals dist.DistrictMasterId
                                    join state in _context.StateMaster
                                     on dist.StateMasterId equals state.StateMasterId
                                    join pc in _context.ParliamentConstituencyMaster
                                    on asem.PCMasterId equals pc.PCMasterId
                                    // on lc.PCMasterId equals pc.PCMasterId

                                    select new LocationModelList
                                    {
                                        StateMasterId = state.StateMasterId,
                                        PCMasterId = pc.PCMasterId,
                                        PCName = pc.PcName,
                                        DistrictMasterId = dist.DistrictMasterId,
                                        AssemblyMasterId = asem.AssemblyMasterId,
                                        StateName = state.StateName,
                                        DistrictName = dist.DistrictName,
                                        AssemblyName = asem.AssemblyName,
                                        LocationMasterId = lc.LocationMasterId,
                                        LocationName = lc.LocationName,
                                        LocationCode = lc.LocationCode,
                                        LocationLatitude = lc.LocationLatitude,
                                        LocationLongitude = lc.LocationLongitude,
                                        IsStatus = lc.Status,
                                        CreatedOn = lc.CreatedOn,

                                    };

                    foreach (var location in locations)
                    {
                        var boothNumbers = _context.BoothMaster
        .Where(d => d.LocationMasterId == location.LocationMasterId)
        .Select(d => int.Parse(d.BoothCode_No))
        .ToList();


                        location.BoothNumbers = string.Join(", ", boothNumbers);
                        locationModelLists.Add(location);
                    }
                    //var sortedlocationList = await locationModelLists.ToListAsync();
                    return locationModelLists;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

        }
        /*public async Task<List<LocationModelList>> GetLocationMasterforARO(string stateMasterId, string districtMasterId, string assemblyMasterId)
        {
            var isStateActive = _context.StateMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId)).FirstOrDefault();
            var isDistrictActive = _context.DistrictMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId)).FirstOrDefault();
            var isAssemblyActive = _context.AssemblyMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId)).FirstOrDefault();
            if (isStateActive.StateStatus && isDistrictActive.DistrictStatus && isAssemblyActive.AssemblyStatus)
            {
                List<LocationModelList> locationModelLists = new List<LocationModelList>();
                var locations = from lc in _context.PollingLocationMaster.Where(d => d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId) && d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId))
                                join asem in _context.AssemblyMaster
                                 on lc.AssemblyMasterId equals asem.AssemblyMasterId
                                join dist in _context.DistrictMaster
                                on asem.DistrictMasterId equals dist.DistrictMasterId
                                join state in _context.StateMaster
                                 on dist.StateMasterId equals state.StateMasterId
                                join pc in _context.ParliamentConstituencyMaster
                                on lc.PCMasterId equals pc.PCMasterId

                                select new LocationModelList
                                {
                                    StateMasterId = state.StateMasterId,
                                    PCMasterId = pc.PCMasterId,
                                    PCName = pc.PcName,
                                    DistrictMasterId = dist.DistrictMasterId,
                                    AssemblyMasterId = asem.AssemblyMasterId,
                                    StateName = state.StateName,
                                    DistrictName = dist.DistrictName,
                                    AssemblyName = asem.AssemblyName,
                                    LocationMasterId = lc.LocationMasterId,
                                    LocationName = lc.LocationName,
                                    LocationCode = lc.LocationCode,
                                    LocationLatitude = lc.LocationLatitude,
                                    LocationLongitude = lc.LocationLongitude,
                                    IsStatus = lc.Status,
                                    CreatedOn = lc.CreatedOn,

                                };

                foreach (var location in locations)
                {
                    var boothNumbers = _context.BoothMaster
    .Where(d => d.LocationMasterId == location.LocationMasterId)
    .Select(d => int.Parse(d.BoothCode_No))
    .ToList();


                    location.BoothNumbers = string.Join(", ", boothNumbers);
                    locationModelLists.Add(location);
                }
                //var sortedlocationList = await locationModelLists.ToListAsync();
                return locationModelLists;
            }
            else
            {
                return null;

            }
        }
        public async Task<List<LocationModel>> GetLocationMasterById(string locationMasterId)
        {
            var locationMasterRecord = await _context.PollingLocationMaster.Where(d => d.LocationMasterId == Convert.ToInt32(locationMasterId)).Select(d => new LocationModel
            {
                LocationMasterId = d.LocationMasterId,
                StateMasterId = d.StateMasterId,
                DistrictMasterId = d.DistrictMasterId,
                AssemblyMasterId = d.AssemblyMasterId,
                PCMasterId = d.PCMasterId,
                LocationName = d.LocationName,
                LocationCode = d.LocationCode,
                LocationLatitude = d.LocationLatitude,
                LocationLongitude = d.LocationLongitude,
                IsStatus = d.Status,
                CreatedOn = d.CreatedOn,
                BoothMasterId = _context.BoothMaster.Where(p => p.LocationMasterId == Convert.ToInt32(locationMasterId)).OrderBy(p => Convert.ToInt32(p.BoothCode_No)).Select(p => p.BoothMasterId).ToList()


            }).ToListAsync();
            return locationMasterRecord;
        }
        public async Task<Response> AddLocation(LocationModel locationModel)
        {
            try
            {
                var isStateActive = _context.StateMaster.Where(d => d.StateMasterId == locationModel.StateMasterId).FirstOrDefault();
                
                var isAssemblyActive = _context.AssemblyMaster.Where(d => d.StateMasterId == locationModel.StateMasterId && d.DistrictMasterId == locationModel.DistrictMasterId && d.AssemblyMasterId == locationModel.AssemblyMasterId).FirstOrDefault();
                // super: ARO->state,district,asem
                if (locationModel.StateMasterId is not 0 && locationModel.DistrictMasterId is not 0 && locationModel.AssemblyMasterId is not 0 && locationModel.PCMasterId is 0)
                {
                    var isDistrictActive = _context.DistrictMaster.Where(d => d.StateMasterId == locationModel.StateMasterId && d.DistrictMasterId == locationModel.DistrictMasterId).FirstOrDefault();
                    if (isStateActive.StateStatus && isDistrictActive.DistrictStatus && isAssemblyActive.AssemblyStatus)
                    {
                        // Check if a location with the same code already exists
                        var locationExist = _context.PollingLocationMaster
                    .FirstOrDefault(p => p.LocationCode == locationModel.LocationCode &&
                                         p.StateMasterId == locationModel.StateMasterId &&
                                         p.AssemblyMasterId == locationModel.AssemblyMasterId &&
                                         p.DistrictMasterId == locationModel.DistrictMasterId);

                        if (locationExist != null)
                        {
                            return new Response { Status = RequestStatusEnum.BadRequest, Message = $"Location with code {locationModel.LocationCode} already exists in the State Assembly." };
                        }

                        // Check if a location with the same name already exists
                        var locationExistName = _context.PollingLocationMaster
                            .FirstOrDefault(p => p.LocationName == locationModel.LocationName &&
                                                 p.StateMasterId == locationModel.StateMasterId &&
                                                 p.AssemblyMasterId == locationModel.AssemblyMasterId &&
                                                 p.DistrictMasterId == locationModel.DistrictMasterId);

                        if (locationExistName != null)
                        {
                            return new Response { Status = RequestStatusEnum.BadRequest, Message = $"Location with name {locationModel.LocationName} already exists in the State Assembly." };
                        }

                        //must check any booth already allocated with other location id

                        // Create a new PollingLocationMaster


                        string boothIdsAlreadyhaveLocationId = ""; string msg = "";
                        foreach (var boothMasterId in locationModel.BoothMasterId)
                        {
                            var boothRecord = _context.BoothMaster.SingleOrDefault(b => b.BoothMasterId == boothMasterId);
                            if (boothRecord != null)
                            {
                                if (boothRecord.LocationMasterId != null)
                                {
                                    boothIdsAlreadyhaveLocationId = boothRecord.BoothCode_No + ",";
                                }
                            }
                        }

                        if (boothIdsAlreadyhaveLocationId == string.Empty)
                        {
                            locationModel.CreatedOn = BharatDateTime();
                            PollingLocationMaster lm = new PollingLocationMaster()
                            {
                                LocationName = locationModel.LocationName,
                                LocationCode = locationModel.LocationCode,
                                StateMasterId = locationModel.StateMasterId,
                                AssemblyMasterId = locationModel.AssemblyMasterId,
                                DistrictMasterId = locationModel.DistrictMasterId,
                                //PCMasterId = locationModel.PCMasterId,
                                PCMasterId = isAssemblyActive.PCMasterId,
                                LocationLatitude = locationModel.LocationLatitude,
                                LocationLongitude = locationModel.LocationLongitude,
                                CreatedOn = locationModel.CreatedOn,
                                Status = locationModel.IsStatus
                            };

                            // Add the new location to the context
                            _context.PollingLocationMaster.Add(lm);
                            await _context.SaveChangesAsync(); // Use asynchronous save changes

                            if (locationModel.BoothMasterId != null && locationModel.BoothMasterId.Any())
                            {

                                foreach (var boothMasterId in locationModel.BoothMasterId)
                                {
                                    var boothRecord = _context.BoothMaster.SingleOrDefault(b => b.BoothMasterId == boothMasterId);
                                    if (boothRecord != null)
                                    {
                                        boothRecord.LocationMasterId = lm.LocationMasterId; // Update with the newly created LocationMasterId
                                    }
                                }

                                await _context.SaveChangesAsync(); // Use asynchronous save changes
                            }

                            msg = locationModel.LocationName + " " + "added & mapped successfully.";
                            return new Response { Status = RequestStatusEnum.OK, Message = msg };
                        }

                        else
                        {
                            msg = "These Booth Numbers Already have Location Mapped: (" + boothIdsAlreadyhaveLocationId + ")" + " " + "First Release its Location.";
                            return new Response { Status = RequestStatusEnum.BadRequest, Message = msg };
                        }
                    }
                    else
                    {
                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Please Check State,District,Assembly Status." };
                    }
                }
               //super for pc
                else if (locationModel.StateMasterId is not 0 && locationModel.DistrictMasterId is  0 && locationModel.AssemblyMasterId is not 0 && locationModel.PCMasterId is not 0)
                {
                    if (locationModel.PCMasterId > 0)
                    {
                        var isPCActive = _context.ParliamentConstituencyMaster.Where(d => d.StateMasterId == locationModel.StateMasterId && d.PCMasterId == locationModel.PCMasterId && d.PcStatus == true).FirstOrDefault();
                        if (isPCActive.PcStatus)
                        {
                            if (isStateActive.StateStatus && isAssemblyActive.AssemblyStatus)
                            {
                                // Check if a location with the same code already exists
                                var locationExist = _context.PollingLocationMaster
                            .FirstOrDefault(p => p.LocationCode == locationModel.LocationCode &&
                                                 p.StateMasterId == locationModel.StateMasterId &&
                                                 p.AssemblyMasterId == locationModel.AssemblyMasterId &&
                                                 p.PCMasterId == locationModel.PCMasterId);

                                if (locationExist != null)
                                {
                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = $"Location with code {locationModel.LocationCode} already exists in the State Assembly." };
                                }

                                // Check if a location with the same name already exists
                                var locationExistName = _context.PollingLocationMaster
                                    .FirstOrDefault(p => p.LocationName == locationModel.LocationName &&
                                                         p.StateMasterId == locationModel.StateMasterId &&
                                                         p.AssemblyMasterId == locationModel.AssemblyMasterId &&
                                                         p.PCMasterId == locationModel.PCMasterId);

                                if (locationExistName != null)
                                {
                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = $"Location with name {locationModel.LocationName} already exists in the State Assembly." };
                                }

                                //must check any booth already allocated with other location id

                                // Create a new PollingLocationMaster


                                string boothIdsAlreadyhaveLocationId = ""; string msg = "";
                                foreach (var boothMasterId in locationModel.BoothMasterId)
                                {
                                    var boothRecord = _context.BoothMaster.SingleOrDefault(b => b.BoothMasterId == boothMasterId);
                                    if (boothRecord != null)
                                    {
                                        if (boothRecord.LocationMasterId != null)
                                        {
                                            boothIdsAlreadyhaveLocationId = boothRecord.BoothCode_No + ",";
                                        }
                                    }
                                }

                                if (boothIdsAlreadyhaveLocationId == string.Empty)
                                {
                                    locationModel.CreatedOn = BharatDateTime();
                                    PollingLocationMaster lm = new PollingLocationMaster()
                                    {
                                        LocationName = locationModel.LocationName,
                                        LocationCode = locationModel.LocationCode,
                                        StateMasterId = locationModel.StateMasterId,
                                        AssemblyMasterId = locationModel.AssemblyMasterId,
                                        //DistrictMasterId = locationModel.DistrictMasterId,
                                        DistrictMasterId = isAssemblyActive.DistrictMasterId,
                                        PCMasterId = locationModel.PCMasterId,
                                        LocationLatitude = locationModel.LocationLatitude,
                                        LocationLongitude = locationModel.LocationLongitude,
                                        CreatedOn = locationModel.CreatedOn,
                                        Status = locationModel.IsStatus
                                    };

                                    // Add the new location to the context
                                    _context.PollingLocationMaster.Add(lm);
                                    await _context.SaveChangesAsync(); // Use asynchronous save changes

                                    if (locationModel.BoothMasterId != null && locationModel.BoothMasterId.Any())
                                    {

                                        foreach (var boothMasterId in locationModel.BoothMasterId)
                                        {
                                            var boothRecord = _context.BoothMaster.SingleOrDefault(b => b.BoothMasterId == boothMasterId);
                                            if (boothRecord != null)
                                            {
                                                boothRecord.LocationMasterId = lm.LocationMasterId; // Update with the newly created LocationMasterId
                                            }
                                        }

                                        await _context.SaveChangesAsync(); // Use asynchronous save changes
                                    }

                                    msg = locationModel.LocationName + " " + "added & mapped successfully.";
                                    return new Response { Status = RequestStatusEnum.OK, Message = msg };
                                }

                                else
                                {
                                    msg = "These Booth Numbers Already have Location Mapped: (" + boothIdsAlreadyhaveLocationId + ")" + " " + "First Release its Location.";
                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = msg };
                                }
                            }
                            else
                            {
                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Please Check State,District,Assembly Status." };
                            }
                        }
                        else
                        {
                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "PC is not Active" };

                        }

                    }
                    else
                    {
                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "PC id can't be 0" };

                    }
                }
                else
                {
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Kindly send accurate Parameters." };

                }

            }
            catch (Exception ex)
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = ex.Message };
            }
        }
        public async Task<Response> UpdateLocation(LocationModel locationModel)
        {
            try
            {
                    var isStateActive = _context.StateMaster.Where(d => d.StateMasterId == locationModel.StateMasterId).FirstOrDefault();
                var isAssemblyActive = _context.AssemblyMaster.Where(d => d.StateMasterId == locationModel.StateMasterId && d.DistrictMasterId == locationModel.DistrictMasterId && d.AssemblyMasterId == locationModel.AssemblyMasterId).FirstOrDefault();

                //Update from super,ARO->state,district,asem
                if (locationModel.StateMasterId is not 0 && locationModel.DistrictMasterId is not 0 && locationModel.AssemblyMasterId is not 0 && locationModel.PCMasterId is 0)
                {
                    var isDistrictActive = _context.DistrictMaster.Where(d => d.StateMasterId == locationModel.StateMasterId && d.DistrictMasterId == locationModel.DistrictMasterId).FirstOrDefault();
                    if (isStateActive.StateStatus && isDistrictActive.DistrictStatus && isAssemblyActive.AssemblyStatus)
                    {
                        if (locationModel.LocationMasterId != null && locationModel.LocationMasterId > 0)
                        {
                            var pollingStationRecord = _context.PollingLocationMaster
                                .FirstOrDefault(d => d.LocationMasterId == locationModel.LocationMasterId);

                            if (pollingStationRecord != null)
                            {
                                // Check if any booth is already allocated to a different location
                                string boothIdsAlreadyHaveLocationId = "";
                                foreach (var boothMasterId in locationModel.BoothMasterId)
                                {
                                    var boothRecord = _context.BoothMaster.SingleOrDefault(b => b.BoothMasterId == boothMasterId);
                                    if (boothRecord != null && boothRecord.LocationMasterId != null && boothRecord.LocationMasterId != locationModel.LocationMasterId)
                                    {
                                        boothIdsAlreadyHaveLocationId += boothRecord.BoothCode_No + ",";
                                    }
                                }

                                if (string.IsNullOrEmpty(boothIdsAlreadyHaveLocationId))
                                {
                                    // Get the existing BoothMaster records associated with the LocationMasterId
                                    var boothRecordList = _context.BoothMaster
                                        .Where(b => b.LocationMasterId == locationModel.LocationMasterId)
                                        .ToList();

                                    // Set LocationMasterId to null for the existing BoothMaster records
                                    foreach (var boothRecord in boothRecordList)
                                    {
                                        boothRecord.LocationMasterId = null;
                                        _context.BoothMaster.Update(boothRecord);
                                    }

                                    await _context.SaveChangesAsync();

                                    // Now, update BoothMaster records with the new LocationMasterId
                                    foreach (var boothMasterId in locationModel.BoothMasterId)
                                    {
                                        var boothRecordNew = _context.BoothMaster.SingleOrDefault(b => b.BoothMasterId == boothMasterId);
                                        if (boothRecordNew != null)
                                        {
                                            boothRecordNew.LocationMasterId = locationModel.LocationMasterId;
                                            _context.BoothMaster.Update(boothRecordNew);
                                        }
                                    }

                                    // Update Main Table (PollingLocationMaster)
                                    pollingStationRecord.LocationName = locationModel.LocationName;
                                    pollingStationRecord.LocationCode = locationModel.LocationCode;
                                    pollingStationRecord.LocationLatitude = locationModel.LocationLatitude;
                                    pollingStationRecord.LocationLongitude = locationModel.LocationLongitude;
                                    pollingStationRecord.CreatedOn = BharatDateTime();
                                    pollingStationRecord.Status = locationModel.IsStatus;
                                    _context.PollingLocationMaster.Update(pollingStationRecord);

                                    await _context.SaveChangesAsync(); // Use asynchronous save changes
                                    return new Response { Status = RequestStatusEnum.OK, Message = "Polling Station Data Updated Successfully." };
                                }
                                else
                                {
                                    string message = "These Booth Number(s) already have Location mapped: (" + boothIdsAlreadyHaveLocationId.TrimEnd(',') + "). First, release their Location or unselect from the list.";
                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = message };
                                }
                            }
                            else
                            {
                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Location Master Record not Found." };
                            }
                        }
                        else
                        {
                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Location Master Id cannot be null or less than/equal to zero." };
                        }
                    }
                    else
                    {
                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Please Check State,District,Assembly Sttaus." };
                    }
                }

                //Update from super only->state,PC,asem
                else if (locationModel.StateMasterId is not 0 && locationModel.DistrictMasterId is  0 && locationModel.AssemblyMasterId is not 0 && locationModel.PCMasterId is not 0)
                {
                    var isPCActive = _context.ParliamentConstituencyMaster.Where(d => d.StateMasterId == locationModel.StateMasterId && d.PCMasterId == locationModel.PCMasterId && d.PcStatus == true).FirstOrDefault();
                    if (isPCActive.PcStatus)
                    {
                        if (isStateActive.StateStatus && isAssemblyActive.AssemblyStatus)
                        {
                            if (locationModel.LocationMasterId != null && locationModel.LocationMasterId > 0)
                            {
                                var pollingStationRecord = _context.PollingLocationMaster
                                    .FirstOrDefault(d => d.LocationMasterId == locationModel.LocationMasterId);

                                if (pollingStationRecord != null)
                                {
                                    // Check if any booth is already allocated to a different location
                                    string boothIdsAlreadyHaveLocationId = "";
                                    foreach (var boothMasterId in locationModel.BoothMasterId)
                                    {
                                        var boothRecord = _context.BoothMaster.SingleOrDefault(b => b.BoothMasterId == boothMasterId);
                                        if (boothRecord != null && boothRecord.LocationMasterId != null && boothRecord.LocationMasterId != locationModel.LocationMasterId)
                                        {
                                            boothIdsAlreadyHaveLocationId += boothRecord.BoothCode_No + ",";
                                        }
                                    }

                                    if (string.IsNullOrEmpty(boothIdsAlreadyHaveLocationId))
                                    {
                                        // Get the existing BoothMaster records associated with the LocationMasterId
                                        var boothRecordList = _context.BoothMaster
                                            .Where(b => b.LocationMasterId == locationModel.LocationMasterId)
                                            .ToList();

                                        // Set LocationMasterId to null for the existing BoothMaster records
                                        foreach (var boothRecord in boothRecordList)
                                        {
                                            boothRecord.LocationMasterId = null;
                                            _context.BoothMaster.Update(boothRecord);
                                        }

                                        await _context.SaveChangesAsync();

                                        // Now, update BoothMaster records with the new LocationMasterId
                                        foreach (var boothMasterId in locationModel.BoothMasterId)
                                        {
                                            var boothRecordNew = _context.BoothMaster.SingleOrDefault(b => b.BoothMasterId == boothMasterId);
                                            if (boothRecordNew != null)
                                            {
                                                boothRecordNew.LocationMasterId = locationModel.LocationMasterId;
                                                _context.BoothMaster.Update(boothRecordNew);
                                            }
                                        }

                                        // Update Main Table (PollingLocationMaster)
                                        pollingStationRecord.LocationName = locationModel.LocationName;
                                        pollingStationRecord.LocationCode = locationModel.LocationCode;
                                        pollingStationRecord.LocationLatitude = locationModel.LocationLatitude;
                                        pollingStationRecord.LocationLongitude = locationModel.LocationLongitude;
                                        pollingStationRecord.CreatedOn = BharatDateTime();
                                        pollingStationRecord.Status = locationModel.IsStatus;
                                        _context.PollingLocationMaster.Update(pollingStationRecord);

                                        await _context.SaveChangesAsync(); // Use asynchronous save changes
                                        return new Response { Status = RequestStatusEnum.OK, Message = "Polling Station Data Updated Successfully." };
                                    }
                                    else
                                    {
                                        string message = "These Booth Number(s) already have Location mapped: (" + boothIdsAlreadyHaveLocationId.TrimEnd(',') + "). First, release their Location or unselect from the list.";
                                        return new Response { Status = RequestStatusEnum.BadRequest, Message = message };
                                    }
                                }
                                else
                                {
                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Location Master Record not Found." };
                                }
                            }
                            else
                            {
                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Location Master Id cannot be null or less than/equal to zero." };
                            }
                        }
                        else
                        {
                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Please Check State,District,Assembly Sttaus." };
                        }
                    }
                    else
                
                    {
                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Pc is not active." };
                    }
                }
                else
                {
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Kindly enter accurate parameters." };

                }

            }
            catch (Exception ex)
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = ex.Message };
            }
        }
        */

        #endregion


        #region LocationMaster


        public async Task<List<LocationModelList>> GetLocationMasterforARO(string stateMasterId, string districtMasterId, string assemblyMasterId)
        {
            var isStateActive = _context.StateMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId)).FirstOrDefault();
            var isDistrictActive = _context.DistrictMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId)).FirstOrDefault();
            var isAssemblyActive = _context.AssemblyMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId)).FirstOrDefault();
            if (isStateActive.StateStatus && isDistrictActive.DistrictStatus && isAssemblyActive.AssemblyStatus)
            {
                List<LocationModelList> locationModelLists = new List<LocationModelList>();
                var locations = from lc in _context.PollingLocationMaster.Where(d => d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId) && d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId))
                                join asem in _context.AssemblyMaster
                                 on lc.AssemblyMasterId equals asem.AssemblyMasterId
                                join dist in _context.DistrictMaster
                                on asem.DistrictMasterId equals dist.DistrictMasterId
                                join state in _context.StateMaster
                                 on dist.StateMasterId equals state.StateMasterId
                                join pc in _context.ParliamentConstituencyMaster
                                on asem.PCMasterId equals pc.PCMasterId

                                select new LocationModelList
                                {
                                    StateMasterId = state.StateMasterId,
                                    PCMasterId = pc.PCMasterId,
                                    PCName = pc.PcName,
                                    DistrictMasterId = dist.DistrictMasterId,
                                    AssemblyMasterId = asem.AssemblyMasterId,
                                    StateName = state.StateName,
                                    DistrictName = dist.DistrictName,
                                    AssemblyName = asem.AssemblyName,
                                    LocationMasterId = lc.LocationMasterId,
                                    LocationName = lc.LocationName,
                                    LocationCode = lc.LocationCode,
                                    LocationLatitude = lc.LocationLatitude,
                                    LocationLongitude = lc.LocationLongitude,
                                    IsStatus = lc.Status,
                                    CreatedOn = lc.CreatedOn,

                                };

                foreach (var location in locations)
                {
                    var boothNumbers = _context.BoothMaster
    .Where(d => d.LocationMasterId == location.LocationMasterId)
    .Select(d => int.Parse(d.BoothCode_No))
    .ToList();


                    location.BoothNumbers = string.Join(", ", boothNumbers);
                    locationModelLists.Add(location);
                }
                //var sortedlocationList = await locationModelLists.ToListAsync();
                return locationModelLists;
            }
            else
            {
                return null;

            }
        }
        public async Task<List<LocationModel>> GetLocationMasterById(string locationMasterId)
        {
            var locationMasterRecord = await _context.PollingLocationMaster.Where(d => d.LocationMasterId == Convert.ToInt32(locationMasterId)).Select(d => new LocationModel
            {
                LocationMasterId = d.LocationMasterId,
                StateMasterId = d.StateMasterId,
                DistrictMasterId = d.DistrictMasterId,
                AssemblyMasterId = d.AssemblyMasterId,
                PCMasterId = d.PCMasterId,
                LocationName = d.LocationName,
                LocationCode = d.LocationCode,
                LocationLatitude = d.LocationLatitude,
                LocationLongitude = d.LocationLongitude,
                IsStatus = d.Status,
                CreatedOn = d.CreatedOn,
                BoothMasterId = _context.BoothMaster.Where(p => p.LocationMasterId == Convert.ToInt32(locationMasterId)).OrderBy(p => Convert.ToInt32(p.BoothCode_No)).Select(p => p.BoothMasterId).ToList()


            }).ToListAsync();
            return locationMasterRecord;
        }
        public async Task<Response> AddLocation(LocationModel locationModel)
        {
            try
            {
                var isStateActive = _context.StateMaster.Where(d => d.StateMasterId == locationModel.StateMasterId).FirstOrDefault();
                var isDistrictActive = _context.DistrictMaster.Where(d => d.StateMasterId == locationModel.StateMasterId && d.DistrictMasterId == locationModel.DistrictMasterId).FirstOrDefault();
                var isAssemblyActive = _context.AssemblyMaster.Where(d => d.StateMasterId == locationModel.StateMasterId && d.DistrictMasterId == locationModel.DistrictMasterId && d.AssemblyMasterId == locationModel.AssemblyMasterId).FirstOrDefault();

                if (isStateActive.StateStatus && isDistrictActive.DistrictStatus && isAssemblyActive.AssemblyStatus)
                {



                    // Check if a location with the same code already exists
                    var locationExist = _context.PollingLocationMaster
                .FirstOrDefault(p => p.LocationCode == locationModel.LocationCode &&
                                     p.StateMasterId == locationModel.StateMasterId &&
                                     p.AssemblyMasterId == locationModel.AssemblyMasterId &&
                                     p.DistrictMasterId == locationModel.DistrictMasterId);

                    if (locationExist != null)
                    {
                        return new Response { Status = RequestStatusEnum.BadRequest, Message = $"Location with code {locationModel.LocationCode} already exists in the State Assembly." };
                    }

                    // Check if a location with the same name already exists
                    var locationExistName = _context.PollingLocationMaster
                        .FirstOrDefault(p => p.LocationName == locationModel.LocationName &&
                                             p.StateMasterId == locationModel.StateMasterId &&
                                             p.AssemblyMasterId == locationModel.AssemblyMasterId &&
                                             p.DistrictMasterId == locationModel.DistrictMasterId);

                    if (locationExistName != null)
                    {
                        return new Response { Status = RequestStatusEnum.BadRequest, Message = $"Location with name {locationModel.LocationName} already exists in the State Assembly." };
                    }

                    //must check any booth already allocated with other location id

                    // Create a new PollingLocationMaster


                    string boothIdsAlreadyhaveLocationId = ""; string msg = "";
                    foreach (var boothMasterId in locationModel.BoothMasterId)
                    {
                        var boothRecord = _context.BoothMaster.SingleOrDefault(b => b.BoothMasterId == boothMasterId);
                        if (boothRecord != null)
                        {
                            if (boothRecord.LocationMasterId != null)
                            {
                                boothIdsAlreadyhaveLocationId = boothRecord.BoothCode_No + ",";
                            }
                        }
                    }

                    if (boothIdsAlreadyhaveLocationId == string.Empty)
                    {
                        locationModel.CreatedOn = BharatDateTime();
                        PollingLocationMaster lm = new PollingLocationMaster()
                        {
                            LocationName = locationModel.LocationName,
                            LocationCode = locationModel.LocationCode,
                            StateMasterId = locationModel.StateMasterId,
                            AssemblyMasterId = locationModel.AssemblyMasterId,
                            DistrictMasterId = locationModel.DistrictMasterId,
                            PCMasterId = locationModel.PCMasterId,
                            LocationLatitude = locationModel.LocationLatitude,
                            LocationLongitude = locationModel.LocationLongitude,
                            CreatedOn = locationModel.CreatedOn,
                            Status = locationModel.IsStatus
                        };

                        // Add the new location to the context
                        _context.PollingLocationMaster.Add(lm);
                        await _context.SaveChangesAsync(); // Use asynchronous save changes

                        if (locationModel.BoothMasterId != null && locationModel.BoothMasterId.Any())
                        {

                            foreach (var boothMasterId in locationModel.BoothMasterId)
                            {
                                var boothRecord = _context.BoothMaster.SingleOrDefault(b => b.BoothMasterId == boothMasterId);
                                if (boothRecord != null)
                                {
                                    boothRecord.LocationMasterId = lm.LocationMasterId; // Update with the newly created LocationMasterId
                                }
                            }

                            await _context.SaveChangesAsync(); // Use asynchronous save changes
                        }

                        msg = locationModel.LocationName + " " + "added & mapped successfully.";
                        return new Response { Status = RequestStatusEnum.OK, Message = msg };
                    }

                    else
                    {
                        msg = "These Booth Numbers Already have Location Mapped: (" + boothIdsAlreadyhaveLocationId + ")" + " " + "First Release its Location.";
                        return new Response { Status = RequestStatusEnum.BadRequest, Message = msg };
                    }
                }
                else
                {
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Please Check State,District,Assembly Status." };
                }

            }
            catch (Exception ex)
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = ex.Message };
            }
        }
        public async Task<Response> UpdateLocation(LocationModel locationModel)
        {
            try
            {
                var isStateActive = _context.StateMaster.Where(d => d.StateMasterId == locationModel.StateMasterId).FirstOrDefault();
                var isDistrictActive = _context.DistrictMaster.Where(d => d.StateMasterId == locationModel.StateMasterId && d.DistrictMasterId == locationModel.DistrictMasterId).FirstOrDefault();
                var isAssemblyActive = _context.AssemblyMaster.Where(d => d.StateMasterId == locationModel.StateMasterId && d.DistrictMasterId == locationModel.DistrictMasterId && d.AssemblyMasterId == locationModel.AssemblyMasterId).FirstOrDefault();


                if (isStateActive.StateStatus && isDistrictActive.DistrictStatus && isAssemblyActive.AssemblyStatus)
                {
                    if (locationModel.LocationMasterId != null && locationModel.LocationMasterId > 0)
                    {
                        var pollingStationRecord = _context.PollingLocationMaster
                            .FirstOrDefault(d => d.LocationMasterId == locationModel.LocationMasterId);

                        if (pollingStationRecord != null)
                        {
                            // Check if any booth is already allocated to a different location
                            string boothIdsAlreadyHaveLocationId = "";
                            foreach (var boothMasterId in locationModel.BoothMasterId)
                            {
                                var boothRecord = _context.BoothMaster.SingleOrDefault(b => b.BoothMasterId == boothMasterId);
                                if (boothRecord != null && boothRecord.LocationMasterId != null && boothRecord.LocationMasterId != locationModel.LocationMasterId)
                                {
                                    boothIdsAlreadyHaveLocationId += boothRecord.BoothCode_No + ",";
                                }
                            }

                            if (string.IsNullOrEmpty(boothIdsAlreadyHaveLocationId))
                            {
                                // Get the existing BoothMaster records associated with the LocationMasterId
                                var boothRecordList = _context.BoothMaster
                                    .Where(b => b.LocationMasterId == locationModel.LocationMasterId)
                                    .ToList();

                                // Set LocationMasterId to null for the existing BoothMaster records
                                foreach (var boothRecord in boothRecordList)
                                {
                                    boothRecord.LocationMasterId = null;
                                    _context.BoothMaster.Update(boothRecord);
                                }

                                await _context.SaveChangesAsync();

                                // Now, update BoothMaster records with the new LocationMasterId
                                foreach (var boothMasterId in locationModel.BoothMasterId)
                                {
                                    var boothRecordNew = _context.BoothMaster.SingleOrDefault(b => b.BoothMasterId == boothMasterId);
                                    if (boothRecordNew != null)
                                    {
                                        boothRecordNew.LocationMasterId = locationModel.LocationMasterId;
                                        _context.BoothMaster.Update(boothRecordNew);
                                    }
                                }

                                // Update Main Table (PollingLocationMaster)
                                pollingStationRecord.LocationName = locationModel.LocationName;
                                pollingStationRecord.LocationCode = locationModel.LocationCode;
                                pollingStationRecord.LocationLatitude = locationModel.LocationLatitude;
                                pollingStationRecord.LocationLongitude = locationModel.LocationLongitude;
                                pollingStationRecord.CreatedOn = BharatDateTime();
                                pollingStationRecord.Status = locationModel.IsStatus;
                                _context.PollingLocationMaster.Update(pollingStationRecord);

                                await _context.SaveChangesAsync(); // Use asynchronous save changes
                                return new Response { Status = RequestStatusEnum.OK, Message = "Polling Station Data Updated Successfully." };
                            }
                            else
                            {
                                string message = "These Booth Number(s) already have Location mapped: (" + boothIdsAlreadyHaveLocationId.TrimEnd(',') + "). First, release their Location or unselect from the list.";
                                return new Response { Status = RequestStatusEnum.BadRequest, Message = message };
                            }
                        }
                        else
                        {
                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Location Master Record not Found." };
                        }
                    }
                    else
                    {
                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Location Master Id cannot be null or less than/equal to zero." };
                    }
                }
                else
                {
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Please Check State,District,Assembly Sttaus." };
                }
            }
            catch (Exception ex)
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = ex.Message };
            }
        }


        #endregion

        #region Reports Booths,Sector Officer
        public async Task<List<ConsolidateBoothReport>> GetConsolidateBoothReports(BoothReportModel boothReportModel)
        {
            List<ConsolidateBoothReport> consolidateBoothReports = new List<ConsolidateBoothReport>();
            var state = _context.StateMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.StateStatus == true).Select(d => new { d.StateName, d.StateCode }).FirstOrDefault();
            var district = new { DistrictName = "", DistrictCode = "" };
            var pcMaster = new { PcName = "", PcCodeNo = "" };

            if (boothReportModel.DistrictMasterId is not 0)
            {
                district = _context.DistrictMaster
                    .Where(d => d.DistrictMasterId == boothReportModel.DistrictMasterId && d.StateMasterId == boothReportModel.StateMasterId && d.DistrictStatus == true)
                    .Select(d => new { d.DistrictName, d.DistrictCode })
                    .FirstOrDefault();
            }

            if (boothReportModel.PCMasterId is not 0)
            {
                pcMaster = _context.ParliamentConstituencyMaster
                    .Where(d => d.PCMasterId == boothReportModel.PCMasterId && d.PcStatus == true)
                    .Select(d => new { d.PcName, d.PcCodeNo })
                    .FirstOrDefault();
            }
            //State
            if (boothReportModel.StateMasterId is not 0 && boothReportModel.DistrictMasterId is 0 && boothReportModel.AssemblyMasterId is 0 && boothReportModel.PCMasterId is 0)
            {

                var assemblyList = _context.AssemblyMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.AssemblyStatus == true).Include(d => d.BoothMaster).ToList();

                foreach (var assembly in assemblyList)
                {

                    ConsolidateBoothReport report = new ConsolidateBoothReport
                    {
                        // Populate your ConsolidateBoothReport properties here based on assembly data
                        Header = $"{state.StateName}({state.StateCode})",
                        Title = $"{state.StateName}",
                        Type = "State",
                        Code = assembly.AssemblyCode.ToString(),
                        Name = assembly.AssemblyName,
                        DistrictName = _context.DistrictMaster.Where(d => d.DistrictMasterId == assembly.DistrictMasterId && d.DistrictStatus == true).Select(d => d.DistrictName).FirstOrDefault(),
                        TotalNumberOfBooths = assembly.TotalBooths,
                        TotalNumberOfBoothsEntered = assembly.BoothMaster.Count,
                        Male = assembly.BoothMaster.Select(d => d.Male).Sum(),
                        Female = assembly.BoothMaster.Select(d => d.Female).Sum(),
                        Trans = assembly.BoothMaster.Select(d => d.Transgender).Sum(),
                        Total = assembly.BoothMaster.Select(d => d.TotalVoters).Sum(),
                        IsStatus = assembly.AssemblyStatus

                    };
                    consolidateBoothReports.Add(report);
                }
                return consolidateBoothReports;
            }
            //District
            else if (boothReportModel.StateMasterId is not 0 && boothReportModel.DistrictMasterId is not 0 && boothReportModel.AssemblyMasterId is 0 && boothReportModel.PCMasterId is 0)
            {
                var assemblyList = _context.AssemblyMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId && d.AssemblyStatus == true).Include(d => d.BoothMaster).ToList();
                foreach (var assembly in assemblyList)
                {
                    ConsolidateBoothReport report = new ConsolidateBoothReport
                    {
                        // Populate your ConsolidateBoothReport properties here based on assembly data
                        Header = $"{state.StateName}({state.StateCode}),{district.DistrictName}({district.DistrictCode})",
                        Title = $"{district.DistrictName}",
                        Type = "District",
                        Code = assembly.AssemblyCode.ToString(),
                        Name = assembly.AssemblyName,
                        DistrictName = _context.DistrictMaster.Where(d => d.DistrictMasterId == assembly.DistrictMasterId && d.DistrictStatus == true).Select(d => d.DistrictName).FirstOrDefault(),
                        TotalNumberOfBooths = assembly.TotalBooths,
                        TotalNumberOfBoothsEntered = assembly.BoothMaster.Count,
                        Male = assembly.BoothMaster.Select(d => d.Male).Sum(),
                        Female = assembly.BoothMaster.Select(d => d.Female).Sum(),
                        Trans = assembly.BoothMaster.Select(d => d.Transgender).Sum(),
                        Total = assembly.BoothMaster.Select(d => d.TotalVoters).Sum(),
                        IsStatus = assembly.AssemblyStatus

                    };
                    consolidateBoothReports.Add(report);
                }
                return consolidateBoothReports;
            }
            //PC
            else if (boothReportModel.StateMasterId is not 0 && boothReportModel.DistrictMasterId is 0 && boothReportModel.PCMasterId is not 0 && boothReportModel.AssemblyMasterId is 0)
            {
                var assemblyList = _context.AssemblyMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.PCMasterId == boothReportModel.PCMasterId && d.AssemblyStatus == true).Include(d => d.BoothMaster).ToList();
                foreach (var assembly in assemblyList)
                {
                    ConsolidateBoothReport report = new ConsolidateBoothReport
                    {
                        // Populate your ConsolidateBoothReport properties here based on assembly data
                        Header = $"{state.StateName}({state.StateCode}),{pcMaster.PcName}({pcMaster.PcCodeNo})",
                        Title = $"{pcMaster.PcName}",
                        Type = "PC",
                        Code = assembly.AssemblyCode.ToString(),
                        Name = assembly.AssemblyName,
                        DistrictName = _context.DistrictMaster.Where(d => d.DistrictMasterId == assembly.DistrictMasterId && d.DistrictStatus == true).Select(d => d.DistrictName).FirstOrDefault(),

                        TotalNumberOfBooths = assembly.TotalBooths,
                        TotalNumberOfBoothsEntered = assembly.BoothMaster.Count,
                        Male = assembly.BoothMaster.Select(d => d.Male).Sum(),
                        Female = assembly.BoothMaster.Select(d => d.Female).Sum(),
                        Trans = assembly.BoothMaster.Select(d => d.Transgender).Sum(),
                        Total = assembly.BoothMaster.Select(d => d.TotalVoters).Sum(),
                        IsStatus = assembly.AssemblyStatus

                    };
                    consolidateBoothReports.Add(report);
                }
                return consolidateBoothReports;
            }
            //Assembly
            else if (boothReportModel.AssemblyMasterId is not 0)
            {

                if (boothReportModel.DistrictMasterId is not 0)
                {
                    var assembly = _context.AssemblyMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId && d.AssemblyMasterId == boothReportModel.AssemblyMasterId && d.AssemblyStatus == true).Include(d => d.BoothMaster).FirstOrDefault();
                    foreach (var booth in assembly.BoothMaster)
                    {
                        ConsolidateBoothReport report = new ConsolidateBoothReport
                        {
                            // Populate your ConsolidateBoothReport properties here based on assembly data
                            Header = $"{state.StateName}({state.StateCode}),{district.DistrictName}({district.DistrictCode}),{assembly.AssemblyName}({assembly.AssemblyCode})",
                            Title = $"{assembly.AssemblyName}",
                            Type = "Assembly",
                            Code = booth.BoothCode_No,
                            Name = booth.BoothName,
                            TotalNumberOfBooths = assembly.TotalBooths,
                            TotalNumberOfBoothsEntered = assembly.BoothMaster.Count,
                            DistrictName = _context.DistrictMaster.Where(d => d.DistrictMasterId == assembly.DistrictMasterId && d.DistrictStatus == true).Select(d => d.DistrictName).FirstOrDefault(),
                            LocationName = _context.PollingLocationMaster.Where(d => d.LocationMasterId == booth.LocationMasterId && d.Status == true).Select(d => d.LocationName).FirstOrDefault(),
                            Male = booth.Male,
                            Female = booth.Female,
                            Trans = booth.Transgender,
                            Total = booth.TotalVoters,
                            IsStatus = booth.BoothStatus

                        };
                        consolidateBoothReports.Add(report);
                    }
                    // Assuming consolidateBoothReports is a List<ConsolidateBoothReport>
                    var orderedReports = consolidateBoothReports.OrderBy(r => int.Parse(r.Code)).ToList();
                    return orderedReports;
                }
                else
                {
                    var assembly = _context.AssemblyMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.PCMasterId == boothReportModel.PCMasterId && d.AssemblyMasterId == boothReportModel.AssemblyMasterId && d.AssemblyStatus == true).Include(d => d.BoothMaster).FirstOrDefault();
                    foreach (var booth in assembly.BoothMaster)
                    {
                        ConsolidateBoothReport report = new ConsolidateBoothReport
                        {
                            // Populate your ConsolidateBoothReport properties here based on assembly data
                            Header = $"{state.StateName}({state.StateCode}),{pcMaster.PcName}({pcMaster.PcCodeNo}),{assembly.AssemblyName}({assembly.AssemblyCode})",
                            Title = $"{assembly.AssemblyName}",
                            Type = "Assembly",
                            Code = booth.BoothCode_No,
                            Name = booth.BoothName,
                            TotalNumberOfBooths = assembly.TotalBooths,
                            TotalNumberOfBoothsEntered = assembly.BoothMaster.Count,
                            DistrictName = _context.DistrictMaster.Where(d => d.DistrictMasterId == assembly.DistrictMasterId && d.DistrictStatus == true).Select(d => d.DistrictName).FirstOrDefault(),
                            Male = booth.Male,
                            Female = booth.Female,
                            Trans = booth.Transgender,
                            Total = booth.TotalVoters,
                            IsStatus = booth.BoothStatus

                        };
                        consolidateBoothReports.Add(report);
                    }
                    //return consolidateBoothReports;
                    var orderedReports = consolidateBoothReports.OrderBy(r => int.Parse(r.Code)).ToList();
                    return orderedReports;
                }


            }


            return null;
        }
        public async Task<List<SoReport>> GetSOReport(BoothReportModel boothReportModel)
        {

            if (boothReportModel.AssemblyMasterId is not 0)
            {
                List<SoReport> soReports = new List<SoReport>(); List<int> boothNumbers = new List<int>();
                if (boothReportModel.DistrictMasterId is not 0)
                {
                    var assembly = _context.AssemblyMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId && d.AssemblyMasterId == boothReportModel.AssemblyMasterId && d.AssemblyStatus == true).Include(d => d.BoothMaster).FirstOrDefault();
                    var sectorOfficerList = _context.SectorOfficerMaster.Where(d => d.StateMasterId == assembly.StateMasterId && d.SoAssemblyCode == assembly.AssemblyCode && d.SoStatus == true).ToList();
                    var state = _context.StateMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.StateStatus == true).FirstOrDefault();
                    var district = _context.DistrictMaster.Where(d => d.DistrictMasterId == boothReportModel.DistrictMasterId && d.DistrictStatus == true).FirstOrDefault();
                    var pollingLocation = _context.PollingLocationMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId && d.AssemblyMasterId == boothReportModel.AssemblyMasterId && d.Status == true).Count();
                    foreach (var so in sectorOfficerList)
                    {
                        var locationMasterId = _context.BoothMaster.Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true).Select(d => d.LocationMasterId).FirstOrDefault();
                        SoReport report = new SoReport
                        {
                            SoMasterId = so.SOMasterId,
                            Header = $"{state.StateName}({state.StateCode}),{district.DistrictName}({district.DistrictCode}),{assembly.AssemblyName}({assembly.AssemblyCode})",
                            Title = $"{district.DistrictName}",
                            Type = "District",
                            TotalBoothCount = assembly.BoothMaster.Count(),
                            TotalPollingLocationCount = pollingLocation,
                            TotalSOAppointedCount = sectorOfficerList.Count(),
                            SOName = so.SoName,
                            SOMobileNo = so.SoMobile,
                            Office = so.SoOfficeName,
                            SODesignation = so.SoDesignation,
                            BoothAllocatedCount = assembly.BoothMaster.Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true).Count(),
                            BoothAllocatedName = assembly.BoothMaster
                                                .Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true)
                                                .Select(d => $"{d.BoothName}({d.BoothCode_No})")
                                                .ToList(),

                            BoothLocationName = _context.PollingLocationMaster.Where(d => d.LocationMasterId == locationMasterId && d.Status == true).Select(d => $"{d.LocationName}-{d.LocationCode}").ToList(),



                        };
                        var AllocatedBoothNumbers = assembly.BoothMaster
                                            .Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true)
                                            .Select(d => $"{d.BoothCode_No}").ToList();
                        if (AllocatedBoothNumbers.Count > 0)
                        {
                            report.AllocatedBoothNumbers = string.Join(",", AllocatedBoothNumbers);
                            soReports.Add(report);
                        }


                    }
                    return soReports;
                }
                else
                {

                    // When Pass PC
                    var assembly = _context.AssemblyMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.PCMasterId == boothReportModel.PCMasterId && d.AssemblyMasterId == boothReportModel.AssemblyMasterId && d.AssemblyStatus == true).Include(d => d.BoothMaster).FirstOrDefault();
                    var sectorOfficerList = _context.SectorOfficerMaster.Where(d => d.StateMasterId == assembly.StateMasterId && d.SoAssemblyCode == assembly.AssemblyCode && d.SoStatus == true).ToList();
                    var state = _context.StateMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.StateStatus == true).FirstOrDefault();
                    var pcMaster = _context.ParliamentConstituencyMaster.Where(d => d.PCMasterId == boothReportModel.PCMasterId && d.PcStatus == true).FirstOrDefault();
                    var pollingLocation = _context.PollingLocationMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId && d.AssemblyMasterId == boothReportModel.AssemblyMasterId && d.Status == true).Count();
                    foreach (var so in sectorOfficerList)
                    {
                        var locationMasterId = _context.BoothMaster.Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true).Select(d => d.LocationMasterId).FirstOrDefault();
                        SoReport report = new SoReport
                        {
                            SoMasterId = so.SOMasterId,
                            Header = $"{state.StateName}({state.StateCode}),{pcMaster.PcName}({pcMaster.PcCodeNo}),{assembly.AssemblyName}({assembly.AssemblyCode})",
                            Title = $"{pcMaster.PcName}",
                            Type = "PC",
                            TotalBoothCount = assembly.BoothMaster.Count(),
                            TotalPollingLocationCount = pollingLocation,
                            TotalSOAppointedCount = sectorOfficerList.Count(),
                            SOName = so.SoName,
                            SOMobileNo = so.SoMobile,
                            Office = so.SoOfficeName,
                            SODesignation = so.SoDesignation,
                            BoothAllocatedCount = assembly.BoothMaster.Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true).Count(),
                            BoothAllocatedName = assembly.BoothMaster
                                                .Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true)
                                                .Select(d => $"{d.BoothName}, {d.BoothCode_No}")
                                                .ToList(),

                            BoothLocationName = _context.PollingLocationMaster.Where(d => d.LocationMasterId == locationMasterId).Select(d => $"{d.LocationName}-{d.LocationCode}").ToList(),



                        };

                        var AllocatedBoothNumbers = assembly.BoothMaster
                                           .Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true)
                                           .Select(d => $"{d.BoothCode_No}").ToList();
                        if (AllocatedBoothNumbers.Count > 0)
                        {
                            report.AllocatedBoothNumbers = string.Join(",", AllocatedBoothNumbers);
                            soReports.Add(report);
                        }

                    }
                    return soReports;
                }


            }
            else
            {
                return null;

            }

        }
        public async Task<List<SoReport>> GetPendingSOReport(BoothReportModel boothReportModel)
        {
            List<SoReport> soReports = new List<SoReport>();
            if (boothReportModel.StateMasterId != 0 && boothReportModel.DistrictMasterId == 0 && boothReportModel.PCMasterId == 0)
            {
                var assemblyList = _context.AssemblyMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId).ToList();
                var districtList = _context.DistrictMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId).ToList();
                var state = _context.StateMaster.FirstOrDefault(d => d.StateMasterId == boothReportModel.StateMasterId);
                var assignedSectorOfficerIds = _context.BoothMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId).Select(b => b.AssignedTo).ToList();
                var getUnassignedSO = _context.SectorOfficerMaster
                    .Where(som => som.StateMasterId == boothReportModel.StateMasterId && !assignedSectorOfficerIds.Contains(som.SOMasterId.ToString())).ToList();

                foreach (var so in getUnassignedSO)
                {
                    // Assuming you have pollingLocation and sectorOfficerList defined somewhere
                    var locationMasterId = _context.BoothMaster.Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true).Select(d => d.LocationMasterId).FirstOrDefault();
                    var assembly = assemblyList.FirstOrDefault(d => d.AssemblyCode == so.SoAssemblyCode);
                    var assemblyName = assembly?.AssemblyName ?? "Default Assembly Name";
                    var assemblyCode = assembly?.AssemblyCode ?? 0;
                    var districtName = districtList.FirstOrDefault(d => d.DistrictMasterId == (assembly?.DistrictMasterId))?.DistrictName ?? "Default District Name";
                    var districtCode = districtList.FirstOrDefault(d => d.DistrictMasterId == (assembly?.DistrictMasterId))?.DistrictCode ?? "0";

                    SoReport report = new SoReport
                    {
                        SoMasterId = so.SOMasterId,
                        Header = $"{state.StateName}({state.StateCode}))",
                        Title = $"{state.StateName}",
                        Type = "State",
                        //TotalBoothCount = assembly.Count(), // Assuming you want to count assemblies
                        //TotalPollingLocationCount = pollingLocation, // Assuming pollingLocation is defined
                        //                                             // TotalSOAppointedCount = sectorOfficerList.Count(), // Assuming sectorOfficerList is defined
                        SOName = so.SoName,
                        SOMobileNo = so.SoMobile,
                        Office = so.SoOfficeName,
                        SODesignation = so.SoDesignation,
                        BoothAllocatedCount = _context.BoothMaster.Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true).Count(),
                        BoothAllocatedName = _context.BoothMaster
                            .Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true)
                            .Select(d => $"{d.BoothName}({d.BoothCode_No})")
                            .ToList(),

                        BoothLocationName = _context.PollingLocationMaster.Where(d => d.LocationMasterId == locationMasterId && d.Status == true).Select(d => $"{d.LocationName}-{d.LocationCode}").ToList()
                    };

                    var AllocatedBoothNumbers = _context.BoothMaster
                        .Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true)
                        .Select(d => $"{d.BoothCode_No}").ToList();
                    if (AllocatedBoothNumbers.Count > 0)
                    {
                        report.AllocatedBoothNumbers = string.Join(",", AllocatedBoothNumbers);
                    }
                    soReports.Add(report);
                }
                return soReports;
            }

            if (boothReportModel.AssemblyMasterId is not 0)
            {
                List<int> boothNumbers = new List<int>();
                if (boothReportModel.DistrictMasterId is not 0)
                {
                    var assembly = _context.AssemblyMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId && d.AssemblyMasterId == boothReportModel.AssemblyMasterId && d.AssemblyStatus == true).Include(d => d.BoothMaster).FirstOrDefault();
                    var sectorOfficerList = _context.SectorOfficerMaster.Where(d => d.StateMasterId == assembly.StateMasterId && d.SoAssemblyCode == assembly.AssemblyCode && d.SoStatus == true).ToList();
                    var state = _context.StateMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.StateStatus == true).FirstOrDefault();
                    var district = _context.DistrictMaster.Where(d => d.DistrictMasterId == boothReportModel.DistrictMasterId && d.DistrictStatus == true).FirstOrDefault();
                    var pollingLocation = _context.PollingLocationMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId && d.AssemblyMasterId == boothReportModel.AssemblyMasterId && d.Status == true).Count();
                    foreach (var so in sectorOfficerList)
                    {
                        var locationMasterId = _context.BoothMaster.Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true).Select(d => d.LocationMasterId).FirstOrDefault();
                        SoReport report = new SoReport
                        {
                            SoMasterId = so.SOMasterId,
                            Header = $"{state.StateName}({state.StateCode}),{district.DistrictName}({district.DistrictCode}),{assembly.AssemblyName}({assembly.AssemblyCode})",
                            Title = $"{district.DistrictName}",
                            Type = "District",
                            TotalBoothCount = assembly.BoothMaster.Count(),
                            TotalPollingLocationCount = pollingLocation,
                            TotalSOAppointedCount = sectorOfficerList.Count(),
                            SOName = so.SoName,
                            SOMobileNo = so.SoMobile,
                            Office = so.SoOfficeName,
                            SODesignation = so.SoDesignation,
                            BoothAllocatedCount = assembly.BoothMaster.Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true).Count(),
                            BoothAllocatedName = assembly.BoothMaster
                                                .Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true)
                                                .Select(d => $"{d.BoothName}({d.BoothCode_No})")
                                                .ToList(),

                            BoothLocationName = _context.PollingLocationMaster.Where(d => d.LocationMasterId == locationMasterId && d.Status == true).Select(d => $"{d.LocationName}-{d.LocationCode}").ToList(),



                        };
                        var AllocatedBoothNumbers = assembly.BoothMaster
                                            .Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true)
                                            .Select(d => $"{d.BoothCode_No}").ToList();
                        if (AllocatedBoothNumbers.Count < 0)
                        {
                            report.AllocatedBoothNumbers = string.Join(",", AllocatedBoothNumbers);
                            soReports.Add(report);
                        }


                    }
                    return soReports;
                }
                else
                {

                    // When Pass PC
                    var assembly = _context.AssemblyMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.PCMasterId == boothReportModel.PCMasterId && d.AssemblyMasterId == boothReportModel.AssemblyMasterId && d.AssemblyStatus == true).Include(d => d.BoothMaster).FirstOrDefault();
                    var sectorOfficerList = _context.SectorOfficerMaster.Where(d => d.StateMasterId == assembly.StateMasterId && d.SoAssemblyCode == assembly.AssemblyCode && d.SoStatus == true).ToList();
                    var state = _context.StateMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.StateStatus == true).FirstOrDefault();
                    var pcMaster = _context.ParliamentConstituencyMaster.Where(d => d.PCMasterId == boothReportModel.PCMasterId && d.PcStatus == true).FirstOrDefault();
                    var pollingLocation = _context.PollingLocationMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId && d.AssemblyMasterId == boothReportModel.AssemblyMasterId && d.Status == true).Count();
                    foreach (var so in sectorOfficerList)
                    {
                        var locationMasterId = _context.BoothMaster.Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true).Select(d => d.LocationMasterId).FirstOrDefault();
                        SoReport report = new SoReport
                        {
                            SoMasterId = so.SOMasterId,
                            Header = $"{state.StateName}({state.StateCode}),{pcMaster.PcName}({pcMaster.PcCodeNo}),{assembly.AssemblyName}({assembly.AssemblyCode})",
                            Title = $"{pcMaster.PcName}",
                            Type = "PC",
                            TotalBoothCount = assembly.BoothMaster.Count(),
                            TotalPollingLocationCount = pollingLocation,
                            TotalSOAppointedCount = sectorOfficerList.Count(),
                            SOName = so.SoName,
                            SOMobileNo = so.SoMobile,
                            Office = so.SoOfficeName,
                            SODesignation = so.SoDesignation,
                            BoothAllocatedCount = assembly.BoothMaster.Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true).Count(),
                            BoothAllocatedName = assembly.BoothMaster
                                                .Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true)
                                                .Select(d => $"{d.BoothName}, {d.BoothCode_No}")
                                                .ToList(),

                            BoothLocationName = _context.PollingLocationMaster.Where(d => d.LocationMasterId == locationMasterId).Select(d => $"{d.LocationName}-{d.LocationCode}").ToList(),



                        };

                        var AllocatedBoothNumbers = assembly.BoothMaster
                                           .Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true)
                                           .Select(d => $"{d.BoothCode_No}").ToList();
                        if (AllocatedBoothNumbers.Count < 0)
                        {
                            report.AllocatedBoothNumbers = string.Join(",", AllocatedBoothNumbers);
                            soReports.Add(report);
                        }

                    }
                    return soReports;
                }


            }
            else
            {
                return null;

            }

        }


        //public async Task<List<SoReport>> GetPendingSOReport(BoothReportModel boothReportModel)
        //{
        //    List<SoReport> soReports = new List<SoReport>();

        //    if (boothReportModel.StateMasterId != 0 && boothReportModel.DistrictMasterId == 0 && boothReportModel.PCMasterId == 0)
        //    {
        //        var assemblyList = _context.AssemblyMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId).ToList();
        //        var districtList = _context.DistrictMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId).ToList();
        //        var state = _context.StateMaster.FirstOrDefault(d => d.StateMasterId == boothReportModel.StateMasterId);
        //        var assignedSectorOfficerIds = _context.BoothMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId).Select(b => b.AssignedTo).ToList();
        //        var getUnassignedSO = _context.SectorOfficerMaster
        //            .Where(som => som.StateMasterId == boothReportModel.StateMasterId && !assignedSectorOfficerIds.Contains(som.SOMasterId.ToString())).ToList();

        //        foreach (var so in getUnassignedSO)
        //        {
        //            // Assuming you have pollingLocation and sectorOfficerList defined somewhere
        //            var locationMasterId = _context.BoothMaster.Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true).Select(d => d.LocationMasterId).FirstOrDefault();
        //            var assembly = assemblyList.FirstOrDefault(d => d.AssemblyCode == so.SoAssemblyCode);
        //            var assemblyName = assembly?.AssemblyName ?? "Default Assembly Name";
        //            var assemblyCode = assembly?.AssemblyCode ?? 0;
        //            var districtName = districtList.FirstOrDefault(d => d.DistrictMasterId == (assembly?.DistrictMasterId))?.DistrictName ?? "Default District Name";
        //            var districtCode = districtList.FirstOrDefault(d => d.DistrictMasterId == (assembly?.DistrictMasterId))?.DistrictCode ?? "0";

        //            SoReport report = new SoReport
        //            {
        //                SoMasterId = so.SOMasterId,
        //                Header = $"{state.StateName}({state.StateCode}))",
        //                Title = $"{state.StateName}",
        //                Type = "State",
        //                //TotalBoothCount = assembly.Count(), // Assuming you want to count assemblies
        //                //TotalPollingLocationCount = pollingLocation, // Assuming pollingLocation is defined
        //                //                                             // TotalSOAppointedCount = sectorOfficerList.Count(), // Assuming sectorOfficerList is defined
        //                SOName = so.SoName,
        //                SOMobileNo = so.SoMobile,
        //                Office = so.SoOfficeName,
        //                SODesignation = so.SoDesignation,
        //                BoothAllocatedCount = _context.BoothMaster.Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true).Count(),
        //                BoothAllocatedName = _context.BoothMaster
        //                    .Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true)
        //                    .Select(d => $"{d.BoothName}({d.BoothCode_No})")
        //                    .ToList(),

        //                BoothLocationName = _context.PollingLocationMaster.Where(d => d.LocationMasterId == locationMasterId && d.Status == true).Select(d => $"{d.LocationName}-{d.LocationCode}").ToList()
        //            };

        //            var AllocatedBoothNumbers = _context.BoothMaster
        //                .Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true)
        //                .Select(d => $"{d.BoothCode_No}").ToList();
        //            if (AllocatedBoothNumbers.Count > 0)
        //            {
        //                report.AllocatedBoothNumbers = string.Join(",", AllocatedBoothNumbers);
        //            }
        //            soReports.Add(report);
        //        }
        //    }
        //    else if (boothReportModel.DistrictMasterId != 0)
        //    {
        //        if (boothReportModel.AssemblyMasterId == 0)
        //        {
        //            var assemblyList = _context.AssemblyMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId).ToList();
        //            var district = _context.DistrictMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId).FirstOrDefault();
        //            var state = _context.StateMaster.FirstOrDefault(d => d.StateMasterId == boothReportModel.StateMasterId);

        //            var assignedSectorOfficerIds = _context.BoothMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId).Select(b => b.AssignedTo).ToList();

        //            var assemblyCodes = assemblyList.Select(assembly => assembly.AssemblyCode).ToList();

        //            var getUnassignedSO = _context.SectorOfficerMaster
        //                .Where(som => som.StateMasterId == boothReportModel.StateMasterId && !assignedSectorOfficerIds.Contains(som.SOMasterId.ToString()) && assemblyCodes.Contains(som.SoAssemblyCode))
        //                .ToList();

        //            foreach (var so in getUnassignedSO)
        //            {
        //                // Assuming you have pollingLocation and sectorOfficerList defined somewhere
        //                var locationMasterId = _context.BoothMaster.Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true).Select(d => d.LocationMasterId).FirstOrDefault();
        //                var assembly = assemblyList.FirstOrDefault(d => d.AssemblyCode == so.SoAssemblyCode);
        //                var assemblyName = assembly?.AssemblyName ?? "Default Assembly Name";
        //                var assemblyCode = assembly?.AssemblyCode ?? 0;
        //                var districtName = district.DistrictName ?? "Default District Name";
        //                var districtCode = district.DistrictCode ?? "0";

        //                SoReport report = new SoReport
        //                {
        //                    SoMasterId = so.SOMasterId,
        //                    Header = $"{state.StateName}({state.StateCode}),{districtName}({districtCode})",
        //                    Title = $"{state.StateName}",
        //                    Type = "District",
        //                    SOName = so.SoName,
        //                    SOMobileNo = so.SoMobile,
        //                    Office = so.SoOfficeName,
        //                    SODesignation = so.SoDesignation,
        //                    BoothAllocatedCount = _context.BoothMaster.Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true).Count(),
        //                    BoothAllocatedName = _context.BoothMaster
        //                        .Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true)
        //                        .Select(d => $"{d.BoothName}({d.BoothCode_No})")
        //                        .ToList(),

        //                    BoothLocationName = _context.PollingLocationMaster.Where(d => d.LocationMasterId == locationMasterId && d.Status == true).Select(d => $"{d.LocationName}-{d.LocationCode}").ToList()
        //                };

        //                var AllocatedBoothNumbers = _context.BoothMaster
        //                    .Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true)
        //                    .Select(d => $"{d.BoothCode_No}").ToList();
        //                if (AllocatedBoothNumbers.Count > 0)
        //                {
        //                    report.AllocatedBoothNumbers = string.Join(",", AllocatedBoothNumbers);
        //                }
        //                soReports.Add(report);
        //            }
        //        }
        //        else if (boothReportModel.AssemblyMasterId != 0)
        //        {
        //            var assembly = _context.AssemblyMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId && d.AssemblyMasterId == boothReportModel.AssemblyMasterId).FirstOrDefault();
        //            var districtList = _context.DistrictMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId).FirstOrDefault();
        //            var state = _context.StateMaster.FirstOrDefault(d => d.StateMasterId == boothReportModel.StateMasterId);
        //            var assignedSectorOfficerIds = _context.BoothMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId).Select(b => b.AssignedTo).ToList();
        //            var getUnassignedSO = _context.SectorOfficerMaster
        //                .Where(som => som.StateMasterId == boothReportModel.StateMasterId && som.SoAssemblyCode == assembly.AssemblyCode && !assignedSectorOfficerIds.Contains(som.SOMasterId.ToString())).ToList();

        //            foreach (var so in getUnassignedSO)
        //            {
        //                // Assuming you have pollingLocation and sectorOfficerList defined somewhere
        //                var locationMasterId = _context.BoothMaster.Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true).Select(d => d.LocationMasterId).FirstOrDefault();

        //                var assemblyName = assembly?.AssemblyName ?? "Default Assembly Name";
        //                var assemblyCode = assembly?.AssemblyCode ?? 0;
        //                var districtName = districtList.DistrictName ?? "Default District Name";
        //                var districtCode = districtList.DistrictCode ?? "0";

        //                SoReport report = new SoReport
        //                {
        //                    SoMasterId = so.SOMasterId,
        //                    Header = $"{state.StateName}({state.StateCode}),{assembly.AssemblyName},{assembly.AssemblyCode})",
        //                    Title = $"{state.StateName}",
        //                    Type = "Assembly",
        //                    //TotalBoothCount = assembly.Count(), // Assuming you want to count assemblies
        //                    //TotalPollingLocationCount = pollingLocation, // Assuming pollingLocation is defined
        //                    //                                             // TotalSOAppointedCount = sectorOfficerList.Count(), // Assuming sectorOfficerList is defined
        //                    SOName = so.SoName,
        //                    SOMobileNo = so.SoMobile,
        //                    Office = so.SoOfficeName,
        //                    SODesignation = so.SoDesignation,
        //                    BoothAllocatedCount = _context.BoothMaster.Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true).Count(),
        //                    BoothAllocatedName = _context.BoothMaster
        //                        .Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true)
        //                        .Select(d => $"{d.BoothName}({d.BoothCode_No})")
        //                        .ToList(),

        //                    BoothLocationName = _context.PollingLocationMaster.Where(d => d.LocationMasterId == locationMasterId && d.Status == true).Select(d => $"{d.LocationName}-{d.LocationCode}").ToList()
        //                };

        //                var AllocatedBoothNumbers = _context.BoothMaster
        //                    .Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true)
        //                    .Select(d => $"{d.BoothCode_No}").ToList();
        //                if (AllocatedBoothNumbers.Count > 0)
        //                {
        //                    report.AllocatedBoothNumbers = string.Join(",", AllocatedBoothNumbers);
        //                }
        //                soReports.Add(report);
        //            }
        //        }
        //    }

        //    //else if (boothReportModel.PCMasterId != 0)
        //    //{
        //    //    var assemblyList = _context.AssemblyMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.PCMasterId == boothReportModel.PCMasterId).ToList();
        //    //    var district = _context.ParliamentConstituencyMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.PCMasterId == boothReportModel.PCMasterId).FirstOrDefault();
        //    //    var state = _context.StateMaster.FirstOrDefault(d => d.StateMasterId == boothReportModel.StateMasterId);

        //    //    var assignedSectorOfficerIds = _context.BoothMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.pc == boothReportModel.DistrictMasterId).Select(b => b.AssignedTo).ToList();

        //    //    var assemblyCodes = assemblyList.Select(assembly => assembly.AssemblyCode).ToList();

        //    //    var getUnassignedSO = _context.SectorOfficerMaster
        //    //        .Where(som => som.StateMasterId == boothReportModel.StateMasterId && !assignedSectorOfficerIds.Contains(som.SOMasterId.ToString()) && assemblyCodes.Contains(som.SoAssemblyCode))
        //    //        .ToList();

        //    //    foreach (var so in getUnassignedSO)
        //    //    {
        //    //        // Assuming you have pollingLocation and sectorOfficerList defined somewhere
        //    //        var locationMasterId = _context.BoothMaster.Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true).Select(d => d.LocationMasterId).FirstOrDefault();
        //    //        var assembly = assemblyList.FirstOrDefault(d => d.AssemblyCode == so.SoAssemblyCode);
        //    //        var assemblyName = assembly?.AssemblyName ?? "Default Assembly Name";
        //    //        var assemblyCode = assembly?.AssemblyCode ?? 0;
        //    //        var districtName = district.DistrictName ?? "Default District Name";
        //    //        var districtCode = district.DistrictCode ?? "0";

        //    //        SoReport report = new SoReport
        //    //        {
        //    //            SoMasterId = so.SOMasterId,
        //    //            Header = $"{state.StateName}({state.StateCode}),{districtName}({districtCode})",
        //    //            Title = $"{state.StateName}",
        //    //            Type = "District",
        //    //            SOName = so.SoName,
        //    //            SOMobileNo = so.SoMobile,
        //    //            Office = so.SoOfficeName,
        //    //            SODesignation = so.SoDesignation,
        //    //            BoothAllocatedCount = _context.BoothMaster.Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true).Count(),
        //    //            BoothAllocatedName = _context.BoothMaster
        //    //                .Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true)
        //    //                .Select(d => $"{d.BoothName}({d.BoothCode_No})")
        //    //                .ToList(),

        //    //            BoothLocationName = _context.PollingLocationMaster.Where(d => d.LocationMasterId == locationMasterId && d.Status == true).Select(d => $"{d.LocationName}-{d.LocationCode}").ToList()
        //    //        };

        //    //        var AllocatedBoothNumbers = _context.BoothMaster
        //    //            .Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true)
        //    //            .Select(d => $"{d.BoothCode_No}").ToList();
        //    //        if (AllocatedBoothNumbers.Count > 0)
        //    //        {
        //    //            report.AllocatedBoothNumbers = string.Join(",", AllocatedBoothNumbers);
        //    //        }
        //    //        soReports.Add(report);
        //    //    }

        //    //}


        //    return soReports;
        //}



        public async Task<List<AssemblyWisePendingBooth>> GetAssemblyWisePendingReports(string stateMasterId)
        {
            List<AssemblyWisePendingBooth> assemblyWisePendingBooths = new List<AssemblyWisePendingBooth>();
            var assemblyList = await _context.AssemblyMaster.Include(d => d.BoothMaster).Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId)).ToListAsync();
            foreach (var assembly in assemblyList)
            {
                AssemblyWisePendingBooth assemblyWisePendingBooth = new AssemblyWisePendingBooth()
                {
                    AssemblyMasterId = assembly.AssemblyMasterId,
                    DistrictName = _context.DistrictMaster.Where(d => d.DistrictMasterId == assembly.DistrictMasterId).Select(d => d.DistrictName).FirstOrDefault(),
                    AssemblyName = assembly.AssemblyName,
                    AssemblyCode = assembly.AssemblyCode,
                    TotalBooth = assembly.BoothMaster.Count

                };
                assemblyWisePendingBooths.Add(assemblyWisePendingBooth);
            }
            return assemblyWisePendingBooths;
        }
        #endregion

        #region Polling Station - P05 Report
        public async Task<List<VTPSReportReportModel>> GetVoterTurnOutPollingStationReports(BoothReportModel boothReportModel)
        {
            List<VTPSReportReportModel> consolidateBoothReports = new List<VTPSReportReportModel>();
            var state = _context.StateMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.StateStatus == true).Select(d => new { d.StateName, d.StateCode }).FirstOrDefault();
            var district = new { DistrictName = "", DistrictCode = "" };
            var pcMaster = new { PcName = "", PcCodeNo = "" };

            if (boothReportModel.DistrictMasterId is not 0)
            {
                district = _context.DistrictMaster.Where(d => d.DistrictMasterId == boothReportModel.DistrictMasterId && d.StateMasterId == boothReportModel.StateMasterId && d.DistrictStatus == true).Select(d => new { d.DistrictName, d.DistrictCode }).FirstOrDefault();
            }

            if (boothReportModel.PCMasterId is not 0)
            {
                pcMaster = _context.ParliamentConstituencyMaster.Where(d => d.PCMasterId == boothReportModel.PCMasterId && d.PcStatus == true).Select(d => new { d.PcName, d.PcCodeNo }).FirstOrDefault();
            }

            //State 
            if (boothReportModel.StateMasterId is not 0 && boothReportModel.DistrictMasterId is 0 && boothReportModel.AssemblyMasterId is 0 && boothReportModel.PCMasterId is 0)
            {

                var assemblyList = _context.AssemblyMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.AssemblyStatus == true).ToList();

                foreach (var assembly in assemblyList)
                {
                    var pollingStationData = _context.PollingStationMaster.Include(psm => psm.PollingStationGender).Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId).ToList();
                    VTPSReportReportModel report = new VTPSReportReportModel
                    {
                        // Populate your ConsolidateBoothReport properties here based on assembly data
                        Header = $"{state.StateName}({state.StateCode})",
                        Title = $"{state.StateName}",
                        Type = "State",
                        DistrictName = district.DistrictName,
                        DistrictCode = district.DistrictCode,
                        AssemblyName = assemblyList.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId).Select(p => p.AssemblyName).FirstOrDefault(),
                        AssemblyCode = assemblyList.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId).Select(p => p.AssemblyCode.ToString()).FirstOrDefault(),

                        //AssemblyCode= _context.PollingStationMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId && d.AssemblyMasterId == boothReportModel.AssemblyMasterId).Select(p=>p.AssemblySegmentNo).FirstOrDefault(),
                        EPIC = pollingStationData.Sum(psm => psm.VotePolledEPIC),
                        TotalEVMS = _context.PollingStationMaster.Where(d => d.AssemblyMasterId == boothReportModel.AssemblyMasterId && d.EVMReplaced).Count(),
                        Male = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2).Sum(gender => gender.Male)),
                        Female = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2).Sum(gender => gender.Female)),
                        ThirdGender = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2).Sum(gender => gender.ThirdGender)),
                        Total = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2).Sum(gender => gender.Total)),
                        OverseasElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 4).Sum(gender => gender.Total)), //overseas
                        YoungElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 3).Sum(gender => gender.Total)), // young electrl
                        PWdEelectoral = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1).Sum(gender => gender.Total)), // no of electrl pwd
                                                                                                                                                                                                    //VotePolledOtherDocument= pollingStationData.Sum(psm =>Convert.ToInt16(psm.VotePolledOtherDocument)),
                        VotePolledOtherDocument = pollingStationData.Sum(psm =>
                        {
                            if (int.TryParse(psm.VotePolledOtherDocument, out int result))
                            {
                                return result;
                            }
                            else
                            {
                                return 0;
                            }
                        }),
                        TenderedVotes = pollingStationData.Sum(psm => psm.TenderedVote),
                    };
                    consolidateBoothReports.Add(report);
                }
                return consolidateBoothReports;
            }

            //District
            else if (boothReportModel.StateMasterId is not 0 && boothReportModel.DistrictMasterId is not 0 && boothReportModel.AssemblyMasterId is 0 && boothReportModel.PCMasterId is 0)
            {
                var assemblyList = _context.AssemblyMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId && d.AssemblyStatus == true).ToList();
                foreach (var assembly in assemblyList)
                {
                    var pollingStationData = _context.PollingStationMaster.Include(psm => psm.PollingStationGender).Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId
                     && d.AssemblyMasterId == assembly.AssemblyMasterId)
            .ToList();
                    VTPSReportReportModel report = new VTPSReportReportModel
                    {
                        // Populate your ConsolidateBoothReport properties here based on assembly data
                        Header = $"{state.StateName}({state.StateCode})",
                        Title = $"{state.StateName}",
                        Type = "District",
                        DistrictName = district.DistrictName,
                        DistrictCode = district.DistrictCode,
                        AssemblyName = assemblyList.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId).Select(p => p.AssemblyName).FirstOrDefault(),
                        AssemblyCode = assemblyList.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId).Select(p => p.AssemblyCode.ToString()).FirstOrDefault(),

                        //AssemblyCode= _context.PollingStationMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId && d.AssemblyMasterId == boothReportModel.AssemblyMasterId).Select(p=>p.AssemblySegmentNo).FirstOrDefault(),
                        EPIC = pollingStationData.Sum(psm => psm.VotePolledEPIC),
                        TotalEVMS = _context.PollingStationMaster.Where(d => d.AssemblyMasterId == boothReportModel.AssemblyMasterId && d.EVMReplaced).Count(),
                        Male = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2).Sum(gender => gender.Male)),
                        Female = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2).Sum(gender => gender.Female)),
                        ThirdGender = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2).Sum(gender => gender.ThirdGender)),
                        Total = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2).Sum(gender => gender.Total)),
                        OverseasElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 4).Sum(gender => gender.Total)), //overseas
                        YoungElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 3).Sum(gender => gender.Total)), // young electrl
                        PWdEelectoral = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1).Sum(gender => gender.Total)), // no of electrl pwd
                                                                                                                                                                                                    //VotePolledOtherDocument= pollingStationData.Sum(psm =>Convert.ToInt16(psm.VotePolledOtherDocument)),
                        VotePolledOtherDocument = pollingStationData.Sum(psm =>
                        {
                            if (int.TryParse(psm.VotePolledOtherDocument, out int result))
                            {
                                return result;
                            }
                            else
                            {
                                // Handle non-numeric values, for example, you can log or set a default value
                                // In this case, I'll set it to 0, but you can adjust based on your requirements
                                return 0;
                            }
                        }),
                        TenderedVotes = pollingStationData.Sum(psm => psm.TenderedVote),

                    };
                    consolidateBoothReports.Add(report);
                }
                return consolidateBoothReports;
            }

            //PC
            else if (boothReportModel.StateMasterId is not 0 && boothReportModel.DistrictMasterId is 0 && boothReportModel.PCMasterId is not 0 && boothReportModel.AssemblyMasterId is 0)
            {
                var assemblyList = _context.AssemblyMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.PCMasterId == boothReportModel.PCMasterId && d.AssemblyStatus == true).ToList();
                foreach (var assembly in assemblyList)
                {
                    var pollingStationData = await _context.PollingStationMaster
            .Include(psm => psm.PollingStationGender)
            .Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId).ToListAsync();
                    VTPSReportReportModel report = new VTPSReportReportModel
                    {
                        // Populate your ConsolidateBoothReport properties here based on assembly data
                        Header = $"{state.StateName}({state.StateCode}),{pcMaster.PcName}({pcMaster.PcCodeNo})",
                        Title = $"{pcMaster.PcName}",
                        Type = "PC",
                        Code = assembly.AssemblyCode.ToString(),
                        Name = assembly.AssemblyName,
                        PCCode = pcMaster.PcCodeNo,
                        PCName = pcMaster.PcName,
                        AssemblyName = assemblyList.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.PCMasterId == boothReportModel.PCMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId).Select(p => p.AssemblyName).FirstOrDefault(),
                        AssemblyCode = assemblyList.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.PCMasterId == boothReportModel.PCMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId).Select(p => p.AssemblyCode.ToString()).FirstOrDefault(),

                        EPIC = pollingStationData.Sum(psm => psm.VotePolledEPIC),
                        TotalEVMS = _context.PollingStationMaster.Where(d => d.AssemblyMasterId == boothReportModel.AssemblyMasterId && d.EVMReplaced).Count(),
                        Male = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2).Sum(gender => gender.Male)),
                        Female = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2).Sum(gender => gender.Female)),
                        ThirdGender = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2).Sum(gender => gender.ThirdGender)),
                        Total = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2).Sum(gender => gender.Total)),
                        OverseasElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 4).Sum(gender => gender.Total)), //overseas
                        YoungElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 3).Sum(gender => gender.Total)), // young electrl
                        PWdEelectoral = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1).Sum(gender => gender.Total)), // no of electrl pwd
                        VotePolledOtherDocument = pollingStationData.Sum(psm =>
                        {
                            if (int.TryParse(psm.VotePolledOtherDocument, out int result))
                            {
                                return result;
                            }
                            else
                            {
                                // Handle non-numeric values, for example, you can log or set a default value
                                // In this case, I'll set it to 0, but you can adjust based on your requirements
                                return 0;
                            }
                        }),
                        TenderedVotes = pollingStationData.Sum(psm => psm.TenderedVote),

                    };
                    consolidateBoothReports.Add(report);
                }
                return consolidateBoothReports;
            }

            //Assembly
            else if (boothReportModel.AssemblyMasterId is not 0)
            {

                if (boothReportModel.DistrictMasterId is not 0)
                {
                    var assemblyList = _context.AssemblyMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId && d.AssemblyStatus == true).ToList();
                    foreach (var assembly in assemblyList)
                    {
                        var pollingStationData = _context.PollingStationMaster.Include(psm => psm.PollingStationGender).Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId
                         && d.AssemblyMasterId == assembly.AssemblyMasterId)
                .ToList();
                        VTPSReportReportModel report = new VTPSReportReportModel
                        {
                            Header = $"{state.StateName}({state.StateCode}),{district.DistrictName}({district.DistrictCode}),{assembly.AssemblyName}({assembly.AssemblyCode})",
                            Title = $"{assembly.AssemblyName}",
                            Type = "Assembly",
                            DistrictName = district.DistrictName,
                            DistrictCode = district.DistrictCode,
                            AssemblyName = assemblyList.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId).Select(p => p.AssemblyName).FirstOrDefault(),
                            AssemblyCode = assemblyList.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId).Select(p => p.AssemblyCode.ToString()).FirstOrDefault(),

                            //AssemblyCode= _context.PollingStationMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId && d.AssemblyMasterId == boothReportModel.AssemblyMasterId).Select(p=>p.AssemblySegmentNo).FirstOrDefault(),
                            EPIC = pollingStationData.Sum(psm => psm.VotePolledEPIC),
                            TotalEVMS = _context.PollingStationMaster.Where(d => d.AssemblyMasterId == boothReportModel.AssemblyMasterId && d.EVMReplaced).Count(),
                            Male = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2).Sum(gender => gender.Male)),
                            Female = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2).Sum(gender => gender.Female)),
                            ThirdGender = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2).Sum(gender => gender.ThirdGender)),
                            Total = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2).Sum(gender => gender.Total)),
                            OverseasElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 4).Sum(gender => gender.Total)), //overseas
                            YoungElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 3).Sum(gender => gender.Total)), // young electrl
                            PWdEelectoral = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1).Sum(gender => gender.Total)), // no of electrl pwd
                                                                                                                                                                                                        //VotePolledOtherDocument= pollingStationData.Sum(psm =>Convert.ToInt16(psm.VotePolledOtherDocument)),
                            VotePolledOtherDocument = pollingStationData.Sum(psm =>
                            {
                                if (int.TryParse(psm.VotePolledOtherDocument, out int result))
                                {
                                    return result;
                                }
                                else
                                {

                                    return 0;
                                }
                            }),
                            TenderedVotes = pollingStationData.Sum(psm => psm.TenderedVote),

                        };
                        consolidateBoothReports.Add(report);
                    }
                    return consolidateBoothReports;
                }
                else
                {
                    var assemblyList = _context.AssemblyMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.PCMasterId == boothReportModel.PCMasterId && d.AssemblyStatus == true).ToList();
                    foreach (var assembly in assemblyList)
                    {
                        var pollingStationData = await _context.PollingStationMaster
                .Include(psm => psm.PollingStationGender)
                .Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId)
                .ToListAsync();
                        VTPSReportReportModel report = new VTPSReportReportModel
                        {
                            // Populate your ConsolidateBoothReport properties here based on assembly data

                            Header = $"{state.StateName}({state.StateCode}),{pcMaster.PcName}({pcMaster.PcCodeNo}),{assembly.AssemblyName}({assembly.AssemblyCode})",
                            Title = $"{assembly.AssemblyName}",
                            Type = "Assembly",
                            Code = assembly.AssemblyCode.ToString(),
                            Name = assembly.AssemblyName,
                            PCCode = pcMaster.PcCodeNo,
                            PCName = pcMaster.PcName,
                            AssemblyName = assemblyList.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.PCMasterId == boothReportModel.PCMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId).Select(p => p.AssemblyName).FirstOrDefault(),
                            AssemblyCode = assemblyList.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.PCMasterId == boothReportModel.PCMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId).Select(p => p.AssemblyCode.ToString()).FirstOrDefault(),

                            EPIC = pollingStationData.Sum(psm => psm.VotePolledEPIC),
                            TotalEVMS = _context.PollingStationMaster.Where(d => d.AssemblyMasterId == boothReportModel.AssemblyMasterId && d.EVMReplaced).Count(),
                            Male = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2).Sum(gender => gender.Male)),
                            Female = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2).Sum(gender => gender.Female)),
                            ThirdGender = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2).Sum(gender => gender.ThirdGender)),
                            Total = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2).Sum(gender => gender.Total)),
                            OverseasElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 4).Sum(gender => gender.Total)), //overseas
                            YoungElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 3).Sum(gender => gender.Total)), // young electrl
                            PWdEelectoral = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1).Sum(gender => gender.Total)), // no of electrl pwd
                            VotePolledOtherDocument = pollingStationData.Sum(psm =>
                            {
                                if (int.TryParse(psm.VotePolledOtherDocument, out int result))
                                {
                                    return result;
                                }
                                else
                                {

                                    return 0;
                                }
                            }),
                            TenderedVotes = pollingStationData.Sum(psm => psm.TenderedVote),

                        };
                        consolidateBoothReports.Add(report);
                    }
                    return consolidateBoothReports;
                }


            }



            return null;
        }
        #endregion

        #region GetDashBoardCount
        public async Task<DashBoardRealTimeCount> GetDashBoardCount(ClaimsIdentity claimsIdentity)
        {
            var rolesClaim = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
            var roles = rolesClaim?.Value;

            var stateMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "StateMasterId")?.Value;
            var assemblyMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "AssemblyMasterId")?.Value;
            var districtMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "DistrictMasterId")?.Value;
            var pcMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "PCMasterId")?.Value;

            if (IsSuperAdminOrECI(roles))
            {
                return await GetDashboardCountForSuperAdminOrECI(roles, claimsIdentity);
            }
            else if (roles == "StateAdmin")
            {
                return await GetDashboardCountForSuperAdminOrECI(roles, claimsIdentity);

            }
            else if (roles == "DistrictAdmin")
            {
                return await GetDashboardCountForDistrictAdmin(claimsIdentity);

            }
            else if (roles == "PC")
            {
                return await GetDashboardCountForPCAdmin(claimsIdentity);

            }
            else
            {
                return await GetDashboardCountForARO(claimsIdentity);
            }
        }

        private bool IsSuperAdminOrECI(string roles)
        {
            return roles == "SuperAdmin" || roles == "ECI";
        }
        private bool IsStateAdmin(string roles)
        {
            return roles == "StateAdmin";
        }

        #region Dashboard Count
        //private async Task<DashBoardRealTimeCount> GetDashboardCountForSuperAdminOrECI1(string roles, ClaimsIdentity claimsIdentity)
        //{
        //    var stateMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "StateMasterId")?.Value;

        //    // Establish a connection to the PostgreSQL database
        //    await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("Postgres"));
        //    await connection.OpenAsync();

        //    var command = new NpgsqlCommand("SELECT * FROM get_election_Dashboard_counts_statewise(@state_master_id)", connection);
        //    command.Parameters.AddWithValue("@state_master_id", Convert.ToInt32(stateMasterId));
        //    // Count specific events

        //    var partyDispatchCount = await _context.ElectionInfoMaster
        //        .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.IsPartyDispatched == true)
        //        .CountAsync();

        //    var partyArrivedCount = await _context.ElectionInfoMaster
        //        .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.IsPartyReached == true)
        //        .CountAsync();

        //    var setupPollingStationCount = await _context.ElectionInfoMaster
        //        .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.IsSetupOfPolling == true)
        //        .CountAsync();

        //    var mockPollDoneCount = await _context.ElectionInfoMaster
        //        .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.IsMockPollDone == true)
        //        .CountAsync();

        //    var pollStartedCount = await _context.ElectionInfoMaster
        //        .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.IsPollStarted == true)
        //        .CountAsync();

        //    var pollEndedCount = await _context.ElectionInfoMaster
        //        .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.IsPollEnded == true)
        //        .CountAsync();

        //    var evmVVPAOffCount = await _context.ElectionInfoMaster
        //        .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.IsMCESwitchOff == true)
        //        .CountAsync();

        //    var partyDepartedCount = await _context.ElectionInfoMaster
        //        .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.IsPartyDeparted == true)
        //        .CountAsync();

        //    var partyReachedAtCollectionCount = await _context.ElectionInfoMaster
        //        .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.IsPartyReachedCollectionCenter == true)
        //        .CountAsync();

        //    var evmDepositedCount = await _context.ElectionInfoMaster
        //        .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.IsEVMDeposited == true)
        //        .CountAsync();


        //    // Calculate total booth count
        //    var totalBoothCount = await _context.BoothMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.BoothStatus == true).CountAsync();

        //    // Calculate total votes polled
        //    var totalVotesPolledCount = await _context.PollDetails
        //        .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId))
        //        .GroupBy(d => d.BoothMasterId)
        //        .Select(group => group.OrderByDescending(d => d.VotesPolledRecivedTime).FirstOrDefault().VotesPolled)
        //        .SumAsync();

        //    // Calculate total voters count
        //    var totalVotersCount = await _context.BoothMaster
        //        .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.BoothStatus == true)
        //        .SumAsync(d => d.TotalVoters);

        //    // Calculate total voters in queue
        //    var totalVotersInQueue = await _context.ElectionInfoMaster
        //        .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId))
        //        .SumAsync(d => d.VoterInQueue);

        //    // Calculate total final votes
        //    var totalFinalVotesCount = await _context.ElectionInfoMaster
        //        .Where(d => d.FinalTVoteStatus == true && d.StateMasterId == Convert.ToInt32(stateMasterId))
        //        .SumAsync(d => d.FinalTVote);

        //    // Calculate total EDC votes
        //    var eDCVoteCount = await _context.ElectionInfoMaster
        //       .Where(d => d.FinalTVoteStatus == true && d.StateMasterId == Convert.ToInt32(stateMasterId))
        //       .SumAsync(d => d.EDC);



        //    // Create and populate EventCount objects
        //    var partyDispatchEvent = new EventCount { EventName = "PartyDispatch", Count = partyDispatchCount };
        //    var partyArrivedEvent = new EventCount { EventName = "PartyArrived", Count = partyArrivedCount };
        //    var setupPollingStationEvent = new EventCount { EventName = "SetupPollingStation", Count = setupPollingStationCount };
        //    var mockPollDoneEvent = new EventCount { EventName = "MockPollDone", Count = mockPollDoneCount };
        //    var pollStartedEvent = new EventCount { EventName = "PollStarted", Count = pollStartedCount };
        //    var votesPolledEvent = new EventCount
        //    {
        //        EventName = "VotesPolled",
        //        VotesPolledCount = totalVotesPolledCount,
        //        VotesPolledPercentage = Math.Round((decimal)(totalVotesPolledCount * 100.0 / totalVotersCount), 1),
        //        TotalVotersCount = totalVotersCount
        //    };
        //    var voterInQueueEvent = new EventCount
        //    {
        //        EventName = "VoterInQueue",
        //        FinalVotesCount = totalVotersInQueue
        //    };

        //    var eDCVotesEvent = new EventCount
        //    {
        //        EventName = "EDCVoters",
        //        //TotalVotersCount = totalVotersCount,
        //        FinalVotesCount = eDCVoteCount,
        //        //FinalVotesPercentage = Math.Round((decimal)(totalFinalVotesCount * 100.0 / totalVotersCount), 1),
        //    };

        //    var finalVotesEvent = new EventCount
        //    {
        //        EventName = "FinalVoteDone",

        //        TotalVotersCount = totalVotersCount,
        //        FinalVotesCount = totalFinalVotesCount + eDCVoteCount,
        //        FinalVotesPercentage = Math.Round((decimal)(totalFinalVotesCount * 100.0 / totalVotersCount), 1),
        //    };
        //    var pollEndedEvent = new EventCount { EventName = "PollEnded", Count = pollEndedCount };
        //    var evmVVPAOffEvent = new EventCount { EventName = "EVMVVPATOff", Count = evmVVPAOffCount };
        //    var partyDepartedEvent = new EventCount { EventName = "PartyDeparted", Count = partyDepartedCount };
        //    var partyReachedAtCollectionEvent = new EventCount { EventName = "PartyReachedAtCollection", Count = partyReachedAtCollectionCount };
        //    var evmDepositedEvent = new EventCount { EventName = "EVMDeposited", Count = evmDepositedCount };

        //    // Create and populate DashboardCount object
        //    var dashboardCount = new DashBoardRealTimeCount
        //    {
        //        Total = totalBoothCount,
        //        Events = new List<EventCount>
        //{
        //    partyDispatchEvent,
        //    partyArrivedEvent,
        //    setupPollingStationEvent,
        //    mockPollDoneEvent,
        //    pollStartedEvent,
        //     votesPolledEvent,
        //    voterInQueueEvent,
        //    finalVotesEvent,
        //    pollEndedEvent,
        //    evmVVPAOffEvent,
        //    partyDepartedEvent,
        //    partyReachedAtCollectionEvent,
        //    evmDepositedEvent,
        //    eDCVotesEvent

        //}
        //    };

        //    return dashboardCount;
        //}

        private async Task<DashBoardRealTimeCount> GetDashboardCountForSuperAdminOrECI(string roles, ClaimsIdentity claimsIdentity)
        {
            var stateMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "StateMasterId")?.Value;

            // Establish a connection to the PostgreSQL database
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("Postgres"));
            await connection.OpenAsync();
            var totalVotersCount = await _context.BoothMaster
           .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.BoothStatus == true)
              .SumAsync(d => d.TotalVoters);
            // Create a command to call the PostgreSQL function
            var command = new NpgsqlCommand("SELECT * FROM get_election_Dashboard_counts_statewise(@state_master_id)", connection);
            command.Parameters.AddWithValue("@state_master_id", Convert.ToInt32(stateMasterId));

            // Execute the command and retrieve the result set
            await using var reader = await command.ExecuteReaderAsync();

            // Read the result set and populate the DashboardCount object
            if (await reader.ReadAsync())
            {
                var dashboardCount = new DashBoardRealTimeCount
                {
                    Total = reader.GetInt32(10), // total_booth_count
                    Events = new List<EventCount>
                    {
                new EventCount { EventName = "PartyDispatch", Count = reader.GetInt32(0) },
                new EventCount { EventName = "PartyArrived", Count = reader.GetInt32(1) },
                new EventCount { EventName = "SetupPollingStation", Count = reader.GetInt32(2) },
                new EventCount { EventName = "MockPollDone", Count = reader.GetInt32(3) },
                new EventCount { EventName = "PollStarted", Count = reader.GetInt32(4) },
                     new EventCount {EventName = "VotesPolled", VotesPolledCount = reader.GetInt32(11) ,VotesPolledPercentage = Math.Round((decimal)(reader.GetInt32(11) * 100.0 / totalVotersCount), 1),
                TotalVotersCount = totalVotersCount
                },
                new EventCount{EventName = "VoterInQueue",FinalVotesCount = reader.GetInt32(13)  },
                new EventCount {EventName = "FinalVoteDone", TotalVotersCount = reader.GetInt32(12), // Total voters count
                    FinalVotesCount = reader.GetInt32(14) + reader.GetInt32(15), // Total final votes count + EDC vote count
                    FinalVotesPercentage = Math.Round((decimal)(reader.GetInt32(14) * 100.0 / reader.GetInt32(12)), 1) // Final votes percentage
                },

                new EventCount { EventName = "PollEnded", Count = reader.GetInt32(5) },
                new EventCount { EventName = "EVMVVPATOff", Count = reader.GetInt32(6) },
                new EventCount { EventName = "PartyDeparted", Count = reader.GetInt32(7) },
                new EventCount { EventName = "PartyReachedAtCollection", Count = reader.GetInt32(8) },
                new EventCount { EventName = "EVMDeposited", Count = reader.GetInt32(9) },
                  new EventCount { EventName = "EDCVoters", FinalVotesCount = reader.GetInt32(15) },

                     }
                };

                return dashboardCount;
            }
            else
            {
                // Handle the case when no rows are returned
                return null;
            }
        }

        //private async Task<DashBoardRealTimeCount> GetDashboardCountForDistrictAdmin(ClaimsIdentity claimsIdentity)
        //{
        //    var stateMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "StateMasterId")?.Value;
        //    var districtMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "DistrictMasterId")?.Value;

        //    // Establish a connection to the PostgreSQL database
        //    await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("Postgres"));
        //    await connection.OpenAsync();

        //    // Create a command to call the PostgreSQL function
        //    var command = new NpgsqlCommand("SELECT * FROM get_election_dashboard_counts_districtwise(@state_master_id,@district_master_id)", connection);
        //    command.Parameters.AddWithValue("@state_master_id", Convert.ToInt32(stateMasterId));
        //    command.Parameters.AddWithValue("@district_master_id", Convert.ToInt32(districtMasterId));

        //    // Execute the command and retrieve the result set
        //    await using var reader = await command.ExecuteReaderAsync();
        //    // Count specific events
        //    var partyDispatchCount = await _context.ElectionInfoMaster
        //        .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.IsPartyDispatched == true)
        //        .CountAsync();

        //    var partyArrivedCount = await _context.ElectionInfoMaster
        //       .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.IsPartyReached == true)
        //        .CountAsync();

        //    var setupPollingStationCount = await _context.ElectionInfoMaster
        //      .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.IsSetupOfPolling == true)
        //        .CountAsync();

        //    var mockPollDoneCount = await _context.ElectionInfoMaster
        //      .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.IsMockPollDone == true)
        //        .CountAsync();

        //    var pollStartedCount = await _context.ElectionInfoMaster
        //        .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.IsPollStarted == true)
        //        .CountAsync();

        //    var pollEndedCount = await _context.ElectionInfoMaster
        // .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.IsPollEnded == true)
        //        .CountAsync();

        //    var evmVVPAOffCount = await _context.ElectionInfoMaster
        //  .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.IsMCESwitchOff == true)
        //        .CountAsync();

        //    var partyDepartedCount = await _context.ElectionInfoMaster
        //.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.IsPartyDeparted == true)
        //        .CountAsync();

        //    var partyReachedAtCollectionCount = await _context.ElectionInfoMaster
        //     .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.IsPartyReachedCollectionCenter == true)
        //        .CountAsync();

        //    var evmDepositedCount = await _context.ElectionInfoMaster
        //        .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.IsEVMDeposited == true)
        //        .CountAsync();

        //    // Calculate total booth count
        //    var totalBoothCount = await _context.BoothMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.BoothStatus == true)
        //        .CountAsync();

        //    // Calculate total votes polled
        //    var totalVotesPolledCount = await _context.PollDetails
        //       .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId))
        //        .GroupBy(d => d.BoothMasterId)
        //        .Select(group => group.OrderByDescending(d => d.VotesPolledRecivedTime).FirstOrDefault().VotesPolled)
        //        .SumAsync();

        //    // Calculate total voters count
        //    var totalVotersCount = await _context.BoothMaster
        //     .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.BoothStatus == true)
        //        .SumAsync(d => d.TotalVoters);

        //    // Calculate total voters in queue
        //    var totalVotersInQueue = await _context.ElectionInfoMaster
        //       .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId))
        //        .SumAsync(d => d.VoterInQueue);

        //    // Calculate total final votes
        //    var totalFinalVotesCount = await _context.ElectionInfoMaster
        //         .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.FinalTVoteStatus == true)
        //        .SumAsync(d => d.FinalTVote);


        //    // Calculate total EDC votes
        //    var eDCVoteCount = await _context.ElectionInfoMaster
        //       .Where(d => d.FinalTVoteStatus == true && d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId))
        //       .SumAsync(d => d.EDC);

        //    // Create and populate EventCount objects
        //    var partyDispatchEvent = new EventCount { EventName = "PartyDispatch", Count = partyDispatchCount };
        //    var partyArrivedEvent = new EventCount { EventName = "PartyArrived", Count = partyArrivedCount };
        //    var setupPollingStationEvent = new EventCount { EventName = "SetupPollingStation", Count = setupPollingStationCount };
        //    var mockPollDoneEvent = new EventCount { EventName = "MockPollDone", Count = mockPollDoneCount };
        //    var pollStartedEvent = new EventCount { EventName = "PollStarted", Count = pollStartedCount };
        //    var votesPolledEvent = new EventCount
        //    {
        //        EventName = "VotesPolled",
        //        VotesPolledCount = totalVotesPolledCount,
        //        VotesPolledPercentage = Math.Round((decimal)(totalVotesPolledCount * 100.0 / totalVotersCount), 1),
        //        TotalVotersCount = totalVotersCount
        //    };
        //    var voterInQueueEvent = new EventCount
        //    {
        //        EventName = "VoterInQueue",
        //        FinalVotesCount = totalVotersInQueue
        //    };
        //    var finalVotesEvent = new EventCount
        //    {
        //        EventName = "FinalVoteDone"
        //        ,
        //        TotalVotersCount = totalVotersCount,
        //        FinalVotesCount = totalFinalVotesCount + eDCVoteCount,
        //        FinalVotesPercentage = Math.Round((decimal)(totalFinalVotesCount * 100.0 / totalVotersCount), 1),
        //    };

        //    var eDCVotesEvent = new EventCount
        //    {
        //        EventName = "EDCVoters",
        //        //TotalVotersCount = totalVotersCount,
        //        FinalVotesCount = eDCVoteCount,
        //        //FinalVotesPercentage = Math.Round((decimal)(totalFinalVotesCount * 100.0 / totalVotersCount), 1),
        //    };

        //    var pollEndedEvent = new EventCount { EventName = "PollEnded", Count = pollEndedCount };
        //    var evmVVPAOffEvent = new EventCount { EventName = "EVMVVPATOff", Count = evmVVPAOffCount };
        //    var partyDepartedEvent = new EventCount { EventName = "PartyDeparted", Count = partyDepartedCount };
        //    var partyReachedAtCollectionEvent = new EventCount { EventName = "PartyReachedAtCollection", Count = partyReachedAtCollectionCount };
        //    var evmDepositedEvent = new EventCount { EventName = "EVMDeposited", Count = evmDepositedCount };

        //    // Create and populate DashboardCount object
        //    var dashboardCount = new DashBoardRealTimeCount
        //    {
        //        Total = totalBoothCount,
        //        Events = new List<EventCount>
        //{
        //    partyDispatchEvent,
        //    partyArrivedEvent,
        //    setupPollingStationEvent,
        //    mockPollDoneEvent,
        //    pollStartedEvent,
        //     votesPolledEvent,
        //    voterInQueueEvent,
        //    finalVotesEvent,
        //    pollEndedEvent,
        //    evmVVPAOffEvent,
        //    partyDepartedEvent,
        //    partyReachedAtCollectionEvent,
        //    evmDepositedEvent,
        //    eDCVotesEvent

        //}
        //    };

        //    return dashboardCount;
        //}

        private async Task<DashBoardRealTimeCount> GetDashboardCountForDistrictAdmin(ClaimsIdentity claimsIdentity)
        {
            var stateMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "StateMasterId")?.Value;
            var districtMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "DistrictMasterId")?.Value;

            // Establish a connection to the PostgreSQL database
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("Postgres"));
            await connection.OpenAsync();

            var totalVotersCount = await _context.BoothMaster
            .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.AssemblyMaster.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.BoothStatus == true)
               .SumAsync(d => d.TotalVoters);
            // Create a command to call the PostgreSQL function
            var command = new NpgsqlCommand("SELECT * FROM get_election_dashboard_counts_districtwise(@state_master_id,@district_master_id)", connection);
            command.Parameters.AddWithValue("@state_master_id", Convert.ToInt32(stateMasterId));
            command.Parameters.AddWithValue("@district_master_id", Convert.ToInt32(districtMasterId));

            // Execute the command and retrieve the result set
            await using var reader = await command.ExecuteReaderAsync();

            // Read the result set and populate the DashboardCount object
            if (await reader.ReadAsync())
            {
                var dashboardCount = new DashBoardRealTimeCount
                {
                    Total = reader.GetInt32(10), // total_booth_count
                    Events = new List<EventCount>
                    {
                new EventCount { EventName = "PartyDispatch", Count = reader.GetInt32(0) },
                new EventCount { EventName = "PartyArrived", Count = reader.GetInt32(1) },
                new EventCount { EventName = "SetupPollingStation", Count = reader.GetInt32(2) },
                new EventCount { EventName = "MockPollDone", Count = reader.GetInt32(3) },
                new EventCount { EventName = "PollStarted", Count = reader.GetInt32(4) },
                     new EventCount {EventName = "VotesPolled", VotesPolledCount = reader.GetInt32(11) ,VotesPolledPercentage = Math.Round((decimal)(reader.GetInt32(11) * 100.0 / totalVotersCount), 1),
                TotalVotersCount = totalVotersCount
                },
                new EventCount{EventName = "VoterInQueue",FinalVotesCount = reader.GetInt32(13)  },
                new EventCount {EventName = "FinalVoteDone", TotalVotersCount = reader.GetInt32(12), // Total voters count
                    FinalVotesCount = reader.GetInt32(14) + reader.GetInt32(15), // Total final votes count + EDC vote count
                    FinalVotesPercentage = Math.Round((decimal)(reader.GetInt32(14) * 100.0 / reader.GetInt32(12)), 1) // Final votes percentage
                },

                new EventCount { EventName = "PollEnded", Count = reader.GetInt32(5) },
                new EventCount { EventName = "EVMVVPATOff", Count = reader.GetInt32(6) },
                new EventCount { EventName = "PartyDeparted", Count = reader.GetInt32(7) },
                new EventCount { EventName = "PartyReachedAtCollection", Count = reader.GetInt32(8) },
                new EventCount { EventName = "EVMDeposited", Count = reader.GetInt32(9) },
                 new EventCount { EventName = "EDCVoters", FinalVotesCount = reader.GetInt32(15) },

                     }
                };
                return dashboardCount;
            }
            else
            {
                // Handle the case when no rows are returned
                return null;
            }
        }

        //private async Task<DashBoardRealTimeCount> GetDashboardCountForPCAdmin1(ClaimsIdentity claimsIdentity)
        //{
        //    var stateMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "StateMasterId")?.Value;
        //    var pcMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "PCMasterId")?.Value;

        //    // Establish a connection to the PostgreSQL database
        //    await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("Postgres"));
        //    await connection.OpenAsync();

        //    // Create a command to call the PostgreSQL function
        //    var command = new NpgsqlCommand("SELECT * FROM get_election_dashboard_counts_pcwise(@state_master_id,@pc_master_id)", connection);
        //    command.Parameters.AddWithValue("@state_master_id", Convert.ToInt32(stateMasterId));
        //    command.Parameters.AddWithValue("@pc_master_id", Convert.ToInt32(pcMasterId));

        //    // Execute the command and retrieve the result set
        //    await using var reader = await command.ExecuteReaderAsync();
        //    // Count specific events
        //    var partyDispatchCount = await _context.ElectionInfoMaster
        //        .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.PCMasterId == Convert.ToInt32(pcMasterId) && d.IsPartyDispatched == true)
        //        .CountAsync();

        //    var partyArrivedCount = await _context.ElectionInfoMaster
        //       .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.PCMasterId == Convert.ToInt32(pcMasterId) && d.IsPartyReached == true)
        //        .CountAsync();

        //    var setupPollingStationCount = await _context.ElectionInfoMaster
        //      .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.PCMasterId == Convert.ToInt32(pcMasterId) && d.IsSetupOfPolling == true)
        //        .CountAsync();

        //    var mockPollDoneCount = await _context.ElectionInfoMaster
        //      .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.PCMasterId == Convert.ToInt32(pcMasterId) && d.IsMockPollDone == true)
        //        .CountAsync();

        //    var pollStartedCount = await _context.ElectionInfoMaster
        //        .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.PCMasterId == Convert.ToInt32(pcMasterId) && d.IsPollStarted == true)
        //        .CountAsync();

        //    var pollEndedCount = await _context.ElectionInfoMaster
        // .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.PCMasterId == Convert.ToInt32(pcMasterId) && d.IsPollEnded == true)
        //        .CountAsync();

        //    var evmVVPAOffCount = await _context.ElectionInfoMaster
        //  .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.PCMasterId == Convert.ToInt32(pcMasterId) && d.IsMCESwitchOff == true)
        //        .CountAsync();

        //    var partyDepartedCount = await _context.ElectionInfoMaster
        //.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.PCMasterId == Convert.ToInt32(pcMasterId) && d.IsPartyDeparted == true)
        //        .CountAsync();

        //    var partyReachedAtCollectionCount = await _context.ElectionInfoMaster
        //     .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.PCMasterId == Convert.ToInt32(pcMasterId) && d.IsPartyReachedCollectionCenter == true)
        //        .CountAsync();

        //    var evmDepositedCount = await _context.ElectionInfoMaster
        //        .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.PCMasterId == Convert.ToInt32(pcMasterId) && d.IsEVMDeposited == true)
        //        .CountAsync();

        //    // Calculate total booth count
        //    var totalBoothCount = await _context.BoothMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.AssemblyMaster.PCMasterId == Convert.ToInt32(pcMasterId) && d.BoothStatus == true)
        //        .CountAsync();

        //    // Calculate total votes polled
        //    var totalVotesPolledCount = await _context.PollDetails
        //       .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.PCMasterId == Convert.ToInt32(pcMasterId))
        //        .GroupBy(d => d.BoothMasterId)
        //        .Select(group => group.OrderByDescending(d => d.VotesPolledRecivedTime).FirstOrDefault().VotesPolled)
        //        .SumAsync();

        //    // Calculate total voters count
        //    var totalVotersCount = await _context.BoothMaster
        //     .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.AssemblyMaster.PCMasterId == Convert.ToInt32(pcMasterId) && d.BoothStatus == true)
        //        .SumAsync(d => d.TotalVoters);

        //    // Calculate total voters in queue
        //    var totalVotersInQueue = await _context.ElectionInfoMaster
        //       .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.PCMasterId == Convert.ToInt32(pcMasterId))
        //        .SumAsync(d => d.VoterInQueue);

        //    // Calculate total final votes
        //    var totalFinalVotesCount = await _context.ElectionInfoMaster
        //         .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.PCMasterId == Convert.ToInt32(pcMasterId) && d.FinalTVoteStatus == true)
        //        .SumAsync(d => d.FinalTVote);

        //    // Calculate total EDC votes
        //    var eDCVoteCount = await _context.ElectionInfoMaster
        //       .Where(d => d.FinalTVoteStatus == true && d.StateMasterId == Convert.ToInt32(stateMasterId) && d.PCMasterId == Convert.ToInt32(pcMasterId))
        //       .SumAsync(d => d.EDC);


        //    // Create and populate EventCount objects
        //    var partyDispatchEvent = new EventCount { EventName = "PartyDispatch", Count = partyDispatchCount };
        //    var partyArrivedEvent = new EventCount { EventName = "PartyArrived", Count = partyArrivedCount };
        //    var setupPollingStationEvent = new EventCount { EventName = "SetupPollingStation", Count = setupPollingStationCount };
        //    var mockPollDoneEvent = new EventCount { EventName = "MockPollDone", Count = mockPollDoneCount };
        //    var pollStartedEvent = new EventCount { EventName = "PollStarted", Count = pollStartedCount };
        //    var votesPolledEvent = new EventCount
        //    {
        //        EventName = "VotesPolled",
        //        VotesPolledCount = totalVotesPolledCount,
        //        VotesPolledPercentage = Math.Round((decimal)(totalVotesPolledCount * 100.0 / totalVotersCount), 1),
        //        TotalVotersCount = totalVotersCount
        //    };
        //    var voterInQueueEvent = new EventCount
        //    {
        //        EventName = "VoterInQueue",
        //        FinalVotesCount = totalVotersInQueue
        //    };

        //    var eDCVotesEvent = new EventCount
        //    {
        //        EventName = "EDCVoters",
        //        //TotalVotersCount = totalVotersCount,
        //        FinalVotesCount = eDCVoteCount,
        //        //FinalVotesPercentage = Math.Round((decimal)(totalFinalVotesCount * 100.0 / totalVotersCount), 1),
        //    };
        //    var finalVotesEvent = new EventCount
        //    {
        //        EventName = "FinalVoteDone"
        //        ,
        //        TotalVotersCount = totalVotersCount,
        //        FinalVotesCount = totalFinalVotesCount + eDCVoteCount,
        //        FinalVotesPercentage = Math.Round((decimal)(totalFinalVotesCount * 100.0 / totalVotersCount), 1),
        //    };
        //    var pollEndedEvent = new EventCount { EventName = "PollEnded", Count = pollEndedCount };
        //    var evmVVPAOffEvent = new EventCount { EventName = "EVMVVPATOff", Count = evmVVPAOffCount };
        //    var partyDepartedEvent = new EventCount { EventName = "PartyDeparted", Count = partyDepartedCount };
        //    var partyReachedAtCollectionEvent = new EventCount { EventName = "PartyReachedAtCollection", Count = partyReachedAtCollectionCount };
        //    var evmDepositedEvent = new EventCount { EventName = "EVMDeposited", Count = evmDepositedCount };

        //    // Create and populate DashboardCount object
        //    var dashboardCount = new DashBoardRealTimeCount
        //    {
        //        Total = totalBoothCount,
        //        Events = new List<EventCount>
        //{
        //    partyDispatchEvent,
        //    partyArrivedEvent,
        //    setupPollingStationEvent,
        //    mockPollDoneEvent,
        //    pollStartedEvent,
        //     votesPolledEvent,
        //    voterInQueueEvent,
        //    finalVotesEvent,
        //    pollEndedEvent,
        //    evmVVPAOffEvent,
        //    partyDepartedEvent,
        //    partyReachedAtCollectionEvent,
        //    evmDepositedEvent,
        //    eDCVotesEvent

        //}
        //    };

        //    return dashboardCount;
        //}

        private async Task<DashBoardRealTimeCount> GetDashboardCountForPCAdmin(ClaimsIdentity claimsIdentity)
        {
            var stateMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "StateMasterId")?.Value;
            var pcMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "PCMasterId")?.Value;

            // Establish a connection to the PostgreSQL database
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("Postgres"));
            await connection.OpenAsync();

            // Create a command to call the PostgreSQL function
            var command = new NpgsqlCommand("SELECT * FROM get_election_dashboard_counts_pcwise(@state_master_id,@pc_master_id)", connection);
            command.Parameters.AddWithValue("@state_master_id", Convert.ToInt32(stateMasterId));
            command.Parameters.AddWithValue("@pc_master_id", Convert.ToInt32(pcMasterId));

            // Execute the command and retrieve the result set
            await using var reader = await command.ExecuteReaderAsync();
            var totalVotersCount = await _context.BoothMaster
            .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.AssemblyMaster.PCMasterId == Convert.ToInt32(pcMasterId) && d.BoothStatus == true)
               .SumAsync(d => d.TotalVoters);
            // Read the result set and populate the DashboardCount object
            if (await reader.ReadAsync())
            {
                var dashboardCount = new DashBoardRealTimeCount
                {
                    Total = reader.GetInt32(10), // total_booth_count
                    Events = new List<EventCount>
                    {
                new EventCount { EventName = "PartyDispatch", Count = reader.GetInt32(0) },
                new EventCount { EventName = "PartyArrived", Count = reader.GetInt32(1) },
                new EventCount { EventName = "SetupPollingStation", Count = reader.GetInt32(2) },
                new EventCount { EventName = "MockPollDone", Count = reader.GetInt32(3) },
                new EventCount { EventName = "PollStarted", Count = reader.GetInt32(4) },
                     new EventCount {EventName = "VotesPolled", VotesPolledCount = reader.GetInt32(11) ,VotesPolledPercentage = Math.Round((decimal)(reader.GetInt32(11) * 100.0 / totalVotersCount), 1),
                TotalVotersCount = totalVotersCount
                },
                new EventCount{EventName = "VoterInQueue",FinalVotesCount = reader.GetInt32(13)  },
                new EventCount {EventName = "FinalVoteDone", TotalVotersCount = reader.GetInt32(12), // Total voters count
                    FinalVotesCount = reader.GetInt32(14) + reader.GetInt32(15), // Total final votes count + EDC vote count
                    FinalVotesPercentage = Math.Round((decimal)(reader.GetInt32(14) * 100.0 / reader.GetInt32(12)), 1) // Final votes percentage
                },

                new EventCount { EventName = "PollEnded", Count = reader.GetInt32(5) },
                new EventCount { EventName = "EVMVVPATOff", Count = reader.GetInt32(6) },
                new EventCount { EventName = "PartyDeparted", Count = reader.GetInt32(7) },
                new EventCount { EventName = "PartyReachedAtCollection", Count = reader.GetInt32(8) },
                new EventCount { EventName = "EVMDeposited", Count = reader.GetInt32(9) },
                  new EventCount { EventName = "EDCVoters", FinalVotesCount = reader.GetInt32(15) },

                     }
                };

                return dashboardCount;
            }
            else
            {
                // Handle the case when no rows are returned
                return null;
            }
        }

        private async Task<DashBoardRealTimeCount> GetDashboardCountForARO(ClaimsIdentity claimsIdentity)
        {
            var stateMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "StateMasterId")?.Value;
            var districtMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "DistrictMasterId")?.Value;
            var assemblyMasterId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "AssemblyMasterId")?.Value;
            // Count specific events
            var partyDispatchCount = await _context.ElectionInfoMaster
                .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId) && d.IsPartyDispatched == true)
                .CountAsync();

            var partyArrivedCount = await _context.ElectionInfoMaster
               .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId) && d.IsPartyReached == true)
                .CountAsync();

            var setupPollingStationCount = await _context.ElectionInfoMaster
              .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId) && d.IsSetupOfPolling == true)
                .CountAsync();

            var mockPollDoneCount = await _context.ElectionInfoMaster
              .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId) && d.IsMockPollDone == true)
                .CountAsync();

            var pollStartedCount = await _context.ElectionInfoMaster
                .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId) && d.IsPollStarted == true)
                .CountAsync();

            var pollEndedCount = await _context.ElectionInfoMaster
         .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId) && d.IsPollEnded == true)
                .CountAsync();

            var evmVVPAOffCount = await _context.ElectionInfoMaster
          .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId) && d.IsMCESwitchOff == true)
                .CountAsync();

            var partyDepartedCount = await _context.ElectionInfoMaster
        .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId) && d.IsPartyDeparted == true)
                .CountAsync();

            var partyReachedAtCollectionCount = await _context.ElectionInfoMaster
             .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId) && d.IsPartyReachedCollectionCenter == true)
                .CountAsync();

            var evmDepositedCount = await _context.ElectionInfoMaster
                .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId) && d.IsEVMDeposited == true)
                .CountAsync();

            // Calculate total booth count
            var totalBoothCount = await _context.BoothMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId) && d.BoothStatus == true)
                .CountAsync();

            // Calculate total votes polled
            var totalVotesPolledCount = await _context.PollDetails
               .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId))
                .GroupBy(d => d.BoothMasterId)
                .Select(group => group.OrderByDescending(d => d.VotesPolledRecivedTime).FirstOrDefault().VotesPolled)
                .SumAsync();

            // Calculate total voters count
            var totalVotersCount = await _context.BoothMaster
             .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId) && d.BoothStatus == true)
                .SumAsync(d => d.TotalVoters);

            // Calculate total voters in queue
            var totalVotersInQueue = await _context.ElectionInfoMaster
               .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId))
                .SumAsync(d => d.VoterInQueue);

            // Calculate total final votes
            var totalFinalVotesCount = await _context.ElectionInfoMaster
                 .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId) && d.FinalTVoteStatus == true)
                .SumAsync(d => d.FinalTVote);

            // Calculate total EDC votes
            var eDCVoteCount = await _context.ElectionInfoMaster
               .Where(d => d.FinalTVoteStatus == true && d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId))
               .SumAsync(d => d.EDC);


            // Create and populate EventCount objects
            var partyDispatchEvent = new EventCount { EventName = "PartyDispatch", Count = partyDispatchCount };
            var partyArrivedEvent = new EventCount { EventName = "PartyArrived", Count = partyArrivedCount };
            var setupPollingStationEvent = new EventCount { EventName = "SetupPollingStation", Count = setupPollingStationCount };
            var mockPollDoneEvent = new EventCount { EventName = "MockPollDone", Count = mockPollDoneCount };
            var pollStartedEvent = new EventCount { EventName = "PollStarted", Count = pollStartedCount };
            var votesPolledEvent = new EventCount
            {
                EventName = "VotesPolled",
                VotesPolledCount = totalVotesPolledCount,
                VotesPolledPercentage = Math.Round((decimal)(totalVotesPolledCount * 100.0 / totalVotersCount), 1),
                TotalVotersCount = totalVotersCount
            };
            var voterInQueueEvent = new EventCount
            {
                EventName = "VoterInQueue",
                FinalVotesCount = totalVotersInQueue
            };

            var eDCVotesEvent = new EventCount
            {
                EventName = "EDCVoters",
                //TotalVotersCount = totalVotersCount,
                FinalVotesCount = eDCVoteCount,
                //FinalVotesPercentage = Math.Round((decimal)(totalFinalVotesCount * 100.0 / totalVotersCount), 1),
            };
            var finalVotesEvent = new EventCount
            {
                EventName = "FinalVoteDone",
                TotalVotersCount = totalVotersCount,
                FinalVotesCount = totalFinalVotesCount + eDCVoteCount,
                //FinalVotesPercentage = Math.Round((decimal)(totalFinalVotesCount * 100.0 / totalVotersCount), 1),
                FinalVotesPercentage = Math.Round((decimal)((totalFinalVotesCount + eDCVoteCount) * 100.0 / totalVotersCount), 1),//Now EDC added 
            };

            var pollEndedEvent = new EventCount { EventName = "PollEnded", Count = pollEndedCount };
            var evmVVPAOffEvent = new EventCount { EventName = "EVMVVPATOff", Count = evmVVPAOffCount };
            var partyDepartedEvent = new EventCount { EventName = "PartyDeparted", Count = partyDepartedCount };
            var partyReachedAtCollectionEvent = new EventCount { EventName = "PartyReachedAtCollection", Count = partyReachedAtCollectionCount };
            var evmDepositedEvent = new EventCount { EventName = "EVMDeposited", Count = evmDepositedCount };

            // Create and populate DashboardCount object
            var dashboardCount = new DashBoardRealTimeCount
            {
                Total = totalBoothCount,
                Events = new List<EventCount>
        {
            partyDispatchEvent,
            partyArrivedEvent,
            setupPollingStationEvent,
            mockPollDoneEvent,
            pollStartedEvent,
             votesPolledEvent,
            voterInQueueEvent,
            finalVotesEvent,
            pollEndedEvent,
            evmVVPAOffEvent,
            partyDepartedEvent,
            partyReachedAtCollectionEvent,
            evmDepositedEvent,
            eDCVotesEvent

        }
            };

            return dashboardCount;
        }

        #endregion


        #endregion

        #region VoterTurn Out Consolidated Report
        public async Task<List<VTReportModel>> GetVoterTurnOutConsolidatedReports(BoothReportModel boothReportModel)
        {
            List<VTReportModel> assemblylistTotal = new List<VTReportModel>();
            List<VTReportModel> consolidateBoothReports = new List<VTReportModel>();
            var state = _context.StateMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.StateStatus == true).Select(d => new { d.StateName, d.StateCode }).FirstOrDefault();
            var district = new { DistrictName = "", DistrictCode = "" };
            var pcMaster = new { PcName = "", PcCodeNo = "" };

            if (boothReportModel.DistrictMasterId is not 0)
            {
                district = _context.DistrictMaster.Where(d => d.DistrictMasterId == boothReportModel.DistrictMasterId && d.StateMasterId == boothReportModel.StateMasterId && d.DistrictStatus == true).Select(d => new { d.DistrictName, d.DistrictCode }).FirstOrDefault();
            }

            if (boothReportModel.PCMasterId is not 0)
            {
                pcMaster = _context.ParliamentConstituencyMaster.Where(d => d.PCMasterId == boothReportModel.PCMasterId && d.PcStatus == true).Select(d => new { d.PcName, d.PcCodeNo }).FirstOrDefault();
            }

            //Type : DistrictACWise
            if (boothReportModel.StateMasterId is not 0 && boothReportModel.Type == "DistrictACWise")
            {
                //all assembies in a district
                if (boothReportModel.DistrictMasterId is not 0 && boothReportModel.AssemblyMasterId is 0)
                {
                    var dis = _context.DistrictMaster
        .Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictStatus && d.DistrictMasterId == boothReportModel.DistrictMasterId)
        .AsEnumerable().FirstOrDefault();

                    int assemblyCount = 0; List<VTReportModel> assemblylistReport = new List<VTReportModel>();
                    var assemblyList = _context.AssemblyMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == dis.DistrictMasterId).OrderBy(d => d.AssemblyCode).ToList();
                    if (assemblyList.Count > 0)
                    {
                        foreach (var assembly in assemblyList)
                        {
                            VTReportModel report = new VTReportModel();
                            report.Header = $"{state.StateName}({state.StateCode})";
                            report.Title = $"{state.StateName},({dis.DistrictName})";
                            report.Type = "ACWisebyDistrict";
                            report.DistrictName = dis.DistrictName;
                            report.DistrictCode = dis.DistrictCode;
                            report.AssemblyName = assemblyList.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId)
                                                               .Select(p => p.AssemblyName).FirstOrDefault();
                            report.AssemblyCode = assemblyList.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId)
                                                               .Select(p => p.AssemblyCode.ToString()).FirstOrDefault();
                            var pollingStationData = _context.PollingStationMaster.Include(psm => psm.PollingStationGender).Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId).ToList();
                            if (pollingStationData != null && pollingStationData.Count > 0)
                            {
                                //type 1: Electorals
                                report.MaleElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                          .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
                                                                          .Sum(gender => gender.Male));
                                report.FemaleElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                            .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
                                                                            .Sum(gender => gender.Female));
                                report.ThirdGenderElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                                .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
                                                                                .Sum(gender => gender.ThirdGender));
                                report.TotalElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                           .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
                                                                           .Sum(gender => gender.Total));

                                //type 2: Votes polled
                                report.MaleVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                       .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
                                                                       .Sum(gender => gender.Male));
                                report.FemaleVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                         .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
                                                                         .Sum(gender => gender.Female));
                                report.ThirdGenderVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                             .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
                                                                             .Sum(gender => gender.ThirdGender));
                                report.TotalVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                        .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
                                                                        .Sum(gender => gender.Total));

                                report.EPIC = pollingStationData.Sum(psm => psm.VotePolledEPIC);
                                report.VotePolledOtherDocument = pollingStationData.Sum(psm =>
                                {
                                    if (int.TryParse(psm.VotePolledOtherDocument, out int result))
                                    {
                                        return result;
                                    }
                                    else
                                    {
                                        return 0;
                                    }
                                });
                                report.MalePercentage = report.MaleVoters == 0 ? 0 : (double)report.MaleVoters / (double)report.MaleElectoral * 100;
                                if (double.IsInfinity(report.MalePercentage))
                                {
                                    report.MalePercentage = 0;
                                }

                                report.FemalePercentage = report.FemaleVoters == 0 ? 0 : (double)report.FemaleVoters / (double)report.FemaleElectoral * 100;
                                if (double.IsInfinity(report.FemalePercentage))
                                {
                                    report.FemalePercentage = 0;
                                }
                                report.ThirdGenderPercentage = report.ThirdGenderVoters == 0 ? 0 : (double)report.ThirdGenderVoters / (double)report.ThirdGenderElectoral * 100;
                                if (double.IsInfinity(report.ThirdGenderPercentage))
                                {
                                    report.ThirdGenderPercentage = 0;
                                }
                                report.TotalPercentage = report.TotalVoters == 0 ? 0 : (double)report.TotalVoters / (double)report.TotalElectoral * 100;
                                if (double.IsInfinity(report.TotalPercentage))
                                {
                                    report.TotalPercentage = 0;
                                }

                                consolidateBoothReports.Add(report); assemblylistReport.Add(report);
                                assemblyCount++;


                            }
                            else
                            {  //type 1: Electorals
                                report.MaleElectoral = 0;
                                report.FemaleElectoral = 0;
                                report.ThirdGenderElectoral = 0;
                                report.TotalElectoral = 0;

                                //type 2: Votes polled
                                report.MaleVoters = 0;
                                report.FemaleVoters = 0;
                                report.ThirdGenderVoters = 0;
                                report.TotalVoters = 0;
                                report.EPIC = 0;
                                report.VotePolledOtherDocument = 0;
                                report.MalePercentage = 0;
                                report.FemalePercentage = 0;
                                report.ThirdGenderPercentage = 0;
                                report.TotalPercentage = 0;
                                consolidateBoothReports.Add(report); assemblylistReport.Add(report);
                                assemblyCount++;


                            }


                        }

                    }
                    else
                    {
                        VTReportModel report = new VTReportModel();
                        report.Header = $"{state.StateName}({state.StateCode})";
                        report.Title = $"{state.StateName},({dis.DistrictName})";
                        report.Type = "ACWisebyDistrict";
                        report.DistrictName = dis.DistrictName;
                        report.DistrictCode = dis.DistrictCode;
                        report.AssemblyName = "N/A";
                        report.AssemblyCode = "N/A";
                        report.MaleElectoral = 0;
                        report.FemaleElectoral = 0;
                        report.ThirdGenderElectoral = 0;
                        report.TotalElectoral = 0;

                        //type 2: Votes polled
                        report.MaleVoters = 0;
                        report.FemaleVoters = 0;
                        report.ThirdGenderVoters = 0;
                        report.TotalVoters = 0;
                        report.EPIC = 0;
                        report.VotePolledOtherDocument = 0;
                        report.MalePercentage = 0;
                        report.FemalePercentage = 0;
                        report.ThirdGenderPercentage = 0;
                        report.TotalPercentage = 0;
                        consolidateBoothReports.Add(report);

                    }
                    if (assemblyList.Count == assemblyCount)
                    {
                        // add Grand Total Row

                        VTReportModel reportTotal = new VTReportModel();
                        reportTotal.Header = "";
                        reportTotal.Title = "";
                        reportTotal.Type = "";
                        reportTotal.DistrictName = "";
                        reportTotal.DistrictCode = "";
                        reportTotal.AssemblyName = "Total";
                        reportTotal.AssemblyCode = "";
                        reportTotal.MaleElectoral = assemblylistReport.Sum(report => report.MaleElectoral);
                        reportTotal.FemaleElectoral = assemblylistReport.Sum(report => report.FemaleElectoral);
                        reportTotal.ThirdGenderElectoral = assemblylistReport.Sum(report => report.ThirdGenderElectoral);
                        reportTotal.TotalElectoral = assemblylistReport.Sum(report => report.TotalElectoral);
                        reportTotal.MaleVoters = assemblylistReport.Sum(report => report.MaleVoters);
                        reportTotal.FemaleVoters = assemblylistReport.Sum(report => report.FemaleVoters);
                        reportTotal.ThirdGenderVoters = assemblylistReport.Sum(report => report.ThirdGenderVoters);
                        reportTotal.TotalVoters = assemblylistReport.Sum(report => report.TotalVoters);
                        reportTotal.EPIC = assemblylistReport.Sum(report => report.EPIC);
                        reportTotal.VotePolledOtherDocument = assemblylistReport.Sum(report => report.VotePolledOtherDocument);

                        //reportTotal.MalePercentage = assemblylistReport.Average(report => report.MalePercentage);
                        //reportTotal.FemalePercentage = assemblylistReport.Average(report => report.FemalePercentage);
                        //reportTotal.ThirdGenderPercentage = assemblylistReport.Average(report => report.ThirdGenderPercentage);
                        //reportTotal.TotalPercentage = assemblylistReport.Average(report => report.TotalPercentage);

                        reportTotal.MalePercentage = (reportTotal.MaleVoters != 0 && reportTotal.MaleElectoral != 0)
   ? ((double)reportTotal.MaleVoters / (double)reportTotal.MaleElectoral) * 100
   : 0;

                        //reportGrandTotal.FemalePercentage = (double)reportGrandTotal.FemaleVoters / (double)reportGrandTotal.FemaleElectoral * 100;
                        reportTotal.FemalePercentage = (reportTotal.FemaleVoters != 0 && reportTotal.FemaleElectoral != 0)
         ? ((double)reportTotal.FemaleVoters / (double)reportTotal.FemaleElectoral) * 100 : 0;

                        reportTotal.ThirdGenderPercentage = (reportTotal.ThirdGenderVoters != 0 && reportTotal.ThirdGenderElectoral != 0)
        ? ((double)reportTotal.ThirdGenderVoters / (double)reportTotal.ThirdGenderElectoral) * 100
        : 0;

                        //reportGrandTotal.TotalPercentage = (double)reportGrandTotal.TotalVoters / (double)reportGrandTotal.TotalElectoral * 100;


                        reportTotal.TotalPercentage = (reportTotal.TotalVoters != 0 && reportTotal.TotalElectoral != 0)
       ? ((double)reportTotal.TotalVoters / (double)reportTotal.TotalElectoral) * 100
       : 0;

                        assemblylistTotal.Add(reportTotal);
                        consolidateBoothReports.Add(reportTotal);
                        // calculate Percentage of Electiral & Voters 

                    }


                    return consolidateBoothReports;
                }

                //particular assembly
                if (boothReportModel.DistrictMasterId is not 0 && boothReportModel.AssemblyMasterId is not 0)
                {
                    var assemblyRec = _context.AssemblyMaster.Where(d => d.AssemblyMasterId == boothReportModel.AssemblyMasterId && d.StateMasterId == boothReportModel.StateMasterId && d.AssemblyStatus == true).Select(d => new { d.AssemblyName, d.AssemblyCode }).FirstOrDefault();


                    var boothList = _context.BoothMaster
                   .Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId && d.BoothStatus && d.AssemblyMasterId == boothReportModel.AssemblyMasterId)
                   .ToList();

                    if (boothList.Count > 0)
                    {
                        foreach (var booth in boothList)
                        {
                            VTReportModel report = new VTReportModel();
                            report.Header = $"{state.StateName}({state.StateCode})";
                            report.Title = $"{state.StateName},({assemblyRec.AssemblyName})";
                            report.Type = "ACBoothWisebyDistrict";
                            report.DistrictName = district.DistrictName;
                            report.DistrictCode = district.DistrictCode;
                            report.AssemblyName = assemblyRec.AssemblyName;
                            report.AssemblyCode = assemblyRec.AssemblyCode.ToString();
                            var BoothRecord = _context.BoothMaster.Where(d => d.BoothMasterId == booth.BoothMasterId).FirstOrDefault();
                            report.BoothName = BoothRecord.BoothName + " " + BoothRecord.BoothCode_No.ToString();

                            var pollingStationData = _context.PollingStationMaster.Include(psm => psm.PollingStationGender).Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId && d.BoothMasterId == booth.BoothMasterId).ToList();
                            if (pollingStationData != null && pollingStationData.Count > 0)
                            {
                                //type 1: Electorals

                                report.MaleElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                          .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
                                                                          .Sum(gender => gender.Male));
                                report.FemaleElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                            .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
                                                                            .Sum(gender => gender.Female));
                                report.ThirdGenderElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                                .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
                                                                                .Sum(gender => gender.ThirdGender));
                                report.TotalElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                           .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
                                                                           .Sum(gender => gender.Total));

                                //type 2: Votes polled
                                report.MaleVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                       .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
                                                                       .Sum(gender => gender.Male));
                                report.FemaleVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                         .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
                                                                         .Sum(gender => gender.Female));
                                report.ThirdGenderVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                             .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
                                                                             .Sum(gender => gender.ThirdGender));
                                report.TotalVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                        .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
                                                                        .Sum(gender => gender.Total));

                                report.EPIC = pollingStationData.Sum(psm => psm.VotePolledEPIC);
                                report.VotePolledOtherDocument = pollingStationData.Sum(psm =>
                                {
                                    if (int.TryParse(psm.VotePolledOtherDocument, out int result))
                                    {
                                        return result;
                                    }
                                    else
                                    {
                                        return 0;
                                    }
                                });
                                report.MalePercentage = report.MaleVoters == 0 ? 0 : (double)report.MaleVoters / (double)report.MaleElectoral * 100;
                                if (double.IsInfinity(report.MalePercentage))
                                {
                                    report.MalePercentage = 0;
                                }

                                report.FemalePercentage = report.FemaleVoters == 0 ? 0 : (double)report.FemaleVoters / (double)report.FemaleElectoral * 100;
                                if (double.IsInfinity(report.FemalePercentage))
                                {
                                    report.FemalePercentage = 0;
                                }
                                report.ThirdGenderPercentage = report.ThirdGenderVoters == 0 ? 0 : (double)report.ThirdGenderVoters / (double)report.ThirdGenderElectoral * 100;
                                if (double.IsInfinity(report.ThirdGenderPercentage))
                                {
                                    report.ThirdGenderPercentage = 0;
                                }
                                report.TotalPercentage = report.TotalVoters == 0 ? 0 : (double)report.TotalVoters / (double)report.TotalElectoral * 100;
                                if (double.IsInfinity(report.TotalPercentage))
                                {
                                    report.TotalPercentage = 0;
                                }
                                consolidateBoothReports.Add(report);



                            }
                            else
                            {  //type 1: Electorals
                                report.MaleElectoral = 0;
                                report.FemaleElectoral = 0;
                                report.ThirdGenderElectoral = 0;
                                report.TotalElectoral = 0;

                                //type 2: Votes polled
                                report.MaleVoters = 0;
                                report.FemaleVoters = 0;
                                report.ThirdGenderVoters = 0;
                                report.TotalVoters = 0;
                                report.EPIC = 0;
                                report.VotePolledOtherDocument = 0;
                                report.MalePercentage = 0;
                                report.FemalePercentage = 0;
                                report.ThirdGenderPercentage = 0;
                                report.TotalPercentage = 0;
                                consolidateBoothReports.Add(report);



                            }

                        }

                    }
                    else

                    {
                        VTReportModel report = new VTReportModel();
                        report.Header = $"{state.StateName}({state.StateCode})";
                        report.Title = $"{state.StateName},({assemblyRec.AssemblyName})";
                        report.Type = "ACBoothWisebyDistrict";
                        report.DistrictName = district.DistrictName;
                        report.DistrictCode = district.DistrictCode;
                        report.AssemblyName = "N/A";
                        report.AssemblyCode = "N/A";
                        report.MaleElectoral = 0;
                        report.FemaleElectoral = 0;
                        report.ThirdGenderElectoral = 0;
                        report.TotalElectoral = 0;

                        //type 2: Votes polled
                        report.MaleVoters = 0;
                        report.FemaleVoters = 0;
                        report.ThirdGenderVoters = 0;
                        report.TotalVoters = 0;
                        report.EPIC = 0;
                        report.VotePolledOtherDocument = 0;
                        report.MalePercentage = 0;
                        report.FemalePercentage = 0;
                        report.ThirdGenderPercentage = 0;
                        report.TotalPercentage = 0;
                        consolidateBoothReports.Add(report);
                    }


                    /* var assemblyList = _context.AssemblyMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId && d.AssemblyMasterId == boothReportModel.AssemblyMasterId).OrderBy(d => d.AssemblyCode).FirstOrDefault();
                 if (assemblyList != null)
                 {

                     VTReportModel report = new VTReportModel();
                     report.Header = $"{state.StateName}({state.StateCode})";
                     report.Title = $"{state.StateName}";
                     report.Type = "ACbyDistrict";
                     report.DistrictName = district.DistrictName;
                     report.DistrictCode = district.DistrictCode;
                     report.AssemblyName = assemblyList.AssemblyName;
                     report.AssemblyCode = assemblyList.AssemblyCode.ToString();


                     var pollingStationData = _context.PollingStationMaster.Include(psm => psm.PollingStationGender).Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId && d.AssemblyMasterId == assemblyList.AssemblyMasterId).ToList();
                     if (pollingStationData != null && pollingStationData.Count > 0)
                     {
                         //type 1: Electorals

                         report.MaleElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                   .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
                                                                   .Sum(gender => gender.Male));
                         report.FemaleElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                     .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
                                                                     .Sum(gender => gender.Female));
                         report.ThirdGenderElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                         .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
                                                                         .Sum(gender => gender.ThirdGender));
                         report.TotalElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                    .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
                                                                    .Sum(gender => gender.Total));

                         //type 2: Votes polled
                         report.MaleVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
                                                                .Sum(gender => gender.Male));
                         report.FemaleVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                  .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
                                                                  .Sum(gender => gender.Female));
                         report.ThirdGenderVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                      .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
                                                                      .Sum(gender => gender.ThirdGender));
                         report.TotalVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                 .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
                                                                 .Sum(gender => gender.Total));

                         report.EPIC = pollingStationData.Sum(psm => psm.VotePolledEPIC);
                         report.VotePolledOtherDocument = pollingStationData.Sum(psm =>
                         {
                             if (int.TryParse(psm.VotePolledOtherDocument, out int result))
                             {
                                 return result;
                             }
                             else
                             {
                                 return 0;
                             }
                         });
                         report.MalePercentage = report.MaleVoters == 0 ? 0 : (double)report.MaleVoters / (double)report.MaleElectoral * 100;
                         if (double.IsInfinity(report.MalePercentage))
                         {
                             report.MalePercentage = 0;
                         }

                         report.FemalePercentage = report.FemaleVoters == 0 ? 0 : (double)report.FemaleVoters / (double)report.FemaleElectoral * 100;
                         if (double.IsInfinity(report.FemalePercentage))
                         {
                             report.FemalePercentage = 0;
                         }
                         report.ThirdGenderPercentage = report.ThirdGenderVoters == 0 ? 0 : (double)report.ThirdGenderVoters / (double)report.ThirdGenderElectoral * 100;
                         if (double.IsInfinity(report.ThirdGenderPercentage))
                         {
                             report.ThirdGenderPercentage = 0;
                         }
                         report.TotalPercentage = report.TotalVoters == 0 ? 0 : (double)report.TotalVoters / (double)report.TotalElectoral * 100;
                         if (double.IsInfinity(report.TotalPercentage))
                         {
                             report.TotalPercentage = 0;
                         }
                         consolidateBoothReports.Add(report);



                     }
                     else
                     {  //type 1: Electorals
                         report.MaleElectoral = 0;
                         report.FemaleElectoral = 0;
                         report.ThirdGenderElectoral = 0;
                         report.TotalElectoral = 0;

                         //type 2: Votes polled
                         report.MaleVoters = 0;
                         report.FemaleVoters = 0;
                         report.ThirdGenderVoters = 0;
                         report.TotalVoters = 0;
                         report.EPIC = 0;
                         report.VotePolledOtherDocument = 0;
                         report.MalePercentage = 0;
                         report.FemalePercentage = 0;
                         report.ThirdGenderPercentage = 0;
                         report.TotalPercentage = 0;
                         consolidateBoothReports.Add(report);



                     }


                 }
                 else
                 {
                     VTReportModel report = new VTReportModel();
                     report.Header = $"{state.StateName}({state.StateCode})";
                     report.Title = $"{state.StateName}";
                     report.Type = "ACbyDistrict";
                     report.DistrictName = dis.DistrictName;
                     report.DistrictCode = dis.DistrictCode;
                     report.AssemblyName = "N/A";
                     report.AssemblyCode = "N/A";
                     report.MaleElectoral = 0;
                     report.FemaleElectoral = 0;
                     report.ThirdGenderElectoral = 0;
                     report.TotalElectoral = 0;

                     //type 2: Votes polled
                     report.MaleVoters = 0;
                     report.FemaleVoters = 0;
                     report.ThirdGenderVoters = 0;
                     report.TotalVoters = 0;
                     report.EPIC = 0;
                     report.VotePolledOtherDocument = 0;
                     report.MalePercentage = 0;
                     report.FemalePercentage = 0;
                     report.ThirdGenderPercentage = 0;
                     report.TotalPercentage = 0;
                     consolidateBoothReports.Add(report);

                 }*/

                    // add Grand Total Row

                    VTReportModel reportTotal = new VTReportModel();
                    reportTotal.Header = "";
                    reportTotal.Title = "";
                    reportTotal.Type = "";
                    reportTotal.DistrictName = "";
                    reportTotal.DistrictCode = "";
                    reportTotal.AssemblyName = "Total";
                    reportTotal.AssemblyCode = "";
                    reportTotal.MaleElectoral = consolidateBoothReports.Sum(report => report.MaleElectoral);
                    reportTotal.FemaleElectoral = consolidateBoothReports.Sum(report => report.FemaleElectoral);
                    reportTotal.ThirdGenderElectoral = consolidateBoothReports.Sum(report => report.ThirdGenderElectoral);
                    reportTotal.TotalElectoral = consolidateBoothReports.Sum(report => report.TotalElectoral);
                    reportTotal.MaleVoters = consolidateBoothReports.Sum(report => report.MaleVoters);
                    reportTotal.FemaleVoters = consolidateBoothReports.Sum(report => report.FemaleVoters);
                    reportTotal.ThirdGenderVoters = consolidateBoothReports.Sum(report => report.ThirdGenderVoters);
                    reportTotal.TotalVoters = consolidateBoothReports.Sum(report => report.TotalVoters);
                    reportTotal.EPIC = consolidateBoothReports.Sum(report => report.EPIC);
                    reportTotal.VotePolledOtherDocument = consolidateBoothReports.Sum(report => report.VotePolledOtherDocument);


                    reportTotal.MalePercentage = (reportTotal.MaleVoters != 0 && reportTotal.MaleElectoral != 0)
  ? ((double)reportTotal.MaleVoters / (double)reportTotal.MaleElectoral) * 100
  : 0;

                    //reportGrandTotal.FemalePercentage = (double)reportGrandTotal.FemaleVoters / (double)reportGrandTotal.FemaleElectoral * 100;
                    reportTotal.FemalePercentage = (reportTotal.FemaleVoters != 0 && reportTotal.FemaleElectoral != 0)
     ? ((double)reportTotal.FemaleVoters / (double)reportTotal.FemaleElectoral) * 100 : 0;

                    reportTotal.ThirdGenderPercentage = (reportTotal.ThirdGenderVoters != 0 && reportTotal.ThirdGenderElectoral != 0)
    ? ((double)reportTotal.ThirdGenderVoters / (double)reportTotal.ThirdGenderElectoral) * 100
    : 0;

                    //reportGrandTotal.TotalPercentage = (double)reportGrandTotal.TotalVoters / (double)reportGrandTotal.TotalElectoral * 100;


                    reportTotal.TotalPercentage = (reportTotal.TotalVoters != 0 && reportTotal.TotalElectoral != 0)
   ? ((double)reportTotal.TotalVoters / (double)reportTotal.TotalElectoral) * 100
   : 0;



                    consolidateBoothReports.Add(reportTotal);




                    return consolidateBoothReports;
                }

                else
                {
                    var districtList = _context.DistrictMaster
        .Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictStatus)
        .AsEnumerable() // Switch to client-side evaluation
        .OrderBy(p => int.Parse(p.DistrictCode))
        .ToList();


                    foreach (var dis in districtList)
                    {
                        int assemblyCount = 0; List<VTReportModel> assemblylistReport = new List<VTReportModel>();
                        //if (dis.DistrictMasterId == 5)
                        //{

                        var assemblyList = _context.AssemblyMaster
                            .Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == dis.DistrictMasterId)
                            .OrderBy(d => d.AssemblyCode)
                            .ToList();

                        if (assemblyList.Count > 0)
                        {
                            foreach (var assembly in assemblyList)
                            {
                                VTReportModel report = new VTReportModel();
                                report.Header = $"{state.StateName}({state.StateCode})";
                                report.Title = $"{state.StateName}";
                                report.Type = "DistrictACWise";
                                report.DistrictName = dis.DistrictName;
                                report.DistrictCode = dis.DistrictCode;
                                report.AssemblyName = assemblyList.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId)
                                                                   .Select(p => p.AssemblyName).FirstOrDefault();
                                report.AssemblyCode = assemblyList.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId)
                                                                   .Select(p => p.AssemblyCode.ToString()).FirstOrDefault();
                                var pollingStationData = _context.PollingStationMaster.Include(psm => psm.PollingStationGender).Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId).ToList();
                                if (pollingStationData != null && pollingStationData.Count > 0)
                                {
                                    //type 1: Electorals
                                    report.MaleElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                              .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
                                                                              .Sum(gender => gender.Male));
                                    report.FemaleElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                                .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
                                                                                .Sum(gender => gender.Female));
                                    report.ThirdGenderElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                                    .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
                                                                                    .Sum(gender => gender.ThirdGender));
                                    report.TotalElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                               .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
                                                                               .Sum(gender => gender.Total));

                                    //type 2: Votes polled
                                    report.MaleVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                           .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
                                                                           .Sum(gender => gender.Male));
                                    report.FemaleVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                             .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
                                                                             .Sum(gender => gender.Female));
                                    report.ThirdGenderVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                                 .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
                                                                                 .Sum(gender => gender.ThirdGender));
                                    report.TotalVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                            .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
                                                                            .Sum(gender => gender.Total));

                                    report.EPIC = pollingStationData.Sum(psm => psm.VotePolledEPIC);
                                    report.VotePolledOtherDocument = pollingStationData.Sum(psm =>
                                    {
                                        if (int.TryParse(psm.VotePolledOtherDocument, out int result))
                                        {
                                            return result;
                                        }
                                        else
                                        {
                                            return 0;
                                        }
                                    });





                                    report.MalePercentage = report.MaleVoters == 0 ? 0 : (double)report.MaleVoters / (double)report.MaleElectoral * 100;
                                    if (double.IsInfinity(report.MalePercentage))
                                    {
                                        report.MalePercentage = 0;
                                    }

                                    report.FemalePercentage = report.FemaleVoters == 0 ? 0 : (double)report.FemaleVoters / (double)report.FemaleElectoral * 100;
                                    if (double.IsInfinity(report.FemalePercentage))
                                    {
                                        report.FemalePercentage = 0;
                                    }
                                    report.ThirdGenderPercentage = report.ThirdGenderVoters == 0 ? 0 : (double)report.ThirdGenderVoters / (double)report.ThirdGenderElectoral * 100;
                                    if (double.IsInfinity(report.ThirdGenderPercentage))
                                    {
                                        report.ThirdGenderPercentage = 0;
                                    }
                                    report.TotalPercentage = report.TotalVoters == 0 ? 0 : (double)report.TotalVoters / (double)report.TotalElectoral * 100;
                                    if (double.IsInfinity(report.TotalPercentage))
                                    {
                                        report.TotalPercentage = 0;
                                    }

                                    consolidateBoothReports.Add(report); assemblylistReport.Add(report);
                                    assemblyCount++;


                                }
                                else
                                {  //type 1: Electorals
                                    report.MaleElectoral = 0;
                                    report.FemaleElectoral = 0;
                                    report.ThirdGenderElectoral = 0;
                                    report.TotalElectoral = 0;

                                    //type 2: Votes polled
                                    report.MaleVoters = 0;
                                    report.FemaleVoters = 0;
                                    report.ThirdGenderVoters = 0;
                                    report.TotalVoters = 0;
                                    report.EPIC = 0;
                                    report.VotePolledOtherDocument = 0;
                                    report.MalePercentage = 0;
                                    report.FemalePercentage = 0;
                                    report.ThirdGenderPercentage = 0;
                                    report.TotalPercentage = 0;

                                    consolidateBoothReports.Add(report); assemblylistReport.Add(report);
                                    assemblyCount++;


                                }


                            }

                        }
                        else
                        {
                            VTReportModel report = new VTReportModel();
                            report.Header = $"{state.StateName}({state.StateCode})";
                            report.Title = $"{state.StateName}";
                            report.Type = "DistrictACWise";
                            report.DistrictName = dis.DistrictName;
                            report.DistrictCode = dis.DistrictCode;
                            report.AssemblyName = "N/A";
                            report.AssemblyCode = "N/A";
                            report.MaleElectoral = 0;
                            report.FemaleElectoral = 0;
                            report.ThirdGenderElectoral = 0;
                            report.TotalElectoral = 0;

                            //type 2: Votes polled
                            report.MaleVoters = 0;
                            report.FemaleVoters = 0;
                            report.ThirdGenderVoters = 0;
                            report.TotalVoters = 0;
                            report.EPIC = 0;
                            report.VotePolledOtherDocument = 0;
                            report.MalePercentage = 0;
                            report.FemalePercentage = 0;
                            report.ThirdGenderPercentage = 0;
                            report.TotalPercentage = 0;
                            consolidateBoothReports.Add(report);

                        }
                        if (assemblyList.Count == assemblyCount)
                        {
                            // add Total Row

                            VTReportModel reportTotal = new VTReportModel();
                            reportTotal.Header = "";
                            reportTotal.Title = "";
                            reportTotal.Type = "";
                            reportTotal.DistrictName = "";
                            reportTotal.DistrictCode = "";
                            reportTotal.AssemblyName = "Total";
                            reportTotal.AssemblyCode = "";
                            reportTotal.MaleElectoral = assemblylistReport.Sum(report => report.MaleElectoral);
                            reportTotal.FemaleElectoral = assemblylistReport.Sum(report => report.FemaleElectoral);
                            reportTotal.ThirdGenderElectoral = assemblylistReport.Sum(report => report.ThirdGenderElectoral);
                            reportTotal.TotalElectoral = assemblylistReport.Sum(report => report.TotalElectoral);
                            reportTotal.MaleVoters = assemblylistReport.Sum(report => report.MaleVoters);
                            reportTotal.FemaleVoters = assemblylistReport.Sum(report => report.FemaleVoters);
                            reportTotal.ThirdGenderVoters = assemblylistReport.Sum(report => report.ThirdGenderVoters);
                            reportTotal.TotalVoters = assemblylistReport.Sum(report => report.TotalVoters);
                            reportTotal.EPIC = assemblylistReport.Sum(report => report.EPIC);
                            reportTotal.VotePolledOtherDocument = assemblylistReport.Sum(report => report.VotePolledOtherDocument);

                            if (assemblylistReport.Any())
                            {
                                reportTotal.MalePercentage = reportTotal.MaleVoters == 0 ? 0 : (double)reportTotal.MaleVoters / (double)reportTotal.MaleElectoral * 100;
                                if (double.IsInfinity(reportTotal.MalePercentage))
                                {
                                    reportTotal.MalePercentage = 0;
                                }
                                reportTotal.FemalePercentage = reportTotal.FemaleVoters == 0 ? 0 : (double)reportTotal.FemaleVoters / (double)reportTotal.FemaleElectoral * 100;
                                if (double.IsInfinity(reportTotal.FemalePercentage))
                                {
                                    reportTotal.FemalePercentage = 0;
                                }
                                reportTotal.ThirdGenderPercentage = reportTotal.ThirdGenderVoters == 0 ? 0 : (double)reportTotal.ThirdGenderVoters / (double)reportTotal.ThirdGenderElectoral * 100;
                                if (double.IsInfinity(reportTotal.ThirdGenderPercentage))
                                {
                                    reportTotal.ThirdGenderPercentage = 0;
                                }
                                reportTotal.TotalPercentage = reportTotal.TotalVoters == 0 ? 0 : (double)reportTotal.TotalVoters / (double)reportTotal.TotalElectoral * 100;
                                if (double.IsInfinity(reportTotal.TotalPercentage))
                                {
                                    reportTotal.TotalPercentage = 0;
                                }
                            }
                            else
                            {
                                // Handle the case when assemblylistReport is empty, for example, set averages to 0
                                reportTotal.MalePercentage = 0;
                                reportTotal.FemalePercentage = 0;
                                reportTotal.ThirdGenderPercentage = 0;
                                reportTotal.TotalPercentage = 0;
                            }

                            assemblylistTotal.Add(reportTotal);
                            consolidateBoothReports.Add(reportTotal);


                        }
                        //}
                    }

                    if (consolidateBoothReports.Count > 0)
                    {
                        // add grand Total after iterations
                        VTReportModel reportGrandTotal = new VTReportModel();
                        reportGrandTotal.Header = "";
                        reportGrandTotal.Title = "";
                        reportGrandTotal.Type = "";
                        reportGrandTotal.DistrictName = "";
                        reportGrandTotal.DistrictCode = "";
                        reportGrandTotal.AssemblyName = "Grand Total";
                        reportGrandTotal.AssemblyCode = "";
                        reportGrandTotal.MaleElectoral = assemblylistTotal.Sum(report => report.MaleElectoral);
                        reportGrandTotal.FemaleElectoral = assemblylistTotal.Sum(report => report.FemaleElectoral);
                        reportGrandTotal.ThirdGenderElectoral = assemblylistTotal.Sum(report => report.ThirdGenderElectoral);
                        reportGrandTotal.TotalElectoral = assemblylistTotal.Sum(report => report.TotalElectoral);
                        reportGrandTotal.MaleVoters = assemblylistTotal.Sum(report => report.MaleVoters);
                        reportGrandTotal.FemaleVoters = assemblylistTotal.Sum(report => report.FemaleVoters);
                        reportGrandTotal.ThirdGenderVoters = assemblylistTotal.Sum(report => report.ThirdGenderVoters);
                        reportGrandTotal.TotalVoters = assemblylistTotal.Sum(report => report.TotalVoters);
                        reportGrandTotal.EPIC = assemblylistTotal.Sum(report => report.EPIC);
                        reportGrandTotal.VotePolledOtherDocument = assemblylistTotal.Sum(report => report.VotePolledOtherDocument);


                        //   reportGrandTotal.MalePercentage = (double)reportGrandTotal.MaleVoters / (double)reportGrandTotal.MaleElectoral * 100;
                        reportGrandTotal.MalePercentage = (reportGrandTotal.MaleVoters != 0 && reportGrandTotal.MaleElectoral != 0)
    ? ((double)reportGrandTotal.MaleVoters / (double)reportGrandTotal.MaleElectoral) * 100
    : 0;

                        //reportGrandTotal.FemalePercentage = (double)reportGrandTotal.FemaleVoters / (double)reportGrandTotal.FemaleElectoral * 100;
                        reportGrandTotal.FemalePercentage = (reportGrandTotal.FemaleVoters != 0 && reportGrandTotal.FemaleElectoral != 0)
         ? ((double)reportGrandTotal.FemaleVoters / (double)reportGrandTotal.FemaleElectoral) * 100 : 0;

                        reportGrandTotal.ThirdGenderPercentage = (reportGrandTotal.ThirdGenderVoters != 0 && reportGrandTotal.ThirdGenderElectoral != 0)
        ? ((double)reportGrandTotal.ThirdGenderVoters / (double)reportGrandTotal.ThirdGenderElectoral) * 100
        : 0;

                        //reportGrandTotal.TotalPercentage = (double)reportGrandTotal.TotalVoters / (double)reportGrandTotal.TotalElectoral * 100;


                        reportGrandTotal.TotalPercentage = (reportGrandTotal.TotalVoters != 0 && reportGrandTotal.TotalElectoral != 0)
       ? ((double)reportGrandTotal.TotalVoters / (double)reportGrandTotal.TotalElectoral) * 100
       : 0;

                        consolidateBoothReports.Add(reportGrandTotal);
                    }

                    return consolidateBoothReports;
                }
            }

            //PCACWise
            else if (boothReportModel.StateMasterId is not 0 && boothReportModel.Type == "PCACWise")

            {
                if (boothReportModel.PCMasterId is not 0 && boothReportModel.AssemblyMasterId is 0)
                {
                    var pcList = _context.ParliamentConstituencyMaster
     .Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.PcStatus == true && d.PCMasterId == boothReportModel.PCMasterId)
     .AsEnumerable().FirstOrDefault();

                    int assemblyCount = 0; List<VTReportModel> assemblylistReport = new List<VTReportModel>();
                    var assemblyList = _context.AssemblyMaster
                          .Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.PCMasterId == boothReportModel.PCMasterId && d.AssemblyStatus == true)
                          .OrderBy(d => d.AssemblyCode)
                          .ToList();
                    if (assemblyList.Count > 0)
                    {
                        foreach (var assembly in assemblyList)
                        {
                            VTReportModel report = new VTReportModel();
                            report.Header = $"{state.StateName}({state.StateCode})";
                            report.Title = $"{state.StateName}";
                            report.Type = "PCACWise";
                            report.PCCode = pcList.PcCodeNo;
                            report.PCName = pcList.PcName;
                            report.AssemblyName = assemblyList.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId)
                                                               .Select(p => p.AssemblyName).FirstOrDefault();
                            report.AssemblyCode = assemblyList.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId)
                                                               .Select(p => p.AssemblyCode.ToString()).FirstOrDefault();
                            var pollingStationData = _context.PollingStationMaster.Include(psm => psm.PollingStationGender).Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId).ToList();
                            if (pollingStationData != null && pollingStationData.Count > 0)
                            {
                                //type 1: Electorals
                                report.MaleElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                          .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
                                                                          .Sum(gender => gender.Male));
                                report.FemaleElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                            .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
                                                                            .Sum(gender => gender.Female));
                                report.ThirdGenderElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                                .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
                                                                                .Sum(gender => gender.ThirdGender));
                                report.TotalElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                           .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
                                                                           .Sum(gender => gender.Total));

                                //type 2: Votes polled
                                report.MaleVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                       .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
                                                                       .Sum(gender => gender.Male));
                                report.FemaleVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                         .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
                                                                         .Sum(gender => gender.Female));
                                report.ThirdGenderVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                             .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
                                                                             .Sum(gender => gender.ThirdGender));
                                report.TotalVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                        .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
                                                                        .Sum(gender => gender.Total));

                                report.EPIC = pollingStationData.Sum(psm => psm.VotePolledEPIC);
                                report.VotePolledOtherDocument = pollingStationData.Sum(psm =>
                                {
                                    if (int.TryParse(psm.VotePolledOtherDocument, out int result))
                                    {
                                        return result;
                                    }
                                    else
                                    {
                                        return 0;
                                    }
                                });
                                report.MalePercentage = report.MaleVoters == 0 ? 0 : (double)report.MaleVoters / (double)report.MaleElectoral * 100;
                                if (double.IsInfinity(report.MalePercentage))
                                {
                                    report.MalePercentage = 0;
                                }

                                report.FemalePercentage = report.FemaleVoters == 0 ? 0 : (double)report.FemaleVoters / (double)report.FemaleElectoral * 100;
                                if (double.IsInfinity(report.FemalePercentage))
                                {
                                    report.FemalePercentage = 0;
                                }
                                report.ThirdGenderPercentage = report.ThirdGenderVoters == 0 ? 0 : (double)report.ThirdGenderVoters / (double)report.ThirdGenderElectoral * 100;
                                if (double.IsInfinity(report.ThirdGenderPercentage))
                                {
                                    report.ThirdGenderPercentage = 0;
                                }
                                report.TotalPercentage = report.TotalVoters == 0 ? 0 : (double)report.TotalVoters / (double)report.TotalElectoral * 100;
                                if (double.IsInfinity(report.TotalPercentage))
                                {
                                    report.TotalPercentage = 0;
                                }
                                consolidateBoothReports.Add(report); assemblylistReport.Add(report);
                                assemblyCount++;


                            }
                            else
                            {  //type 1: Electorals
                                report.MaleElectoral = 0;
                                report.FemaleElectoral = 0;
                                report.ThirdGenderElectoral = 0;
                                report.TotalElectoral = 0;

                                //type 2: Votes polled
                                report.MaleVoters = 0;
                                report.FemaleVoters = 0;
                                report.ThirdGenderVoters = 0;
                                report.TotalVoters = 0;
                                report.EPIC = 0;
                                report.VotePolledOtherDocument = 0;
                                report.MalePercentage = 0;
                                report.FemalePercentage = 0;
                                report.ThirdGenderPercentage = 0;
                                report.TotalPercentage = 0;

                                consolidateBoothReports.Add(report); assemblylistReport.Add(report);
                                assemblyCount++;


                            }


                        }

                    }
                    else
                    {
                        VTReportModel report = new VTReportModel();
                        report.Header = $"{state.StateName}({state.StateCode})";
                        report.Title = $"{state.StateName}";
                        report.Type = "PCACWise";
                        report.PCCode = pcList.PcCodeNo;
                        report.PCName = pcList.PcName;
                        report.AssemblyName = "N/A";
                        report.AssemblyCode = "N/A";
                        report.MaleElectoral = 0;
                        report.FemaleElectoral = 0;
                        report.ThirdGenderElectoral = 0;
                        report.TotalElectoral = 0;

                        //type 2: Votes polled
                        report.MaleVoters = 0;
                        report.FemaleVoters = 0;
                        report.ThirdGenderVoters = 0;
                        report.TotalVoters = 0;
                        report.EPIC = 0;
                        report.VotePolledOtherDocument = 0;
                        report.MalePercentage = 0;
                        report.FemalePercentage = 0;
                        report.ThirdGenderPercentage = 0;
                        report.TotalPercentage = 0;
                        consolidateBoothReports.Add(report);

                    }
                    if (assemblyList.Count == assemblyCount)
                    {
                        // add Grand Total Row

                        VTReportModel reportTotal = new VTReportModel();
                        reportTotal.Header = "";
                        reportTotal.Title = "";
                        reportTotal.Type = "";
                        reportTotal.DistrictName = "";
                        reportTotal.DistrictCode = "";
                        reportTotal.AssemblyName = "Total";
                        reportTotal.AssemblyCode = "";
                        reportTotal.MaleElectoral = assemblylistReport.Sum(report => report.MaleElectoral);
                        reportTotal.FemaleElectoral = assemblylistReport.Sum(report => report.FemaleElectoral);
                        reportTotal.ThirdGenderElectoral = assemblylistReport.Sum(report => report.ThirdGenderElectoral);
                        reportTotal.TotalElectoral = assemblylistReport.Sum(report => report.TotalElectoral);
                        reportTotal.MaleVoters = assemblylistReport.Sum(report => report.MaleVoters);
                        reportTotal.FemaleVoters = assemblylistReport.Sum(report => report.FemaleVoters);
                        reportTotal.ThirdGenderVoters = assemblylistReport.Sum(report => report.ThirdGenderVoters);
                        reportTotal.TotalVoters = assemblylistReport.Sum(report => report.TotalVoters);
                        reportTotal.EPIC = assemblylistReport.Sum(report => report.EPIC);
                        reportTotal.VotePolledOtherDocument = assemblylistReport.Sum(report => report.VotePolledOtherDocument);
                        //   reportGrandTotal.MalePercentage = (double)reportGrandTotal.MaleVoters / (double)reportGrandTotal.MaleElectoral * 100;
                        reportTotal.MalePercentage = (reportTotal.MaleVoters != 0 && reportTotal.MaleElectoral != 0)
    ? ((double)reportTotal.MaleVoters / (double)reportTotal.MaleElectoral) * 100
    : 0;

                        //reportGrandTotal.FemalePercentage = (double)reportGrandTotal.FemaleVoters / (double)reportGrandTotal.FemaleElectoral * 100;
                        reportTotal.FemalePercentage = (reportTotal.FemaleVoters != 0 && reportTotal.FemaleElectoral != 0)
         ? ((double)reportTotal.FemaleVoters / (double)reportTotal.FemaleElectoral) * 100 : 0;

                        reportTotal.ThirdGenderPercentage = (reportTotal.ThirdGenderVoters != 0 && reportTotal.ThirdGenderElectoral != 0)
        ? ((double)reportTotal.ThirdGenderVoters / (double)reportTotal.ThirdGenderElectoral) * 100
        : 0;

                        //reportGrandTotal.TotalPercentage = (double)reportGrandTotal.TotalVoters / (double)reportGrandTotal.TotalElectoral * 100;


                        reportTotal.TotalPercentage = (reportTotal.TotalVoters != 0 && reportTotal.TotalElectoral != 0)
       ? ((double)reportTotal.TotalVoters / (double)reportTotal.TotalElectoral) * 100
       : 0;
                        assemblylistTotal.Add(reportTotal);
                        consolidateBoothReports.Add(reportTotal);

                    }


                    return consolidateBoothReports;
                }
                else if (boothReportModel.PCMasterId is not 0 && boothReportModel.AssemblyMasterId is not 0)
                {
                    var pcList = _context.ParliamentConstituencyMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.PcStatus == true && d.PCMasterId == boothReportModel.PCMasterId).AsEnumerable().FirstOrDefault();

                    var assemblyList = _context.AssemblyMaster
                        .Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.PCMasterId == boothReportModel.PCMasterId && d.AssemblyStatus == true && d.AssemblyMasterId == boothReportModel.AssemblyMasterId)
                        .OrderBy(d => d.AssemblyCode)
                        .FirstOrDefault();

                    var boothList = _context.BoothMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.BoothStatus && d.AssemblyMasterId == boothReportModel.AssemblyMasterId && d.AssemblyMasterId == assemblyList.AssemblyMasterId).ToList();

                    if (boothList.Count > 0)
                    {
                        foreach (var booth in boothList)
                        {
                            VTReportModel report = new VTReportModel();
                            report.Header = $"{state.StateName}({state.StateCode})";
                            report.Title = $"{state.StateName},({pcList.PcName})";
                            report.Type = "PCACWise";
                            report.PCCode = pcList.PcCodeNo;
                            report.PCName = pcList.PcName;
                            report.AssemblyName = assemblyList.AssemblyName;
                            report.AssemblyCode = assemblyList.AssemblyCode.ToString();
                            var BoothRecord = _context.BoothMaster.Where(d => d.BoothMasterId == booth.BoothMasterId).FirstOrDefault();
                            report.BoothName = BoothRecord.BoothName + " " + BoothRecord.BoothCode_No.ToString();

                            var pollingStationData = _context.PollingStationMaster.Include(psm => psm.PollingStationGender).Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.PCasterId == boothReportModel.PCMasterId && d.BoothMasterId == booth.BoothMasterId).ToList();
                            if (pollingStationData != null && pollingStationData.Count > 0)
                            {
                                //type 1: Electorals

                                report.MaleElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                          .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
                                                                          .Sum(gender => gender.Male));
                                report.FemaleElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                            .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
                                                                            .Sum(gender => gender.Female));
                                report.ThirdGenderElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                                .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
                                                                                .Sum(gender => gender.ThirdGender));
                                report.TotalElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                           .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
                                                                           .Sum(gender => gender.Total));

                                //type 2: Votes polled
                                report.MaleVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                       .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
                                                                       .Sum(gender => gender.Male));
                                report.FemaleVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                         .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
                                                                         .Sum(gender => gender.Female));
                                report.ThirdGenderVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                             .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
                                                                             .Sum(gender => gender.ThirdGender));
                                report.TotalVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                        .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
                                                                        .Sum(gender => gender.Total));

                                report.EPIC = pollingStationData.Sum(psm => psm.VotePolledEPIC);
                                report.VotePolledOtherDocument = pollingStationData.Sum(psm =>
                                {
                                    if (int.TryParse(psm.VotePolledOtherDocument, out int result))
                                    {
                                        return result;
                                    }
                                    else
                                    {
                                        return 0;
                                    }
                                });
                                report.MalePercentage = report.MaleVoters == 0 ? 0 : (double)report.MaleVoters / (double)report.MaleElectoral * 100;
                                if (double.IsInfinity(report.MalePercentage))
                                {
                                    report.MalePercentage = 0;
                                }

                                report.FemalePercentage = report.FemaleVoters == 0 ? 0 : (double)report.FemaleVoters / (double)report.FemaleElectoral * 100;
                                if (double.IsInfinity(report.FemalePercentage))
                                {
                                    report.FemalePercentage = 0;
                                }
                                report.ThirdGenderPercentage = report.ThirdGenderVoters == 0 ? 0 : (double)report.ThirdGenderVoters / (double)report.ThirdGenderElectoral * 100;
                                if (double.IsInfinity(report.ThirdGenderPercentage))
                                {
                                    report.ThirdGenderPercentage = 0;
                                }
                                report.TotalPercentage = report.TotalVoters == 0 ? 0 : (double)report.TotalVoters / (double)report.TotalElectoral * 100;
                                if (double.IsInfinity(report.TotalPercentage))
                                {
                                    report.TotalPercentage = 0;
                                }
                                consolidateBoothReports.Add(report);



                            }
                            else
                            {  //type 1: Electorals
                                report.MaleElectoral = 0;
                                report.FemaleElectoral = 0;
                                report.ThirdGenderElectoral = 0;
                                report.TotalElectoral = 0;

                                //type 2: Votes polled
                                report.MaleVoters = 0;
                                report.FemaleVoters = 0;
                                report.ThirdGenderVoters = 0;
                                report.TotalVoters = 0;
                                report.EPIC = 0;
                                report.VotePolledOtherDocument = 0;
                                report.MalePercentage = 0;
                                report.FemalePercentage = 0;
                                report.ThirdGenderPercentage = 0;
                                report.TotalPercentage = 0;
                                consolidateBoothReports.Add(report);



                            }

                        }

                    }
                    else

                    {
                        VTReportModel report = new VTReportModel();
                        report.Header = $"{state.StateName}({state.StateCode})";
                        report.Title = $"{state.StateName},({pcList.PcName})";
                        report.Type = "PCACWise";
                        report.AssemblyName = "N/A";
                        report.AssemblyCode = "N/A";
                        report.MaleElectoral = 0;
                        report.FemaleElectoral = 0;
                        report.ThirdGenderElectoral = 0;
                        report.TotalElectoral = 0;

                        //type 2: Votes polled
                        report.MaleVoters = 0;
                        report.FemaleVoters = 0;
                        report.ThirdGenderVoters = 0;
                        report.TotalVoters = 0;
                        report.EPIC = 0;
                        report.VotePolledOtherDocument = 0;
                        report.MalePercentage = 0;
                        report.FemalePercentage = 0;
                        report.ThirdGenderPercentage = 0;
                        report.TotalPercentage = 0;
                        consolidateBoothReports.Add(report);
                    }
                    // add Grand Total Row

                    VTReportModel reportTotal = new VTReportModel();
                    reportTotal.Header = "";
                    reportTotal.Title = "";
                    reportTotal.Type = "";
                    reportTotal.DistrictName = "";
                    reportTotal.DistrictCode = "";
                    reportTotal.AssemblyName = "Total";
                    reportTotal.AssemblyCode = "";
                    reportTotal.MaleElectoral = consolidateBoothReports.Sum(report => report.MaleElectoral);
                    reportTotal.FemaleElectoral = consolidateBoothReports.Sum(report => report.FemaleElectoral);
                    reportTotal.ThirdGenderElectoral = consolidateBoothReports.Sum(report => report.ThirdGenderElectoral);
                    reportTotal.TotalElectoral = consolidateBoothReports.Sum(report => report.TotalElectoral);
                    reportTotal.MaleVoters = consolidateBoothReports.Sum(report => report.MaleVoters);
                    reportTotal.FemaleVoters = consolidateBoothReports.Sum(report => report.FemaleVoters);
                    reportTotal.ThirdGenderVoters = consolidateBoothReports.Sum(report => report.ThirdGenderVoters);
                    reportTotal.TotalVoters = consolidateBoothReports.Sum(report => report.TotalVoters);
                    reportTotal.EPIC = consolidateBoothReports.Sum(report => report.EPIC);
                    reportTotal.VotePolledOtherDocument = consolidateBoothReports.Sum(report => report.VotePolledOtherDocument);


                    reportTotal.MalePercentage = (reportTotal.MaleVoters != 0 && reportTotal.MaleElectoral != 0)
  ? ((double)reportTotal.MaleVoters / (double)reportTotal.MaleElectoral) * 100
  : 0;

                    //reportGrandTotal.FemalePercentage = (double)reportGrandTotal.FemaleVoters / (double)reportGrandTotal.FemaleElectoral * 100;
                    reportTotal.FemalePercentage = (reportTotal.FemaleVoters != 0 && reportTotal.FemaleElectoral != 0)
     ? ((double)reportTotal.FemaleVoters / (double)reportTotal.FemaleElectoral) * 100 : 0;

                    reportTotal.ThirdGenderPercentage = (reportTotal.ThirdGenderVoters != 0 && reportTotal.ThirdGenderElectoral != 0)
    ? ((double)reportTotal.ThirdGenderVoters / (double)reportTotal.ThirdGenderElectoral) * 100
    : 0;

                    //reportGrandTotal.TotalPercentage = (double)reportGrandTotal.TotalVoters / (double)reportGrandTotal.TotalElectoral * 100;


                    reportTotal.TotalPercentage = (reportTotal.TotalVoters != 0 && reportTotal.TotalElectoral != 0)
   ? ((double)reportTotal.TotalVoters / (double)reportTotal.TotalElectoral) * 100
   : 0;



                    consolidateBoothReports.Add(reportTotal);



                    return consolidateBoothReports;



                }
                else

                {

                    var pcList = _context.ParliamentConstituencyMaster
    .Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.PcStatus == true)
    .AsEnumerable() // Switch to client-side evaluation
    .OrderBy(p => int.Parse(p.PcCodeNo))
    .ToList();


                    foreach (var pc in pcList)
                    {
                        int assemblyCount = 0; List<VTReportModel> assemblylistReport = new List<VTReportModel>();
                        var assemblyList = _context.AssemblyMaster
                           .Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.PCMasterId == pc.PCMasterId && d.AssemblyStatus == true)
                           .OrderBy(d => d.AssemblyCode)
                           .ToList();

                        if (assemblyList.Count > 0)
                        {
                            foreach (var assembly in assemblyList)
                            {
                                VTReportModel report = new VTReportModel();
                                report.Header = $"{state.StateName}({state.StateCode}),{pc.PcName}({pc.PcCodeNo})";
                                report.Title = $"{pc.PcName}";
                                report.Type = "PCACWise";
                                report.PCCode = pc.PcCodeNo;
                                report.PCName = pc.PcName;
                                report.AssemblyName = assemblyList.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId)
                                                                   .Select(p => p.AssemblyName).FirstOrDefault();
                                report.AssemblyCode = assemblyList.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId)
                                                                   .Select(p => p.AssemblyCode.ToString()).FirstOrDefault();
                                var pollingStationData = _context.PollingStationMaster.Include(psm => psm.PollingStationGender).Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId).ToList();
                                if (pollingStationData != null && pollingStationData.Count > 0)
                                {
                                    //type 1: Electorals
                                    report.MaleElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                              .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
                                                                              .Sum(gender => gender.Male));
                                    report.FemaleElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                                .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
                                                                                .Sum(gender => gender.Female));
                                    report.ThirdGenderElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                                    .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
                                                                                    .Sum(gender => gender.ThirdGender));
                                    report.TotalElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                               .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
                                                                               .Sum(gender => gender.Total));

                                    //type 2: Votes polled
                                    report.MaleVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                           .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
                                                                           .Sum(gender => gender.Male));
                                    report.FemaleVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                             .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
                                                                             .Sum(gender => gender.Female));
                                    report.ThirdGenderVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                                 .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
                                                                                 .Sum(gender => gender.ThirdGender));
                                    report.TotalVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                            .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
                                                                            .Sum(gender => gender.Total));

                                    report.EPIC = pollingStationData.Sum(psm => psm.VotePolledEPIC);
                                    report.VotePolledOtherDocument = pollingStationData.Sum(psm =>
                                    {
                                        if (int.TryParse(psm.VotePolledOtherDocument, out int result))
                                        {
                                            return result;
                                        }
                                        else
                                        {
                                            return 0;
                                        }
                                    });
                                    report.MalePercentage = report.MaleVoters == 0 ? 0 : (double)report.MaleVoters / (double)report.MaleElectoral * 100;
                                    if (double.IsInfinity(report.MalePercentage))
                                    {
                                        report.MalePercentage = 0;
                                    }

                                    report.FemalePercentage = report.FemaleVoters == 0 ? 0 : (double)report.FemaleVoters / (double)report.FemaleElectoral * 100;
                                    if (double.IsInfinity(report.FemalePercentage))
                                    {
                                        report.FemalePercentage = 0;
                                    }
                                    report.ThirdGenderPercentage = report.ThirdGenderVoters == 0 ? 0 : (double)report.ThirdGenderVoters / (double)report.ThirdGenderElectoral * 100;
                                    if (double.IsInfinity(report.ThirdGenderPercentage))
                                    {
                                        report.ThirdGenderPercentage = 0;
                                    }
                                    report.TotalPercentage = report.TotalVoters == 0 ? 0 : (double)report.TotalVoters / (double)report.TotalElectoral * 100;
                                    if (double.IsInfinity(report.TotalPercentage))
                                    {
                                        report.TotalPercentage = 0;
                                    }


                                    consolidateBoothReports.Add(report); assemblylistReport.Add(report);
                                    assemblyCount++;


                                }
                                else
                                {  //type 1: Electorals
                                    report.MaleElectoral = 0;
                                    report.FemaleElectoral = 0;
                                    report.ThirdGenderElectoral = 0;
                                    report.TotalElectoral = 0;

                                    //type 2: Votes polled
                                    report.MaleVoters = 0;
                                    report.FemaleVoters = 0;
                                    report.ThirdGenderVoters = 0;
                                    report.TotalVoters = 0;
                                    report.EPIC = 0;
                                    report.VotePolledOtherDocument = 0;
                                    report.MalePercentage = 0;
                                    report.FemalePercentage = 0;
                                    report.ThirdGenderPercentage = 0;
                                    report.TotalPercentage = 0;
                                    consolidateBoothReports.Add(report); assemblylistReport.Add(report);
                                    assemblyCount++;


                                }


                            }

                        }
                        else
                        {
                            VTReportModel report = new VTReportModel();
                            report.Header = $"{state.StateName}({state.StateCode})";
                            report.Title = $"{state.StateName}";
                            report.Type = "PCACWise";
                            report.PCCode = pc.PcCodeNo;
                            report.PCName = pc.PcName;
                            report.AssemblyName = "N/A";
                            report.AssemblyCode = "N/A";
                            report.MaleElectoral = 0;
                            report.FemaleElectoral = 0;
                            report.ThirdGenderElectoral = 0;
                            report.TotalElectoral = 0;

                            //type 2: Votes polled
                            report.MaleVoters = 0;
                            report.FemaleVoters = 0;
                            report.ThirdGenderVoters = 0;
                            report.TotalVoters = 0;
                            report.EPIC = 0;
                            report.MalePercentage = 0;
                            report.FemalePercentage = 0;
                            report.ThirdGenderPercentage = 0;
                            report.TotalPercentage = 0;
                            report.VotePolledOtherDocument = 0;
                            consolidateBoothReports.Add(report);

                        }
                        if (assemblyList.Count == assemblyCount)
                        {
                            // add Total Row
                            VTReportModel reportTotal = new VTReportModel();
                            reportTotal.Header = "";
                            reportTotal.Title = "";
                            reportTotal.Type = "";
                            reportTotal.DistrictName = "";
                            reportTotal.DistrictCode = "";

                            reportTotal.AssemblyName = "Total";
                            reportTotal.AssemblyCode = "";
                            reportTotal.MaleElectoral = assemblylistReport.Sum(report => report.MaleElectoral);
                            reportTotal.FemaleElectoral = assemblylistReport.Sum(report => report.FemaleElectoral);
                            reportTotal.ThirdGenderElectoral = assemblylistReport.Sum(report => report.ThirdGenderElectoral);
                            reportTotal.TotalElectoral = assemblylistReport.Sum(report => report.TotalElectoral);
                            reportTotal.MaleVoters = assemblylistReport.Sum(report => report.MaleVoters);
                            reportTotal.FemaleVoters = assemblylistReport.Sum(report => report.FemaleVoters);
                            reportTotal.ThirdGenderVoters = assemblylistReport.Sum(report => report.ThirdGenderVoters);
                            reportTotal.TotalVoters = assemblylistReport.Sum(report => report.TotalVoters);
                            reportTotal.EPIC = assemblylistReport.Sum(report => report.EPIC);
                            reportTotal.VotePolledOtherDocument = assemblylistReport.Sum(report => report.VotePolledOtherDocument);

                            if (assemblylistReport.Any())
                            {
                                reportTotal.MalePercentage = reportTotal.MaleVoters == 0 ? 0 : (double)reportTotal.MaleVoters / (double)reportTotal.MaleElectoral * 100;
                                if (double.IsInfinity(reportTotal.MalePercentage))
                                {
                                    reportTotal.MalePercentage = 0;
                                }
                                reportTotal.FemalePercentage = reportTotal.FemaleVoters == 0 ? 0 : (double)reportTotal.FemaleVoters / (double)reportTotal.FemaleElectoral * 100;
                                if (double.IsInfinity(reportTotal.FemalePercentage))
                                {
                                    reportTotal.FemalePercentage = 0;
                                }
                                reportTotal.ThirdGenderPercentage = reportTotal.ThirdGenderVoters == 0 ? 0 : (double)reportTotal.ThirdGenderVoters / (double)reportTotal.ThirdGenderElectoral * 100;
                                if (double.IsInfinity(reportTotal.ThirdGenderPercentage))
                                {
                                    reportTotal.ThirdGenderPercentage = 0;
                                }
                                reportTotal.TotalPercentage = reportTotal.TotalVoters == 0 ? 0 : (double)reportTotal.TotalVoters / (double)reportTotal.TotalElectoral * 100;
                                if (double.IsInfinity(reportTotal.TotalPercentage))
                                {
                                    reportTotal.TotalPercentage = 0;
                                }
                            }
                            else
                            {
                                // Handle the case when assemblylistReport is empty, for example, set averages to 0
                                reportTotal.MalePercentage = 0;
                                reportTotal.FemalePercentage = 0;
                                reportTotal.ThirdGenderPercentage = 0;
                                reportTotal.TotalPercentage = 0;
                            }




                            assemblylistTotal.Add(reportTotal);
                            consolidateBoothReports.Add(reportTotal);

                        }

                    }
                    if (consolidateBoothReports.Count > 0)
                    {
                        // add grand Total after iterations
                        VTReportModel reportGrandTotal = new VTReportModel();
                        reportGrandTotal.Header = "";
                        reportGrandTotal.Title = "";
                        reportGrandTotal.Type = "";
                        reportGrandTotal.DistrictName = "";
                        reportGrandTotal.DistrictCode = "";
                        reportGrandTotal.AssemblyName = "Grand Total";
                        reportGrandTotal.AssemblyCode = "";
                        reportGrandTotal.MaleElectoral = assemblylistTotal.Sum(report => report.MaleElectoral);
                        reportGrandTotal.FemaleElectoral = assemblylistTotal.Sum(report => report.FemaleElectoral);
                        reportGrandTotal.ThirdGenderElectoral = assemblylistTotal.Sum(report => report.ThirdGenderElectoral);
                        reportGrandTotal.TotalElectoral = assemblylistTotal.Sum(report => report.TotalElectoral);
                        reportGrandTotal.MaleVoters = assemblylistTotal.Sum(report => report.MaleVoters);
                        reportGrandTotal.FemaleVoters = assemblylistTotal.Sum(report => report.FemaleVoters);
                        reportGrandTotal.ThirdGenderVoters = assemblylistTotal.Sum(report => report.ThirdGenderVoters);
                        reportGrandTotal.TotalVoters = assemblylistTotal.Sum(report => report.TotalVoters);
                        reportGrandTotal.EPIC = assemblylistTotal.Sum(report => report.EPIC);
                        reportGrandTotal.VotePolledOtherDocument = assemblylistTotal.Sum(report => report.VotePolledOtherDocument);


                        //   reportGrandTotal.MalePercentage = (double)reportGrandTotal.MaleVoters / (double)reportGrandTotal.MaleElectoral * 100;
                        reportGrandTotal.MalePercentage = (reportGrandTotal.MaleVoters != 0 && reportGrandTotal.MaleElectoral != 0)
    ? ((double)reportGrandTotal.MaleVoters / (double)reportGrandTotal.MaleElectoral) * 100
    : 0;

                        //reportGrandTotal.FemalePercentage = (double)reportGrandTotal.FemaleVoters / (double)reportGrandTotal.FemaleElectoral * 100;
                        reportGrandTotal.FemalePercentage = (reportGrandTotal.FemaleVoters != 0 && reportGrandTotal.FemaleElectoral != 0)
         ? ((double)reportGrandTotal.FemaleVoters / (double)reportGrandTotal.FemaleElectoral) * 100 : 0;

                        reportGrandTotal.ThirdGenderPercentage = (reportGrandTotal.ThirdGenderVoters != 0 && reportGrandTotal.ThirdGenderElectoral != 0)
        ? ((double)reportGrandTotal.ThirdGenderVoters / (double)reportGrandTotal.ThirdGenderElectoral) * 100
        : 0;

                        //reportGrandTotal.TotalPercentage = (double)reportGrandTotal.TotalVoters / (double)reportGrandTotal.TotalElectoral * 100;


                        reportGrandTotal.TotalPercentage = (reportGrandTotal.TotalVoters != 0 && reportGrandTotal.TotalElectoral != 0)
       ? ((double)reportGrandTotal.TotalVoters / (double)reportGrandTotal.TotalElectoral) * 100
       : 0;

                        consolidateBoothReports.Add(reportGrandTotal);
                    }

                    return consolidateBoothReports;

                    return consolidateBoothReports;
                }
            }

            //DetailedDistrictACWise
            if (boothReportModel.StateMasterId is not 0 && boothReportModel.Type == "DetailedDistrictACWise")
            {
                var districtList = _context.DistrictMaster
    .Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictStatus)
    .AsEnumerable() // Switch to client-side evaluation
    .OrderBy(p => int.Parse(p.DistrictCode))
    .ToList();


                foreach (var dis in districtList)
                {
                    var assemblyList = _context.AssemblyMaster
                        .Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == dis.DistrictMasterId)
                        .OrderBy(d => d.AssemblyCode)
                        .ToList();

                    if (assemblyList.Count > 0)
                    {
                        foreach (var assembly in assemblyList)
                        {
                            VTReportModel report = new VTReportModel();
                            report.Header = $"{state.StateName}({state.StateCode})";
                            report.Title = $"{state.StateName}";
                            report.Type = "DetailedDistrictACWise";
                            report.DistrictName = dis.DistrictName;
                            report.DistrictCode = dis.DistrictCode;
                            report.AssemblyName = assemblyList.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId)
                                                               .Select(p => p.AssemblyName).FirstOrDefault();
                            report.AssemblyCode = assemblyList.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId)
                                                               .Select(p => p.AssemblyCode.ToString()).FirstOrDefault();
                            var pollingStationData = _context.PollingStationMaster.Include(psm => psm.PollingStationGender).Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId).ToList();
                            if (pollingStationData != null && pollingStationData.Count > 0)
                            {
                                //type 1: Electorals
                                report.MaleElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                          .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
                                                                          .Sum(gender => gender.Male));
                                report.FemaleElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                            .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
                                                                            .Sum(gender => gender.Female));
                                report.ThirdGenderElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                                .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
                                                                                .Sum(gender => gender.ThirdGender));
                                report.TotalElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                           .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
                                                                           .Sum(gender => gender.Total));

                                //type 2: Votes polled
                                report.MaleVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                       .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
                                                                       .Sum(gender => gender.Male));
                                report.FemaleVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                         .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
                                                                         .Sum(gender => gender.Female));
                                report.ThirdGenderVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                             .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
                                                                             .Sum(gender => gender.ThirdGender));
                                report.TotalVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                        .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
                                                                        .Sum(gender => gender.Total));

                                report.EPIC = pollingStationData.Sum(psm => psm.VotePolledEPIC);
                                report.VotePolledOtherDocument = pollingStationData.Sum(psm =>
                                {
                                    if (int.TryParse(psm.VotePolledOtherDocument, out int result))
                                    {
                                        return result;
                                    }
                                    else
                                    {
                                        return 0;
                                    }
                                });
                                report.TotalCUsUsed = pollingStationData.Sum(psm => psm.TotalCUsUsed);
                                report.TotalBUsUsed = pollingStationData.Sum(psm => psm.TotalBUsUsed);
                                report.TotalVVPATUsed = pollingStationData.Sum(psm => psm.TotalVVPATUsed);

                                report.OverseasElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                     .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 4)
                                                                     .Sum(gender => gender.Total));
                                report.PWdEelectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                   .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 11)
                                                                   .Sum(gender => gender.Total));
                                report.YoungElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                  .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 3)
                                                                  .Sum(gender => gender.Total));
                                report.FortyNineVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
                                                                 .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 6)
                                                                 .Sum(gender => gender.Total));

                                report.TenderedVotes = pollingStationData.Sum(psm => psm.TenderedVote);

                                consolidateBoothReports.Add(report);



                            }
                            else
                            {  //type 1: Electorals
                                report.MaleElectoral = 0;
                                report.FemaleElectoral = 0;
                                report.ThirdGenderElectoral = 0;
                                report.TotalElectoral = 0;

                                //type 2: Votes polled
                                report.MaleVoters = 0;
                                report.FemaleVoters = 0;
                                report.ThirdGenderVoters = 0;
                                report.TotalVoters = 0;
                                report.EPIC = 0;
                                report.VotePolledOtherDocument = 0;
                                report.TotalCUsUsed = 0;
                                report.TotalBUsUsed = 0;
                                report.TotalVVPATUsed = 0;

                                report.OverseasElectoral = 0;
                                report.PWdEelectoral = 0;
                                report.YoungElectoral = 0;
                                report.FortyNineVoters = 0;

                                report.TenderedVotes = 0;
                                consolidateBoothReports.Add(report);



                            }


                        }

                    }
                    else
                    {
                        VTReportModel report = new VTReportModel();
                        report.Header = $"{state.StateName}({state.StateCode})";
                        report.Title = $"{state.StateName}";
                        report.Type = "DetailedDistrictACWise";
                        report.DistrictName = dis.DistrictName;
                        report.DistrictCode = dis.DistrictCode;
                        report.AssemblyName = "N/A";
                        report.AssemblyCode = "N/A";
                        //type 1: Electorals
                        report.MaleElectoral = 0;
                        report.FemaleElectoral = 0;
                        report.ThirdGenderElectoral = 0;
                        report.TotalElectoral = 0;

                        //type 2: Votes polled
                        report.MaleVoters = 0;
                        report.FemaleVoters = 0;
                        report.ThirdGenderVoters = 0;
                        report.TotalVoters = 0;
                        report.EPIC = 0;
                        report.VotePolledOtherDocument = 0;
                        report.TotalCUsUsed = 0;
                        report.TotalBUsUsed = 0;
                        report.TotalVVPATUsed = 0;

                        report.OverseasElectoral = 0;
                        report.PWdEelectoral = 0;
                        report.YoungElectoral = 0;
                        report.FortyNineVoters = 0;

                        report.TenderedVotes = 0;
                        consolidateBoothReports.Add(report);

                    }


                }
                // add grand Total after iterations
                VTReportModel reportGrandTotal = new VTReportModel();
                reportGrandTotal.Header = "";
                reportGrandTotal.Title = "";
                reportGrandTotal.Type = "";
                reportGrandTotal.DistrictName = "";
                reportGrandTotal.DistrictCode = "";
                reportGrandTotal.AssemblyName = "Grand Total";
                reportGrandTotal.AssemblyCode = "";
                reportGrandTotal.MaleElectoral = consolidateBoothReports.Sum(report => report.MaleElectoral);
                reportGrandTotal.FemaleElectoral = consolidateBoothReports.Sum(report => report.FemaleElectoral);
                reportGrandTotal.ThirdGenderElectoral = consolidateBoothReports.Sum(report => report.ThirdGenderElectoral);
                reportGrandTotal.TotalElectoral = consolidateBoothReports.Sum(report => report.TotalElectoral);
                reportGrandTotal.MaleVoters = consolidateBoothReports.Sum(report => report.MaleVoters);
                reportGrandTotal.FemaleVoters = consolidateBoothReports.Sum(report => report.FemaleVoters);
                reportGrandTotal.ThirdGenderVoters = consolidateBoothReports.Sum(report => report.ThirdGenderVoters);
                reportGrandTotal.TotalVoters = consolidateBoothReports.Sum(report => report.TotalVoters);
                reportGrandTotal.EPIC = consolidateBoothReports.Sum(report => report.EPIC);
                reportGrandTotal.VotePolledOtherDocument = consolidateBoothReports.Sum(report => report.VotePolledOtherDocument);

                reportGrandTotal.TotalCUsUsed = consolidateBoothReports.Sum(report => report.TotalCUsUsed);
                reportGrandTotal.TotalBUsUsed = consolidateBoothReports.Sum(report => report.TotalBUsUsed);
                reportGrandTotal.TotalVVPATUsed = consolidateBoothReports.Sum(report => report.TotalVVPATUsed);

                reportGrandTotal.OverseasElectoral = consolidateBoothReports.Sum(report => report.OverseasElectoral);
                reportGrandTotal.PWdEelectoral = consolidateBoothReports.Sum(report => report.PWdEelectoral);
                reportGrandTotal.YoungElectoral = consolidateBoothReports.Sum(report => report.YoungElectoral);
                reportGrandTotal.FortyNineVoters = consolidateBoothReports.Sum(report => report.FortyNineVoters);

                reportGrandTotal.TenderedVotes = consolidateBoothReports.Sum(report => report.TenderedVotes);
                consolidateBoothReports.Add(reportGrandTotal);

                return consolidateBoothReports;
            }

            return consolidateBoothReports;
        }

        #endregion

        #region Get Slot Wiseby Id and Other is all  slots Voter Turn Out Report


        public async Task<List<VTReportModel>> GetSlotBasedVoterTurnOutReport(SlotVTReportModel boothReportModel)
        {
            List<VTReportModel> consolidateBoothReports = new List<VTReportModel>();
            List<VTReportModel> FinalList = new List<VTReportModel>();
            var state = _context.StateMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.StateStatus == true).Select(d => new { d.StateName, d.StateCode }).FirstOrDefault();

            var district = new { DistrictName = "", DistrictCode = "" };
            var pcMaster = new { PcName = "", PcCodeNo = "" };
            var slotMaster = new { StartTime = new TimeOnly(), EndTime = new TimeOnly?(), LockTime = new TimeOnly?() };



            if (boothReportModel.SlotMasterId > 0)
            {
                slotMaster = _context.SlotManagementMaster
               .Where(d => d.SlotManagementId == boothReportModel.SlotMasterId)
               .Select(d => new { d.StartTime, d.EndTime, d.LockTime })
               .FirstOrDefault();
            }
            if (boothReportModel.DistrictMasterId is not 0)
            {
                district = _context.DistrictMaster
                    .Where(d => d.DistrictMasterId == boothReportModel.DistrictMasterId && d.StateMasterId == boothReportModel.StateMasterId && d.DistrictStatus == true)
                    .Select(d => new { d.DistrictName, d.DistrictCode })
                    .FirstOrDefault();
            }

            if (boothReportModel.PCMasterId is not 0)
            {
                pcMaster = _context.ParliamentConstituencyMaster
                    .Where(d => d.PCMasterId == boothReportModel.PCMasterId && d.PcStatus == true)
                    .Select(d => new { d.PcName, d.PcCodeNo })
                    .FirstOrDefault();
            }
            //State
            if (boothReportModel.StateMasterId is not 0 && boothReportModel.DistrictMasterId is 0 && boothReportModel.AssemblyMasterId is 0 && boothReportModel.PCMasterId is 0)
            {

                var assemblyList = _context.AssemblyMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.AssemblyStatus == true).Include(d => d.BoothMaster).ToList();

                foreach (var assembly in assemblyList)
                {
                    VTReportModel report = new VTReportModel
                    {
                        // Populate your ConsolidateBoothReport properties here based on assembly data
                        Header = $"{state.StateName}({state.StateCode})",
                        Title = "State Slot Report" + $"({slotMaster.StartTime.ToString()} ) , ( {slotMaster.EndTime.ToString()} ) , ({slotMaster.LockTime.ToString()})",
                        Type = "State",

                        DistrictName = _context.DistrictMaster.Where(d => d.DistrictMasterId == assembly.DistrictMasterId && d.DistrictStatus == true).Select(d => d.DistrictName).FirstOrDefault(),

                        MaleElectoral = assembly.BoothMaster.Select(d => d.Male).Sum(),
                        FemaleElectoral = assembly.BoothMaster.Select(d => d.Female).Sum(),
                        ThirdGenderElectoral = assembly.BoothMaster.Select(d => d.Transgender).Sum(),
                        TotalElectoral = assembly.BoothMaster.Select(d => d.TotalVoters).Sum(),

                        MaleVoters = _context.PollDetails.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.SlotManagementId == boothReportModel.SlotMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId)
                         .GroupBy(d => d.BoothMasterId)
                         .Select(group => new
                         {
                             BoothMasterId = group.Key,
                             TotalMaleVoters = group.OrderByDescending(d => d.VotesPolledRecivedTime).FirstOrDefault().Male // Summing only the first record of each group
                         })
                         .Sum(result => result.TotalMaleVoters),
                        FemaleVoters = _context.PollDetails.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.SlotManagementId == boothReportModel.SlotMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId)
                         .GroupBy(d => d.BoothMasterId)
                         .Select(group => new
                         {
                             BoothMasterId = group.Key,
                             TotalFemaleVoters = group.OrderByDescending(d => d.VotesPolledRecivedTime).FirstOrDefault().Female // Summing only the first record of each group
                         })
                         .Sum(result => result.TotalFemaleVoters),
                        ThirdGenderVoters = _context.PollDetails.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.SlotManagementId == boothReportModel.SlotMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId)
                         .GroupBy(d => d.BoothMasterId)
                         .Select(group => new
                         {
                             BoothMasterId = group.Key,
                             TotalTransgenderVoters = group.OrderByDescending(d => d.VotesPolledRecivedTime).FirstOrDefault().Transgender // Summing only the first record of each group
                         })
                         .Sum(result => result.TotalTransgenderVoters),
                        TotalVoters = _context.PollDetails.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.SlotManagementId == boothReportModel.SlotMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId)
                       .GroupBy(d => d.BoothMasterId)
                       .Select(group => new
                       {
                           BoothMasterId = group.Key,
                           TotalVotesPolled = group.OrderByDescending(d => d.VotesPolledRecivedTime).FirstOrDefault().VotesPolled // Summing only the first record of each group
                       })
                       .Sum(result => result.TotalVotesPolled)
                    };


                    consolidateBoothReports.Add(report);
                }
                // add group by state districts
                var districtGroupedReports = consolidateBoothReports
         .GroupBy(report => report.DistrictName)
         .Select(group => new VTReportModel
         {
             Header = $"{state.StateName}({state.StateCode})",
             Title = $"{state.StateName}",
             Type = "State",
             DistrictName = group.Key,
             MaleElectoral = group.Sum(report => report.MaleElectoral),
             FemaleElectoral = group.Sum(report => report.FemaleElectoral),
             ThirdGenderElectoral = group.Sum(report => report.ThirdGenderElectoral),
             TotalElectoral = group.Sum(report => report.TotalElectoral),
             MaleVoters = group.Sum(report => report.MaleVoters),
             FemaleVoters = group.Sum(report => report.FemaleVoters),
             ThirdGenderVoters = group.Sum(report => report.ThirdGenderVoters),
             TotalVoters = group.Sum(report => report.TotalVoters)
         })
         .ToList();

                // Now districtGroupedReports contains the grouped reports by district. You can add this to your main final list.
                FinalList.AddRange(districtGroupedReports);


                return districtGroupedReports;
            }
            //District
            else if (boothReportModel.StateMasterId is not 0 && boothReportModel.DistrictMasterId is not 0 && boothReportModel.AssemblyMasterId is 0 && boothReportModel.PCMasterId is 0)
            {
                var assemblyList = _context.AssemblyMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId && d.AssemblyStatus == true).Include(d => d.BoothMaster).ToList();
                foreach (var assembly in assemblyList)
                {


                    VTReportModel report = new VTReportModel
                    {

                        Header = $"{state.StateName}({state.StateCode}),{district.DistrictName}({district.DistrictCode})",
                        Title = "District Slot Report" + $"({slotMaster.StartTime.ToString()} ) , ( {slotMaster.EndTime.ToString()} ) , ({slotMaster.LockTime.ToString()})",
                        Type = "District",

                        DistrictName = _context.DistrictMaster.Where(d => d.DistrictMasterId == assembly.DistrictMasterId && d.DistrictStatus == true).Select(d => d.DistrictName).FirstOrDefault(),
                        AssemblyName = assembly.AssemblyName,
                        AssemblyCode = assembly.AssemblyCode.ToString(),
                        MaleElectoral = assembly.BoothMaster.Select(d => d.Male).Sum(),
                        FemaleElectoral = assembly.BoothMaster.Select(d => d.Female).Sum(),
                        ThirdGenderElectoral = assembly.BoothMaster.Select(d => d.Transgender).Sum(),
                        TotalElectoral = assembly.BoothMaster.Select(d => d.TotalVoters).Sum(),

                        MaleVoters = _context.PollDetails.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.SlotManagementId == boothReportModel.SlotMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId)
                         .GroupBy(d => d.BoothMasterId)
                         .Select(group => new
                         {
                             BoothMasterId = group.Key,
                             TotalMaleVoters = group.OrderByDescending(d => d.VotesPolledRecivedTime).FirstOrDefault().Male // Summing only the first record of each group
                         })
                         .Sum(result => result.TotalMaleVoters),
                        FemaleVoters = _context.PollDetails.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.SlotManagementId == boothReportModel.SlotMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId)
                         .GroupBy(d => d.BoothMasterId)
                         .Select(group => new
                         {
                             BoothMasterId = group.Key,
                             TotalFemaleVoters = group.OrderByDescending(d => d.VotesPolledRecivedTime).FirstOrDefault().Female // Summing only the first record of each group
                         })
                         .Sum(result => result.TotalFemaleVoters),
                        ThirdGenderVoters = _context.PollDetails.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.SlotManagementId == boothReportModel.SlotMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId)
                         .GroupBy(d => d.BoothMasterId)
                         .Select(group => new
                         {
                             BoothMasterId = group.Key,
                             TotalTransgenderVoters = group.OrderByDescending(d => d.VotesPolledRecivedTime).FirstOrDefault().Transgender // Summing only the first record of each group
                         })
                         .Sum(result => result.TotalTransgenderVoters),
                        TotalVoters = _context.PollDetails.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.SlotManagementId == boothReportModel.SlotMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId)
                       .GroupBy(d => d.BoothMasterId)
                       .Select(group => new
                       {
                           BoothMasterId = group.Key,
                           TotalVotesPolled = group.OrderByDescending(d => d.VotesPolledRecivedTime).FirstOrDefault().VotesPolled // Summing only the first record of each group
                       })
                       .Sum(result => result.TotalVotesPolled)
                    };
                    consolidateBoothReports.Add(report);
                }
                return consolidateBoothReports;
            }
            //PC
            else if (boothReportModel.StateMasterId is not 0 && boothReportModel.DistrictMasterId is 0 && boothReportModel.PCMasterId is not 0 && boothReportModel.AssemblyMasterId is 0)
            {
                var assemblyList = _context.AssemblyMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.PCMasterId == boothReportModel.PCMasterId && d.AssemblyStatus == true).Include(d => d.BoothMaster).ToList();
                foreach (var assembly in assemblyList)
                {
                    VTReportModel report = new VTReportModel
                    {
                        // Populate your ConsolidateBoothReport properties here based on assembly data
                        Header = $"{state.StateName}({state.StateCode}),{pcMaster.PcName}({pcMaster.PcCodeNo})",
                        Title = $"{pcMaster.PcName}",
                        Type = "State - Slot Report" + $"{slotMaster.StartTime.ToString()},({slotMaster.EndTime}),{slotMaster.LockTime})",

                        AssemblyName = assembly.AssemblyName,
                        AssemblyCode = assembly.AssemblyCode.ToString(),
                        MaleElectoral = assembly.BoothMaster.Select(d => d.Male).Sum(),
                        FemaleElectoral = assembly.BoothMaster.Select(d => d.Female).Sum(),
                        ThirdGenderElectoral = assembly.BoothMaster.Select(d => d.Transgender).Sum(),
                        TotalElectoral = assembly.BoothMaster.Select(d => d.TotalVoters).Sum(),

                        MaleVoters = _context.PollDetails.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.SlotManagementId == boothReportModel.SlotMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId && d.PCMasterId == boothReportModel.PCMasterId)
                         .GroupBy(d => d.BoothMasterId)
                         .Select(group => new
                         {
                             BoothMasterId = group.Key,
                             TotalMaleVoters = group.OrderByDescending(d => d.VotesPolledRecivedTime).FirstOrDefault().Male // Summing only the first record of each group
                         })
                         .Sum(result => result.TotalMaleVoters),
                        FemaleVoters = _context.PollDetails.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.SlotManagementId == boothReportModel.SlotMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId && d.PCMasterId == boothReportModel.PCMasterId)
                         .GroupBy(d => d.BoothMasterId)
                         .Select(group => new
                         {
                             BoothMasterId = group.Key,
                             TotalFemaleVoters = group.OrderByDescending(d => d.VotesPolledRecivedTime).FirstOrDefault().Female // Summing only the first record of each group
                         })
                         .Sum(result => result.TotalFemaleVoters),
                        ThirdGenderVoters = _context.PollDetails.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.SlotManagementId == boothReportModel.SlotMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId && d.PCMasterId == boothReportModel.PCMasterId)
                         .GroupBy(d => d.BoothMasterId)
                         .Select(group => new
                         {
                             BoothMasterId = group.Key,
                             TotalTransgenderVoters = group.OrderByDescending(d => d.VotesPolledRecivedTime).FirstOrDefault().Transgender // Summing only the first record of each group
                         })
                         .Sum(result => result.TotalTransgenderVoters),
                        TotalVoters = _context.PollDetails.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.SlotManagementId == boothReportModel.SlotMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId && d.PCMasterId == boothReportModel.PCMasterId)
                       .GroupBy(d => d.BoothMasterId)
                       .Select(group => new
                       {
                           BoothMasterId = group.Key,
                           TotalVotesPolled = group.OrderByDescending(d => d.VotesPolledRecivedTime).FirstOrDefault().VotesPolled // Summing only the first record of each group
                       })
                       .Sum(result => result.TotalVotesPolled)

                    };
                    consolidateBoothReports.Add(report);
                }
                return consolidateBoothReports;
            }
            //Assembly
            else if (boothReportModel.AssemblyMasterId is not 0)
            {
                if (boothReportModel.DistrictMasterId is not 0)
                {
                    var assemblyRecords = _context.AssemblyMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId && d.AssemblyMasterId == boothReportModel.AssemblyMasterId && d.AssemblyStatus == true).Include(d => d.BoothMaster).ToList();
                    foreach (var assembly in assemblyRecords)
                    {
                        VTReportModel report = new VTReportModel
                        {

                            Header = $"{state.StateName}({state.StateCode}),{district.DistrictName}({district.DistrictCode}),{assembly.AssemblyName}({assembly.AssemblyCode})",
                            //Title = $"{assembly.AssemblyName}",
                            Type = "Assembly",
                            Title = "State Slot Report" + $"({slotMaster.StartTime.ToString()} ) , ( {slotMaster.EndTime.ToString()} ) , ({slotMaster.LockTime.ToString()})",
                            AssemblyName = assembly.AssemblyName,
                            AssemblyCode = assembly.AssemblyCode.ToString(),
                            MaleElectoral = assembly.BoothMaster.Select(d => d.Male).Sum(),
                            FemaleElectoral = assembly.BoothMaster.Select(d => d.Female).Sum(),
                            ThirdGenderElectoral = assembly.BoothMaster.Select(d => d.Transgender).Sum(),
                            TotalElectoral = assembly.BoothMaster.Select(d => d.TotalVoters).Sum(),

                            MaleVoters = _context.PollDetails.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.SlotManagementId == boothReportModel.SlotMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId)
                         .GroupBy(d => d.BoothMasterId)
                         .Select(group => new
                         {
                             BoothMasterId = group.Key,
                             TotalMaleVoters = group.OrderByDescending(d => d.VotesPolledRecivedTime).FirstOrDefault().Male // Summing only the first record of each group
                         })
                         .Sum(result => result.TotalMaleVoters),
                            FemaleVoters = _context.PollDetails.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.SlotManagementId == boothReportModel.SlotMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId)
                         .GroupBy(d => d.BoothMasterId)
                         .Select(group => new
                         {
                             BoothMasterId = group.Key,
                             TotalFemaleVoters = group.OrderByDescending(d => d.VotesPolledRecivedTime).FirstOrDefault().Female // Summing only the first record of each group
                         })
                         .Sum(result => result.TotalFemaleVoters),
                            ThirdGenderVoters = _context.PollDetails.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.SlotManagementId == boothReportModel.SlotMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId)
                         .GroupBy(d => d.BoothMasterId)
                         .Select(group => new
                         {
                             BoothMasterId = group.Key,
                             TotalTransgenderVoters = group.OrderByDescending(d => d.VotesPolledRecivedTime).FirstOrDefault().Transgender // Summing only the first record of each group
                         })
                         .Sum(result => result.TotalTransgenderVoters),
                            TotalVoters = _context.PollDetails.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.SlotManagementId == boothReportModel.SlotMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId)
                       .GroupBy(d => d.BoothMasterId)
                       .Select(group => new
                       {
                           BoothMasterId = group.Key,
                           TotalVotesPolled = group.OrderByDescending(d => d.VotesPolledRecivedTime).FirstOrDefault().VotesPolled // Summing only the first record of each group
                       })
                       .Sum(result => result.TotalVotesPolled)

                        };
                        consolidateBoothReports.Add(report);
                    }
                    return consolidateBoothReports;
                }
                else
                {
                    var assembly = _context.AssemblyMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.AssemblyMasterId == boothReportModel.AssemblyMasterId && d.AssemblyStatus == true).Include(d => d.BoothMaster).FirstOrDefault();
                    foreach (var booth in assembly.BoothMaster)
                    {
                        VTReportModel report = new VTReportModel
                        {
                            // Populate your ConsolidateBoothReport properties here based on assembly data
                            Header = $"{state.StateName}({state.StateCode}),{assembly.AssemblyName}({assembly.AssemblyCode})",
                            //Title = $"{assembly.AssemblyName}",
                            Title = "State Slot Report" + $"({slotMaster.StartTime.ToString()} ) , ( {slotMaster.EndTime.ToString()} ) , ({slotMaster.LockTime.ToString()})",
                            Type = "Assembly",

                            AssemblyName = assembly.AssemblyName,
                            AssemblyCode = assembly.AssemblyCode.ToString(),
                            BoothName = booth.BoothName,
                            BoothCode = booth.BoothCode_No.ToString(),
                            MaleElectoral = assembly.BoothMaster.Select(d => d.Male).Sum(),
                            FemaleElectoral = assembly.BoothMaster.Select(d => d.Female).Sum(),
                            ThirdGenderElectoral = assembly.BoothMaster.Select(d => d.Transgender).Sum(),
                            TotalElectoral = assembly.BoothMaster.Select(d => d.TotalVoters).Sum(),



                            MaleVoters = _context.PollDetails.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.SlotManagementId == boothReportModel.SlotMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId && d.BoothMasterId == booth.BoothMasterId).Sum(d => d.Male),
                            FemaleVoters = _context.PollDetails.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.SlotManagementId == boothReportModel.SlotMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId && d.BoothMasterId == booth.BoothMasterId).Sum(d => d.Female),
                            ThirdGenderVoters = _context.PollDetails.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.SlotManagementId == boothReportModel.SlotMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId && d.BoothMasterId == booth.BoothMasterId).Sum(d => d.Transgender),
                            TotalVoters = _context.PollDetails.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.SlotManagementId == boothReportModel.SlotMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId && d.BoothMasterId == booth.BoothMasterId).OrderByDescending(d => d.VotesPolledRecivedTime).Select(p => p.VotesPolled).FirstOrDefault()
                        };
                        consolidateBoothReports.Add(report);

                    }
                    return consolidateBoothReports;
                }


            }


            return consolidateBoothReports;
        }




        #endregion


        #region GetChartConsolidatedReport
        public async Task<List<ChartConsolidatedReport>> GetChartConsolidatedReport(ChartReportModel boothReportModel)
        {
            List<ChartConsolidatedReport> consolidateBoothReports = new List<ChartConsolidatedReport>();
            var state = _context.StateMaster
                .Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.StateStatus == true)
                .Select(d => new { d.StateName, d.StateCode })
                .FirstOrDefault();

            var eventRecord = _context.EventMaster
                .Where(d => d.EventMasterId == boothReportModel.EventMasterId)
                .FirstOrDefault();

            // State
            if (boothReportModel.StateMasterId is not 0 && boothReportModel.DistrictMasterId is 0 && boothReportModel.AssemblyMasterId is 0 && boothReportModel.PCMasterId is 0)
            {
                var allDistricts = await _context.DistrictMaster
                    .Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictStatus == true)
                    .ToListAsync();

                ChartConsolidatedReport report = new ChartConsolidatedReport
                {
                    Name = state.StateName,
                    Type = "State",
                    Heading = "District Wise",
                    Event = eventRecord.EventName,
                    EventData = new List<object>() // Initialize EventData list

                };

                foreach (var dis in allDistricts)
                {
                    // ...

                    Dictionary<int, int> electionInfoByDistrictId;

                    switch (boothReportModel.EventMasterId)
                    {
                        case 1:
                            electionInfoByDistrictId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.IsPartyDispatched == true && e.DistrictMasterId == dis.DistrictMasterId)
                                .GroupBy(e => e.DistrictMasterId)
                                .Select(g => new
                                {
                                    DistrictMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsPartyDispatched == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.DistrictMasterId, g => g.EventData); // Specify the types explicitly here
                            break;

                        case 2:
                            electionInfoByDistrictId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.IsPartyReached == true && e.DistrictMasterId == dis.DistrictMasterId)
                                .GroupBy(e => e.DistrictMasterId)
                                .Select(g => new
                                {
                                    DistrictMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsPartyReached == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.DistrictMasterId, g => g.EventData); // Specify the types explicitly here
                            break;
                        case 3:
                            electionInfoByDistrictId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.IsSetupOfPolling == true && e.DistrictMasterId == dis.DistrictMasterId)
                                .GroupBy(e => e.DistrictMasterId)
                                .Select(g => new
                                {
                                    DistrictMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsSetupOfPolling == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.DistrictMasterId, g => g.EventData); // Specify the types explicitly here
                            break;
                        case 4:
                            electionInfoByDistrictId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.IsMockPollDone == true && e.DistrictMasterId == dis.DistrictMasterId)
                                .GroupBy(e => e.DistrictMasterId)
                                .Select(g => new
                                {
                                    DistrictMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsMockPollDone == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.DistrictMasterId, g => g.EventData); // Specify the types explicitly here
                            break;
                        case 5:
                            electionInfoByDistrictId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.IsPollStarted == true && e.DistrictMasterId == dis.DistrictMasterId)
                                .GroupBy(e => e.DistrictMasterId)
                                .Select(g => new
                                {
                                    DistrictMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsPollStarted == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.DistrictMasterId, g => g.EventData); // Specify the types explicitly here
                            break;
                        case 6:
                            electionInfoByDistrictId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.IsVoterTurnOut == true && e.DistrictMasterId == dis.DistrictMasterId)
                                .GroupBy(e => e.DistrictMasterId)
                                .Select(g => new
                                {
                                    DistrictMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsVoterTurnOut == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.DistrictMasterId, g => g.EventData); // Specify the types explicitly here
                            break;

                        case 7:
                            electionInfoByDistrictId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.VoterInQueue != null && e.DistrictMasterId == dis.DistrictMasterId)
                                .GroupBy(e => e.DistrictMasterId)
                                .Select(g => new
                                {
                                    DistrictMasterId = g.Key,
                                    EventData = g.Sum(e => e.VoterInQueue) ?? 0, // Assuming VoterInQueue is a numeric type
                                })
                                .ToDictionaryAsync(g => g.DistrictMasterId, g => g.EventData);
                            break;


                        case 8:
                            electionInfoByDistrictId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.FinalTVoteStatus == true && e.DistrictMasterId == dis.DistrictMasterId)
                                .GroupBy(e => e.DistrictMasterId)
                                .Select(g => new
                                {
                                    DistrictMasterId = g.Key,
                                    EventData = g.Sum(e => e.FinalTVoteStatus == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.DistrictMasterId, g => g.EventData); // Specify the types explicitly here
                            break;
                        case 9:
                            electionInfoByDistrictId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.IsPollEnded == true && e.DistrictMasterId == dis.DistrictMasterId)
                                .GroupBy(e => e.DistrictMasterId)
                                .Select(g => new
                                {
                                    DistrictMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsPollEnded == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.DistrictMasterId, g => g.EventData); // Specify the types explicitly here
                            break;
                        case 10:
                            electionInfoByDistrictId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.IsMCESwitchOff == true && e.DistrictMasterId == dis.DistrictMasterId)
                                .GroupBy(e => e.DistrictMasterId)
                                .Select(g => new
                                {
                                    DistrictMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsMCESwitchOff == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.DistrictMasterId, g => g.EventData); // Specify the types explicitly here
                            break;

                        case 11:
                            electionInfoByDistrictId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.IsPartyDeparted == true && e.DistrictMasterId == dis.DistrictMasterId)
                                .GroupBy(e => e.DistrictMasterId)
                                .Select(g => new
                                {
                                    DistrictMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsPartyDeparted == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.DistrictMasterId, g => g.EventData); // Specify the types explicitly here
                            break;
                        case 12:
                            electionInfoByDistrictId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.IsPartyReachedCollectionCenter == true && e.DistrictMasterId == dis.DistrictMasterId)
                                .GroupBy(e => e.DistrictMasterId)
                                .Select(g => new
                                {
                                    DistrictMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsPartyReachedCollectionCenter == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.DistrictMasterId, g => g.EventData); // Specify the types explicitly here
                            break;
                        case 13:
                            electionInfoByDistrictId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.IsEVMDeposited == true && e.DistrictMasterId == dis.DistrictMasterId)
                                .GroupBy(e => e.DistrictMasterId)
                                .Select(g => new
                                {
                                    DistrictMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsEVMDeposited == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.DistrictMasterId, g => g.EventData); // Specify the types explicitly here
                            break;


                        // Add more cases as needed

                        default:
                            electionInfoByDistrictId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.IsPartyDispatched == true && e.DistrictMasterId == dis.DistrictMasterId)
                                .GroupBy(e => e.DistrictMasterId)
                                .Select(g => new
                                {
                                    DistrictMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsPartyDispatched == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.DistrictMasterId, g => g.EventData); // Specify the types explicitly here
                            break;
                    }

                    // get total booths
                    var getTotalBooths = await _context.BoothMaster.Where(p => p.StateMasterId == boothReportModel.StateMasterId && p.DistrictMasterId == dis.DistrictMasterId && p.BoothStatus == true).CountAsync();
                    if (electionInfoByDistrictId.TryGetValue(dis.DistrictMasterId, out var eventData))
                    {
                        //create %age
                        decimal getPercentage = Math.Round((decimal)(eventData * 100.0 / getTotalBooths), 1);
                        report.EventData.Add(new List<object> { dis.DistrictName, eventData, getTotalBooths, getPercentage });


                    }
                    else
                    {
                        report.EventData.Add(new List<object> { dis.DistrictName, 0, getTotalBooths, 0 });

                    }

                }

                consolidateBoothReports.Add(report);
                return consolidateBoothReports;
            }
            //PC
            if (boothReportModel.StateMasterId is not 0 && boothReportModel.DistrictMasterId is 0 && boothReportModel.AssemblyMasterId is 0 && boothReportModel.PCMasterId is not 0)
            {
                var allAssemblies = await _context.AssemblyMaster
                    .Where(d => d.PCMasterId == boothReportModel.PCMasterId && d.AssemblyStatus == true)
                    .ToListAsync();

                var pc = _context.ParliamentConstituencyMaster.FirstOrDefault(x => x.PCMasterId == boothReportModel.PCMasterId);

                ChartConsolidatedReport report = new ChartConsolidatedReport
                {
                    Name = pc.PcName,
                    Type = "Parliament Constituency",
                    Heading = "Asembly Wise",
                    Event = eventRecord.EventName,
                    EventData = [] // Initialize EventData list
                };

                foreach (var assembly in allAssemblies)
                {

                    Dictionary<int, int> electionInfoByPCId;

                    switch (boothReportModel.EventMasterId)
                    {
                        case 1:
                            electionInfoByPCId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.AssemblyMasterId == assembly.AssemblyMasterId && e.IsPartyDispatched == true && e.PCMasterId == assembly.PCMasterId)
                                .GroupBy(e => e.AssemblyMasterId)
                                .Select(g => new
                                {
                                    AssemblyMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsPartyDispatched == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.AssemblyMasterId, g => g.EventData); // Specify the types explicitly here
                            break;

                        case 2:
                            electionInfoByPCId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.AssemblyMasterId == assembly.AssemblyMasterId && e.IsPartyReached == true && e.PCMasterId == assembly.PCMasterId)
                                .GroupBy(e => e.AssemblyMasterId)
                                .Select(g => new
                                {
                                    AssemblyMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsPartyReached == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.AssemblyMasterId, g => g.EventData); // Specify the types explicitly here
                            break;
                        case 3:
                            electionInfoByPCId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.AssemblyMasterId == assembly.AssemblyMasterId && e.IsSetupOfPolling == true && e.PCMasterId == assembly.PCMasterId)
                                .GroupBy(e => e.AssemblyMasterId)
                                .Select(g => new
                                {
                                    AssemblyMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsSetupOfPolling == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.AssemblyMasterId, g => g.EventData); // Specify the types explicitly here
                            break;
                        case 4:
                            electionInfoByPCId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.AssemblyMasterId == assembly.AssemblyMasterId && e.IsMockPollDone == true && e.PCMasterId == assembly.PCMasterId)
                                .GroupBy(e => e.AssemblyMasterId)
                                .Select(g => new
                                {
                                    AssemblyMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsMockPollDone == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.AssemblyMasterId, g => g.EventData); // Specify the types explicitly here
                            break;
                        case 5:
                            electionInfoByPCId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.AssemblyMasterId == assembly.AssemblyMasterId && e.IsPollStarted == true && e.PCMasterId == assembly.PCMasterId)
                                .GroupBy(e => e.AssemblyMasterId)
                                .Select(g => new
                                {
                                    AssemblyMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsPollStarted == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.AssemblyMasterId, g => g.EventData); // Specify the types explicitly here
                            break;
                        case 6:
                            electionInfoByPCId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.AssemblyMasterId == assembly.AssemblyMasterId && e.IsVoterTurnOut == true && e.PCMasterId == assembly.PCMasterId)
                                .GroupBy(e => e.AssemblyMasterId)
                                .Select(g => new
                                {
                                    AssemblyMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsVoterTurnOut == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.AssemblyMasterId, g => g.EventData); // Specify the types explicitly here
                            break;

                        case 7:
                            electionInfoByPCId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.AssemblyMasterId == assembly.AssemblyMasterId && e.VoterInQueue != null && e.PCMasterId == assembly.PCMasterId)
                                .GroupBy(e => e.AssemblyMasterId)
                                .Select(g => new
                                {
                                    AssemblyMasterId = g.Key,
                                    EventData = g.Sum(e => e.VoterInQueue) ?? 0, // Assuming VoterInQueue is a numeric type
                                })
                                .ToDictionaryAsync(g => g.AssemblyMasterId, g => g.EventData);
                            break;


                        case 8:
                            electionInfoByPCId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.AssemblyMasterId == assembly.AssemblyMasterId && e.FinalTVoteStatus == true && e.PCMasterId == assembly.PCMasterId)
                                .GroupBy(e => e.AssemblyMasterId)
                                .Select(g => new
                                {
                                    AssemblyMasterId = g.Key,
                                    EventData = g.Sum(e => e.FinalTVoteStatus == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.AssemblyMasterId, g => g.EventData); // Specify the types explicitly here
                            break;
                        case 9:
                            electionInfoByPCId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.AssemblyMasterId == assembly.AssemblyMasterId && e.IsPollEnded == true && e.PCMasterId == assembly.PCMasterId)
                                .GroupBy(e => e.AssemblyMasterId)
                                .Select(g => new
                                {
                                    AssemblyMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsPollEnded == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.AssemblyMasterId, g => g.EventData); // Specify the types explicitly here
                            break;
                        case 10:
                            electionInfoByPCId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.AssemblyMasterId == assembly.AssemblyMasterId && e.IsMCESwitchOff == true && e.PCMasterId == assembly.PCMasterId)
                                .GroupBy(e => e.AssemblyMasterId)
                                .Select(g => new
                                {
                                    AssemblyMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsMCESwitchOff == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.AssemblyMasterId, g => g.EventData); // Specify the types explicitly here
                            break;

                        case 11:
                            electionInfoByPCId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.AssemblyMasterId == assembly.AssemblyMasterId && e.IsPartyDeparted == true && e.PCMasterId == assembly.PCMasterId)
                                .GroupBy(e => e.AssemblyMasterId)
                                .Select(g => new
                                {
                                    AssemblyMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsPartyDeparted == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.AssemblyMasterId, g => g.EventData); // Specify the types explicitly here
                            break;
                        case 12:
                            electionInfoByPCId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.AssemblyMasterId == assembly.AssemblyMasterId && e.IsPartyReachedCollectionCenter == true && e.PCMasterId == assembly.PCMasterId)
                                .GroupBy(e => e.AssemblyMasterId)
                                .Select(g => new
                                {
                                    AssemblyMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsPartyReachedCollectionCenter == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.AssemblyMasterId, g => g.EventData); // Specify the types explicitly here
                            break;
                        case 13:
                            electionInfoByPCId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.AssemblyMasterId == assembly.AssemblyMasterId && e.IsEVMDeposited == true && e.PCMasterId == assembly.PCMasterId)
                                .GroupBy(e => e.AssemblyMasterId)
                                .Select(g => new
                                {
                                    AssemblyMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsEVMDeposited == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.AssemblyMasterId, g => g.EventData); // Specify the types explicitly here
                            break;


                        // Add more cases as needed

                        default:
                            electionInfoByPCId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.AssemblyMasterId == assembly.AssemblyMasterId && e.IsPartyDispatched == true && e.PCMasterId == assembly.PCMasterId)
                                .GroupBy(e => e.AssemblyMasterId)
                                .Select(g => new
                                {
                                    AssemblyMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsPartyDispatched == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.AssemblyMasterId, g => g.EventData); // Specify the types explicitly here
                            break;
                    }

                    //foreach (var ele in electionInfoByPCId)
                    //{
                    //    electionInfoByPCId.TryGetValue(ele.Key, out var eventData);
                    //    report.EventData.Add(new List<object> { allAssemblies.FirstOrDefault(x => x.AssemblyMasterId == ele.Key)?.AssemblyName ?? "", eventData });
                    //}

                    /*  if (electionInfoByPCId.TryGetValue(assembly.AssemblyMasterId, out var eventData))
                      {
                          report.EventData.Add(new List<object> { assembly.AssemblyName, eventData });


                      }
                      else
                      {
                          report.EventData.Add(new List<object> { assembly.AssemblyName, 0 });

                      }*/
                    // get total booths
                    var getTotalBooths = await _context.BoothMaster.Where(p => p.StateMasterId == boothReportModel.StateMasterId && p.BoothStatus == true && p.AssemblyMasterId == assembly.AssemblyMasterId).CountAsync();
                    if (electionInfoByPCId.TryGetValue(assembly.AssemblyMasterId, out var eventData))
                    {
                        //create %age
                        decimal getPercentage = Math.Round((decimal)(eventData * 100.0 / getTotalBooths), 1);
                        report.EventData.Add(new List<object> { assembly.AssemblyName, eventData, getTotalBooths, getPercentage });


                    }
                    else
                    {
                        report.EventData.Add(new List<object> { assembly.AssemblyName, 0, getTotalBooths, 0 });

                    }
                }



                consolidateBoothReports.Add(report);
                return consolidateBoothReports;
            }
            //District
            if (boothReportModel.StateMasterId is not 0 && boothReportModel.DistrictMasterId is not 0 && boothReportModel.AssemblyMasterId is 0 && boothReportModel.PCMasterId is 0)
            {
                var allAssemblies = await _context.AssemblyMaster
                    .Where(d => d.DistrictMasterId == boothReportModel.DistrictMasterId && d.AssemblyStatus == true)
                    .ToListAsync();

                var dist = _context.DistrictMaster.FirstOrDefault(x => x.DistrictMasterId == boothReportModel.DistrictMasterId);

                ChartConsolidatedReport report = new ChartConsolidatedReport
                {
                    Name = dist.DistrictName,
                    Type = "District",
                    Heading = "Assembly Wise",
                    Event = eventRecord.EventName,
                    EventData = [] // Initialize EventData list
                };

                foreach (var assembly in allAssemblies)
                {

                    Dictionary<int, int> electionInfoByPCId;

                    switch (boothReportModel.EventMasterId)
                    {
                        case 1:
                            electionInfoByPCId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.AssemblyMasterId == assembly.AssemblyMasterId && e.IsPartyDispatched == true)
                                .GroupBy(e => e.AssemblyMasterId)
                                .Select(g => new
                                {
                                    AssemblyMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsPartyDispatched == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.AssemblyMasterId, g => g.EventData); // Specify the types explicitly here
                            break;

                        case 2:
                            electionInfoByPCId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && assembly.AssemblyMasterId == e.AssemblyMasterId && e.IsPartyReached == true)
                                .GroupBy(e => e.AssemblyMasterId)
                                .Select(g => new
                                {
                                    AssemblyMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsPartyReached == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.AssemblyMasterId, g => g.EventData); // Specify the types explicitly here
                            break;
                        case 3:
                            electionInfoByPCId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && assembly.AssemblyMasterId == e.AssemblyMasterId && e.IsSetupOfPolling == true)
                                .GroupBy(e => e.AssemblyMasterId)
                                .Select(g => new
                                {
                                    AssemblyMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsSetupOfPolling == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.AssemblyMasterId, g => g.EventData); // Specify the types explicitly here
                            break;
                        case 4:
                            electionInfoByPCId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && assembly.AssemblyMasterId == e.AssemblyMasterId && e.IsMockPollDone == true)
                                .GroupBy(e => e.AssemblyMasterId)
                                .Select(g => new
                                {
                                    AssemblyMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsMockPollDone == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.AssemblyMasterId, g => g.EventData); // Specify the types explicitly here
                            break;
                        case 5:
                            electionInfoByPCId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && assembly.AssemblyMasterId == e.AssemblyMasterId && e.IsPollStarted == true)
                                .GroupBy(e => e.AssemblyMasterId)
                                .Select(g => new
                                {
                                    AssemblyMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsPollStarted == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.AssemblyMasterId, g => g.EventData); // Specify the types explicitly here
                            break;
                        case 6:
                            electionInfoByPCId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && assembly.AssemblyMasterId == e.AssemblyMasterId && e.IsVoterTurnOut == true)
                                .GroupBy(e => e.AssemblyMasterId)
                                .Select(g => new
                                {
                                    AssemblyMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsVoterTurnOut == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.AssemblyMasterId, g => g.EventData); // Specify the types explicitly here
                            break;

                        case 7:
                            electionInfoByPCId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && assembly.AssemblyMasterId == e.AssemblyMasterId && e.VoterInQueue != null)
                                .GroupBy(e => e.AssemblyMasterId)
                                .Select(g => new
                                {
                                    AssemblyMasterId = g.Key,
                                    EventData = g.Sum(e => e.VoterInQueue) ?? 0, // Assuming VoterInQueue is a numeric type
                                })
                                .ToDictionaryAsync(g => g.AssemblyMasterId, g => g.EventData);
                            break;


                        case 8:
                            electionInfoByPCId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && assembly.AssemblyMasterId == e.AssemblyMasterId && e.FinalTVoteStatus == true)
                                .GroupBy(e => e.AssemblyMasterId)
                                .Select(g => new
                                {
                                    AssemblyMasterId = g.Key,
                                    EventData = g.Sum(e => e.FinalTVoteStatus == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.AssemblyMasterId, g => g.EventData); // Specify the types explicitly here
                            break;
                        case 9:
                            electionInfoByPCId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && assembly.AssemblyMasterId == e.AssemblyMasterId && e.IsPollEnded == true)
                                .GroupBy(e => e.AssemblyMasterId)
                                .Select(g => new
                                {
                                    AssemblyMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsPollEnded == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.AssemblyMasterId, g => g.EventData); // Specify the types explicitly here
                            break;
                        case 10:
                            electionInfoByPCId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && assembly.AssemblyMasterId == e.AssemblyMasterId && e.IsMCESwitchOff == true)
                                .GroupBy(e => e.AssemblyMasterId)
                                .Select(g => new
                                {
                                    AssemblyMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsMCESwitchOff == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.AssemblyMasterId, g => g.EventData); // Specify the types explicitly here
                            break;

                        case 11:
                            electionInfoByPCId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && assembly.AssemblyMasterId == e.AssemblyMasterId && e.IsPartyDeparted == true)
                                .GroupBy(e => e.AssemblyMasterId)
                                .Select(g => new
                                {
                                    AssemblyMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsPartyDeparted == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.AssemblyMasterId, g => g.EventData); // Specify the types explicitly here
                            break;
                        case 12:
                            electionInfoByPCId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && assembly.AssemblyMasterId == e.AssemblyMasterId && e.IsPartyReachedCollectionCenter == true)
                                .GroupBy(e => e.AssemblyMasterId)
                                .Select(g => new
                                {
                                    AssemblyMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsPartyReachedCollectionCenter == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.AssemblyMasterId, g => g.EventData); // Specify the types explicitly here
                            break;
                        case 13:
                            electionInfoByPCId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && assembly.AssemblyMasterId == e.AssemblyMasterId && e.IsEVMDeposited == true)
                                .GroupBy(e => e.AssemblyMasterId)
                                .Select(g => new
                                {
                                    AssemblyMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsEVMDeposited == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.AssemblyMasterId, g => g.EventData); // Specify the types explicitly here
                            break;


                        // Add more cases as needed

                        default:
                            electionInfoByPCId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && assembly.AssemblyMasterId == e.AssemblyMasterId && e.IsPartyDispatched == true)
                                .GroupBy(e => e.DistrictMasterId)
                                .Select(g => new
                                {
                                    AssemblyMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsPartyDispatched == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.AssemblyMasterId, g => g.EventData); // Specify the types explicitly here
                            break;
                    }

                    //foreach (var ele in electionInfoByPCId)
                    //{
                    //    electionInfoByPCId.TryGetValue(ele.Key, out var eventData);
                    //    report.EventData.Add(new List<object> { allAssemblies.FirstOrDefault(x => x.AssemblyMasterId == ele.Key)?.AssemblyName ?? "", eventData });
                    //}

                    var getTotalBooths = await _context.BoothMaster.Where(p => p.StateMasterId == boothReportModel.StateMasterId && p.BoothStatus == true && p.DistrictMasterId == boothReportModel.DistrictMasterId && p.AssemblyMasterId == assembly.AssemblyMasterId).CountAsync();

                    if (electionInfoByPCId.TryGetValue(assembly.AssemblyMasterId, out var eventData))
                    {
                        decimal getPercentage = Math.Round((decimal)(eventData * 100.0 / getTotalBooths), 1);

                        report.EventData.Add(new List<object> { assembly.AssemblyName, eventData, getTotalBooths, getPercentage });


                    }
                    else
                    {
                        report.EventData.Add(new List<object> { assembly.AssemblyName, 0, getTotalBooths, 0 });

                    }

                }

                consolidateBoothReports.Add(report);
                return consolidateBoothReports;
            }

            //Assembly-District if district selected
            if (boothReportModel.StateMasterId is not 0 && boothReportModel.DistrictMasterId is not 0 && boothReportModel.AssemblyMasterId is not 0 && boothReportModel.PCMasterId is 0)
            {
                var allBooths = await _context.BoothMaster
                    .Where(d => d.AssemblyMasterId == boothReportModel.AssemblyMasterId && d.BoothStatus == true)
                    .ToListAsync();

                var assembly = _context.AssemblyMaster.FirstOrDefault(x => x.AssemblyMasterId == boothReportModel.AssemblyMasterId);

                ChartConsolidatedReport report = new ChartConsolidatedReport
                {
                    Name = assembly.AssemblyName,
                    Type = "Assembly",
                    Heading = "Booth Wise",
                    Event = eventRecord.EventName,
                    EventData = [] // Initialize EventData list
                };

                foreach (var booth in allBooths)
                {

                    Dictionary<int, int> electionInfoByPCId;

                    switch (boothReportModel.EventMasterId)
                    {
                        case 1:
                            electionInfoByPCId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.BoothMasterId == booth.BoothMasterId && e.IsPartyDispatched == true)
                                .GroupBy(e => e.BoothMasterId)
                                .Select(g => new
                                {
                                    BoothMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsPartyDispatched == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.BoothMasterId, g => g.EventData); // Specify the types explicitly here
                            break;

                        case 2:
                            electionInfoByPCId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.BoothMasterId == booth.BoothMasterId && e.IsPartyReached == true)
                                .GroupBy(e => e.BoothMasterId)
                                .Select(g => new
                                {
                                    BoothMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsPartyReached == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.BoothMasterId, g => g.EventData); // Specify the types explicitly here
                            break;
                        case 3:
                            electionInfoByPCId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.BoothMasterId == booth.BoothMasterId && e.IsSetupOfPolling == true)
                                .GroupBy(e => e.BoothMasterId)
                                .Select(g => new
                                {
                                    BoothMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsSetupOfPolling == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.BoothMasterId, g => g.EventData); // Specify the types explicitly here
                            break;
                        case 4:
                            electionInfoByPCId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.BoothMasterId == booth.BoothMasterId && e.IsMockPollDone == true)
                                .GroupBy(e => e.BoothMasterId)
                                .Select(g => new
                                {
                                    BoothMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsMockPollDone == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.BoothMasterId, g => g.EventData); // Specify the types explicitly here
                            break;
                        case 5:
                            electionInfoByPCId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.BoothMasterId == booth.BoothMasterId && e.IsPollStarted == true)
                                .GroupBy(e => e.BoothMasterId)
                                .Select(g => new
                                {
                                    BoothMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsPollStarted == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.BoothMasterId, g => g.EventData); // Specify the types explicitly here
                            break;
                        case 6:
                            electionInfoByPCId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.BoothMasterId == booth.BoothMasterId && e.IsVoterTurnOut == true)
                                .GroupBy(e => e.BoothMasterId)
                                .Select(g => new
                                {
                                    BoothMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsVoterTurnOut == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.BoothMasterId, g => g.EventData); // Specify the types explicitly here
                            break;

                        case 7:
                            electionInfoByPCId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.BoothMasterId == booth.BoothMasterId && e.VoterInQueue != null)
                                .GroupBy(e => e.BoothMasterId)
                                .Select(g => new
                                {
                                    BoothMasterId = g.Key,
                                    EventData = g.Sum(e => e.VoterInQueue) ?? 0, // Assuming VoterInQueue is a numeric type
                                })
                                .ToDictionaryAsync(g => g.BoothMasterId, g => g.EventData);
                            break;


                        case 8:
                            electionInfoByPCId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.BoothMasterId == booth.BoothMasterId && e.FinalTVoteStatus == true)
                                .GroupBy(e => e.BoothMasterId)
                                .Select(g => new
                                {
                                    BoothMasterId = g.Key,
                                    EventData = g.Sum(e => e.FinalTVoteStatus == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.BoothMasterId, g => g.EventData); // Specify the types explicitly here
                            break;
                        case 9:
                            electionInfoByPCId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.BoothMasterId == booth.BoothMasterId && e.IsPollEnded == true)
                                .GroupBy(e => e.BoothMasterId)
                                .Select(g => new
                                {
                                    BoothMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsPollEnded == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.BoothMasterId, g => g.EventData); // Specify the types explicitly here
                            break;
                        case 10:
                            electionInfoByPCId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.BoothMasterId == booth.BoothMasterId && e.IsMCESwitchOff == true)
                                .GroupBy(e => e.BoothMasterId)
                                .Select(g => new
                                {
                                    BoothMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsMCESwitchOff == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.BoothMasterId, g => g.EventData); // Specify the types explicitly here
                            break;

                        case 11:
                            electionInfoByPCId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.BoothMasterId == booth.BoothMasterId && e.IsPartyDeparted == true)
                                .GroupBy(e => e.BoothMasterId)
                                .Select(g => new
                                {
                                    BoothMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsPartyDeparted == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.BoothMasterId, g => g.EventData); // Specify the types explicitly here
                            break;
                        case 12:
                            electionInfoByPCId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.BoothMasterId == booth.BoothMasterId && e.IsPartyReachedCollectionCenter == true)
                                .GroupBy(e => e.BoothMasterId)
                                .Select(g => new
                                {
                                    BoothMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsPartyReachedCollectionCenter == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.BoothMasterId, g => g.EventData); // Specify the types explicitly here
                            break;
                        case 13:
                            electionInfoByPCId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.BoothMasterId == booth.BoothMasterId && e.IsEVMDeposited == true)
                                .GroupBy(e => e.BoothMasterId)
                                .Select(g => new
                                {
                                    BoothMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsEVMDeposited == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.BoothMasterId, g => g.EventData); // Specify the types explicitly here
                            break;


                        // Add more cases as needed

                        default:
                            electionInfoByPCId = await _context.ElectionInfoMaster
                                .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.BoothMasterId == booth.BoothMasterId && e.IsPartyDispatched == true)
                                .GroupBy(e => e.BoothMasterId)
                                .Select(g => new
                                {
                                    BoothMasterId = g.Key,
                                    EventData = g.Sum(e => e.IsPartyDispatched == true ? 1 : 0),
                                })
                                .ToDictionaryAsync(g => g.BoothMasterId, g => g.EventData); // Specify the types explicitly here
                            break;
                    }

                    //foreach (var ele in electionInfoByPCId)
                    //{
                    //    electionInfoByPCId.TryGetValue(ele.Key, out var eventData);
                    //    report.EventData.Add(new List<object> { allBooths.FirstOrDefault(x => x.BoothMasterId == ele.Key)?.BoothName ?? "", eventData });
                    //}
                    var getTotalBooths = await _context.BoothMaster.Where(p => p.StateMasterId == boothReportModel.StateMasterId && p.BoothStatus == true && p.BoothMasterId == booth.BoothMasterId).CountAsync();

                    if (electionInfoByPCId.TryGetValue(booth.BoothMasterId, out var eventData))
                    {

                        decimal getPercentage = Math.Round((decimal)(eventData * 100.0 / getTotalBooths), 1);

                        report.EventData.Add(new List<object> { booth.BoothName, eventData, getTotalBooths, getPercentage });


                    }
                    else
                    {
                        report.EventData.Add(new List<object> { booth.BoothName, 0, getTotalBooths, 0 });

                    }
                }

                consolidateBoothReports.Add(report);
                return consolidateBoothReports;
            }


            //Assemblyu - PC if PC selected fro Assembly
            if (boothReportModel.StateMasterId is not 0 && boothReportModel.DistrictMasterId is 0 && boothReportModel.AssemblyMasterId is not 0 && boothReportModel.PCMasterId is not 0)
            {
                var assembly = _context.AssemblyMaster.FirstOrDefault(x => x.AssemblyMasterId == boothReportModel.AssemblyMasterId && x.PCMasterId == boothReportModel.PCMasterId);

                if (assembly.AssemblyMasterId == boothReportModel.AssemblyMasterId)
                {
                    var allBooths = await _context.BoothMaster
                        .Where(d => d.AssemblyMasterId == boothReportModel.AssemblyMasterId && d.BoothStatus == true)
                        .ToListAsync();

                    ChartConsolidatedReport report = new ChartConsolidatedReport
                    {
                        Name = assembly.AssemblyName,
                        Type = "Assembly",
                        Heading = "Booth Wise",
                        Event = eventRecord.EventName,
                        EventData = [] // Initialize EventData list
                    };

                    foreach (var booth in allBooths)
                    {

                        Dictionary<int, int> electionInfoByPCId;

                        switch (boothReportModel.EventMasterId)
                        {
                            case 1:
                                electionInfoByPCId = await _context.ElectionInfoMaster
                                    .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.BoothMasterId == booth.BoothMasterId && e.IsPartyDispatched == true)
                                    .GroupBy(e => e.BoothMasterId)
                                    .Select(g => new
                                    {
                                        BoothMasterId = g.Key,
                                        EventData = g.Sum(e => e.IsPartyDispatched == true ? 1 : 0),
                                    })
                                    .ToDictionaryAsync(g => g.BoothMasterId, g => g.EventData); // Specify the types explicitly here
                                break;

                            case 2:
                                electionInfoByPCId = await _context.ElectionInfoMaster
                                    .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.BoothMasterId == booth.BoothMasterId && e.IsPartyReached == true)
                                    .GroupBy(e => e.BoothMasterId)
                                    .Select(g => new
                                    {
                                        BoothMasterId = g.Key,
                                        EventData = g.Sum(e => e.IsPartyReached == true ? 1 : 0),
                                    })
                                    .ToDictionaryAsync(g => g.BoothMasterId, g => g.EventData); // Specify the types explicitly here
                                break;
                            case 3:
                                electionInfoByPCId = await _context.ElectionInfoMaster
                                    .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.BoothMasterId == booth.BoothMasterId && e.IsSetupOfPolling == true)
                                    .GroupBy(e => e.BoothMasterId)
                                    .Select(g => new
                                    {
                                        BoothMasterId = g.Key,
                                        EventData = g.Sum(e => e.IsSetupOfPolling == true ? 1 : 0),
                                    })
                                    .ToDictionaryAsync(g => g.BoothMasterId, g => g.EventData); // Specify the types explicitly here
                                break;
                            case 4:
                                electionInfoByPCId = await _context.ElectionInfoMaster
                                    .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.BoothMasterId == booth.BoothMasterId && e.IsMockPollDone == true)
                                    .GroupBy(e => e.BoothMasterId)
                                    .Select(g => new
                                    {
                                        BoothMasterId = g.Key,
                                        EventData = g.Sum(e => e.IsMockPollDone == true ? 1 : 0),
                                    })
                                    .ToDictionaryAsync(g => g.BoothMasterId, g => g.EventData); // Specify the types explicitly here
                                break;
                            case 5:
                                electionInfoByPCId = await _context.ElectionInfoMaster
                                    .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.BoothMasterId == booth.BoothMasterId && e.IsPollStarted == true)
                                    .GroupBy(e => e.BoothMasterId)
                                    .Select(g => new
                                    {
                                        BoothMasterId = g.Key,
                                        EventData = g.Sum(e => e.IsPollStarted == true ? 1 : 0),
                                    })
                                    .ToDictionaryAsync(g => g.BoothMasterId, g => g.EventData); // Specify the types explicitly here
                                break;
                            case 6:
                                electionInfoByPCId = await _context.ElectionInfoMaster
                                    .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.BoothMasterId == booth.BoothMasterId && e.IsVoterTurnOut == true)
                                    .GroupBy(e => e.BoothMasterId)
                                    .Select(g => new
                                    {
                                        BoothMasterId = g.Key,
                                        EventData = g.Sum(e => e.IsVoterTurnOut == true ? 1 : 0),
                                    })
                                    .ToDictionaryAsync(g => g.BoothMasterId, g => g.EventData); // Specify the types explicitly here
                                break;

                            case 7:
                                electionInfoByPCId = await _context.ElectionInfoMaster
                                    .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.BoothMasterId == booth.BoothMasterId && e.VoterInQueue != null)
                                    .GroupBy(e => e.BoothMasterId)
                                    .Select(g => new
                                    {
                                        BoothMasterId = g.Key,
                                        EventData = g.Sum(e => e.VoterInQueue) ?? 0, // Assuming VoterInQueue is a numeric type
                                    })
                                    .ToDictionaryAsync(g => g.BoothMasterId, g => g.EventData);
                                break;


                            case 8:
                                electionInfoByPCId = await _context.ElectionInfoMaster
                                    .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.BoothMasterId == booth.BoothMasterId && e.FinalTVoteStatus == true)
                                    .GroupBy(e => e.BoothMasterId)
                                    .Select(g => new
                                    {
                                        BoothMasterId = g.Key,
                                        EventData = g.Sum(e => e.FinalTVoteStatus == true ? 1 : 0),
                                    })
                                    .ToDictionaryAsync(g => g.BoothMasterId, g => g.EventData); // Specify the types explicitly here
                                break;
                            case 9:
                                electionInfoByPCId = await _context.ElectionInfoMaster
                                    .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.BoothMasterId == booth.BoothMasterId && e.IsPollEnded == true)
                                    .GroupBy(e => e.BoothMasterId)
                                    .Select(g => new
                                    {
                                        BoothMasterId = g.Key,
                                        EventData = g.Sum(e => e.IsPollEnded == true ? 1 : 0),
                                    })
                                    .ToDictionaryAsync(g => g.BoothMasterId, g => g.EventData); // Specify the types explicitly here
                                break;
                            case 10:
                                electionInfoByPCId = await _context.ElectionInfoMaster
                                    .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.BoothMasterId == booth.BoothMasterId && e.IsMCESwitchOff == true)
                                    .GroupBy(e => e.BoothMasterId)
                                    .Select(g => new
                                    {
                                        BoothMasterId = g.Key,
                                        EventData = g.Sum(e => e.IsMCESwitchOff == true ? 1 : 0),
                                    })
                                    .ToDictionaryAsync(g => g.BoothMasterId, g => g.EventData); // Specify the types explicitly here
                                break;

                            case 11:
                                electionInfoByPCId = await _context.ElectionInfoMaster
                                    .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.BoothMasterId == booth.BoothMasterId && e.IsPartyDeparted == true)
                                    .GroupBy(e => e.BoothMasterId)
                                    .Select(g => new
                                    {
                                        BoothMasterId = g.Key,
                                        EventData = g.Sum(e => e.IsPartyDeparted == true ? 1 : 0),
                                    })
                                    .ToDictionaryAsync(g => g.BoothMasterId, g => g.EventData); // Specify the types explicitly here
                                break;
                            case 12:
                                electionInfoByPCId = await _context.ElectionInfoMaster
                                    .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.BoothMasterId == booth.BoothMasterId && e.IsPartyReachedCollectionCenter == true)
                                    .GroupBy(e => e.BoothMasterId)
                                    .Select(g => new
                                    {
                                        BoothMasterId = g.Key,
                                        EventData = g.Sum(e => e.IsPartyReachedCollectionCenter == true ? 1 : 0),
                                    })
                                    .ToDictionaryAsync(g => g.BoothMasterId, g => g.EventData); // Specify the types explicitly here
                                break;
                            case 13:
                                electionInfoByPCId = await _context.ElectionInfoMaster
                                    .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.BoothMasterId == booth.BoothMasterId && e.IsEVMDeposited == true)
                                    .GroupBy(e => e.BoothMasterId)
                                    .Select(g => new
                                    {
                                        BoothMasterId = g.Key,
                                        EventData = g.Sum(e => e.IsEVMDeposited == true ? 1 : 0),
                                    })
                                    .ToDictionaryAsync(g => g.BoothMasterId, g => g.EventData); // Specify the types explicitly here
                                break;


                            // Add more cases as needed

                            default:
                                electionInfoByPCId = await _context.ElectionInfoMaster
                                    .Where(e => e.StateMasterId == boothReportModel.StateMasterId && e.BoothMasterId == booth.BoothMasterId && e.IsPartyDispatched == true)
                                    .GroupBy(e => e.BoothMasterId)
                                    .Select(g => new
                                    {
                                        BoothMasterId = g.Key,
                                        EventData = g.Sum(e => e.IsPartyDispatched == true ? 1 : 0),
                                    })
                                    .ToDictionaryAsync(g => g.BoothMasterId, g => g.EventData); // Specify the types explicitly here
                                break;
                        }

                        //foreach (var ele in electionInfoByPCId)
                        //{
                        //    electionInfoByPCId.TryGetValue(ele.Key, out var eventData);
                        //    report.EventData.Add(new List<object> { allBooths.FirstOrDefault(x => x.BoothMasterId == ele.Key)?.BoothName ?? "", eventData });
                        //}
                        var getTotalBooths = await _context.BoothMaster.Where(p => p.StateMasterId == boothReportModel.StateMasterId && p.BoothStatus == true && p.BoothMasterId == booth.BoothMasterId).CountAsync();

                        if (electionInfoByPCId.TryGetValue(booth.BoothMasterId, out var eventData))
                        {
                            decimal getPercentage = Math.Round((decimal)(eventData * 100.0 / getTotalBooths), 1);

                            report.EventData.Add(new List<object> { booth.BoothName, eventData, getTotalBooths, getPercentage });


                        }
                        else
                        {
                            report.EventData.Add(new List<object> { booth.BoothName, 0, getTotalBooths, 0 });

                        }
                    }

                    consolidateBoothReports.Add(report);
                    return consolidateBoothReports;
                }
                else

                {
                    return consolidateBoothReports;
                }
            }

            return consolidateBoothReports;

        }

        #endregion


        #region AddHelpDeskInfo
        public async Task<Response> AddHelpDeskInfo(HelpDeskDetail helpDeskDetail)
        {
            try
            {

                //var hlpExist = _context.HelpDeskDetail.Where(p => p.MobileNumber == helpDeskDetail.MobileNumber && p.AssemblyMasterId == helpDeskDetail.AssemblyMasterId).FirstOrDefault();

                //if (hlpExist == null)
                //{

                helpDeskDetail.CreatedAt = BharatDateTime();
                _context.HelpDeskDetail.Add(helpDeskDetail);
                _context.SaveChanges();

                return new Response { Status = RequestStatusEnum.OK, Message = helpDeskDetail.ContactName + " " + "Contact Added Successfully" };

                //}
                //else
                //{
                //    return new Response { Status = RequestStatusEnum.BadRequest, Message = hlpExist.PcCodeNo + "Same PC Code Already Exists" };

                //}

            }

            catch (Exception ex)
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = ex.Message };
            }
        }


        public async Task<List<HelpDeskDetail>> GetHelpDeskDetail(string assemblyMasterId)
        {
            var boothRecord = await _context.HelpDeskDetail.Where(d => d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId)).OrderByDescending(d => d.CreatedAt).ToListAsync();

            return boothRecord;
        }
        #endregion



        /* public async Task<List<SoCountEventWiseReportModel>> GetDistrictWiseSOCountEventWiseCount(string stateId)
        {
            var eventActivityList = new List<SoCountEventWiseReportModel>();

            // Establish a connection to the PostgreSQL database
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("Postgres"));
            await connection.OpenAsync();

            var command = new NpgsqlCommand("select * from getdistrictwiseeventlistbyid(@state_master_id)", connection);
            command.Parameters.AddWithValue("@state_master_id", Convert.ToInt32(stateId));

            // Execute the command and read the results
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                // Create a new EventActivityCount object and populate its properties from the reader
                var eventActivityCount = new EventActivityCount
                {
                    Key = GenerateRandomAlphanumericString(6),
                    MasterId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Type = "District",
                    PartyDispatch = reader.IsDBNull(4) ? null : reader.GetString(4),
                    PartyArrived = reader.IsDBNull(5) ? null : reader.GetString(5),
                    SetupPollingStation = reader.IsDBNull(6) ? null : reader.GetString(6),
                    MockPollDone = reader.IsDBNull(7) ? null : reader.GetString(7),
                    PollStarted = reader.IsDBNull(8) ? null : reader.GetString(8),
                    PollEnded = reader.IsDBNull(9) ? null : reader.GetString(9),
                    MCEVMOff = reader.IsDBNull(10) ? null : reader.GetString(10),
                    PartyDeparted = reader.IsDBNull(11) ? null : reader.GetString(11),
                    EVMDeposited = reader.IsDBNull(12) ? null : reader.GetString(12),
                    PartyReachedAtCollection = reader.IsDBNull(13) ? null : reader.GetString(13),
                    QueueValue = reader.IsDBNull(14) ? null : reader.GetString(14),
                    FinalVotesValue = reader.IsDBNull(15) ? null : reader.GetString(15),
                    VoterTurnOutValue = reader.IsDBNull(16) ? null : reader.GetString(16),
                    TotalSo = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                    Children = new List<object>()
                };


                // Add the object to the list
                eventActivityList.Add(eventActivityCount);
            }

            return eventActivityList;
        }*/


        public async Task<List<CombinedMaster>> AppNotDownload(string StateMasterId)
        {



            IQueryable<CombinedMaster> solist = Enumerable.Empty<CombinedMaster>().AsQueryable();



            /*solist = from so in _context.SectorOfficerMaster.Where(d => d.StateMasterId == Convert.ToInt32(StateMasterId) && d.OTP == null && d.SoStatus == true) // outer sequence
                     join asem in _context.AssemblyMaster
                     on so.SoAssemblyCode equals asem.AssemblyCode
                     join pc in _context.ParliamentConstituencyMaster
                     on asem.PCMasterId equals pc.PCMasterId
                     where asem.StateMasterId == Convert.ToInt32(StateMasterId)
                     join state in _context.StateMaster
                      on pc.StateMasterId equals state.StateMasterId

                     select new CombinedMaster
                     { // result selector 
                         StateName = state.StateName,
                         //DistrictId = dist.DistrictMasterId,

                         PCMasterId = pc.PCMasterId,
                         PCName = pc.PcName,
                         AssemblyId = asem.AssemblyMasterId,
                         AssemblyName = asem.AssemblyName,
                         AssemblyCode = asem.AssemblyCode,
                         soName = so.SoName,
                         soMobile = so.SoMobile,
                         soDesignation = so.SoDesignation,
                         soMasterId = so.SOMasterId,

                         IsStatus = so.SoStatus


                     };*/




            return await solist.ToListAsync();




        }




        public async Task<int?> GetPCMasterIdByAssemblyIdCode(string assemblyMasterId)
        {
            var assemblyMaster = await _context.AssemblyMaster.FirstOrDefaultAsync(d => d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId));

            return assemblyMaster?.PCMasterId;
        }

        #region QueueManagement
        public async Task<ServiceResponse> AddQueue(QIS addQIS)
        {

            var isboothActive = _context.BoothMaster.Any(d => d.StateMasterId == addQIS.StateMasterId && d.DistrictMasterId == addQIS.DistrictMasterId && d.AssemblyMasterId == addQIS.AssemblyMasterId && d.BoothMasterId == addQIS.BoothMasterId && d.BoothStatus == true);
            if (isboothActive == true)
            {
                _context.QIS.Add(addQIS);
                int recordsAffected = await _context.SaveChangesAsync();

                if (recordsAffected > 0)
                {
                    return new ServiceResponse { IsSucceed = true, Message = "Record  inserted successfully." };
                }
                else
                {
                    return new ServiceResponse { IsSucceed = false, Message = "Failed to insert the record. Please try again later." };
                }
            }
            else
            {
                return new ServiceResponse { IsSucceed = false, Message = "Booth is not Active" };


            }
        }
        public async Task<QIS> GetQISList(int stateMasterId, int districtMasterId, int assemblyMasterId, int boothMasterId)
        {
            var result = await _context.QIS
                .Where(d => d.StateMasterId == stateMasterId &&
                            d.DistrictMasterId == districtMasterId &&
                            d.AssemblyMasterId == assemblyMasterId &&
                            d.BoothMasterId == boothMasterId)
                .OrderByDescending(d => d.CreatedAt)
                .FirstOrDefaultAsync();

            return result; // Return an empty QIS if result is null
        }


        #endregion



        #region Slot Based Voter Turn Out Report
        public async Task<List<VoterTurnOutSlotWise>> GetVoterTurnOutSlotBasedReport(string stateMasterId)
        {
            var voterTurnOutList = new List<VoterTurnOutSlotWise>();

            // Establish a connection to the PostgreSQL database
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("Postgres"));
            await connection.OpenAsync();

            var command = new NpgsqlCommand("SELECT * FROM getvoterturnoutreport_percentage(@state_master_id)", connection);
            command.Parameters.AddWithValue("@state_master_id", Convert.ToInt32(stateMasterId));

            // Execute the command and read the results
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var slotVotes = (string[])reader.GetValue(3); // Assuming slot_votes array is at index 3

                var voterTurnOut = new VoterTurnOutSlotWise
                {
                    Key = GenerateRandomAlphanumericString(6),
                    MasterId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Type = "District",
                    SlotVotes = slotVotes, // Assigning the array to SlotVotes property
                    Children = new List<object>()
                };

                // Add the object to the list
                voterTurnOutList.Add(voterTurnOut);
            }


            return voterTurnOutList;
        }
        public async Task<List<AssemblyVoterTurnOutSlotWise>> GetSlotVTReporttAssemblyWise(string stateId, string districtId)
        {
            var eventActivityList = new List<AssemblyVoterTurnOutSlotWise>();

            // Establish a connection to the PostgreSQL database
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("Postgres"));
            await connection.OpenAsync();

            var command = new NpgsqlCommand("SELECT * FROM getvoterturnoutreportassemblywise_percentage(@state_master_id, @district_master_id)", connection);
            command.Parameters.AddWithValue("@state_master_id", Convert.ToInt32(stateId));
            command.Parameters.AddWithValue("@district_master_id", Convert.ToInt32(districtId));

            // Execute the command and read the results
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var slotVotes = (string[])reader.GetValue(3);
                var eventActivityCount = new AssemblyVoterTurnOutSlotWise
                {
                    Key = GenerateRandomAlphanumericString(6), // You need to define this method to generate a random alphanumeric string
                    MasterId = reader.IsDBNull(0) ? (int?)null : reader.GetInt32(0),
                    Name = reader.IsDBNull(2) ? ((int?)null).ToString() : reader.GetInt32(2).ToString() + "-" + (reader.IsDBNull(1) ? null : reader.GetString(1)),
                    //Name = reader.IsDBNull(1) ? null : reader.GetString(1),
                    Type = "Assembly", // Assuming this is the type for assembly
                    StateMasterId = Convert.ToInt32(stateId),
                    DistrictMasterId = Convert.ToInt32(districtId),
                    AssemblyCode = reader.IsDBNull(2) ? (int?)null : reader.GetInt32(2),
                    SlotVotes = slotVotes, // Assigning the array to SlotVotes property
                    Children = new List<object>()
                };

                // Add the object to the list
                eventActivityList.Add(eventActivityCount);
            }

            return eventActivityList;
        }
        public async Task<List<BoothWiseVoterTurnOutSlotWise>> GetSlotVTReportBoothWise(string stateId, string districtId, string assemblyId)
        {
            var eventActivityList = new List<BoothWiseVoterTurnOutSlotWise>();

            // Establish a connection to the PostgreSQL database
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("Postgres"));
            await connection.OpenAsync();

            var command = new NpgsqlCommand("SELECT * FROM getvoterturnoutreportboothwise_percentage(@state_master_id, @district_master_id, @assembly_master_id)", connection);
            command.Parameters.AddWithValue("@state_master_id", Convert.ToInt32(stateId));
            command.Parameters.AddWithValue("@district_master_id", Convert.ToInt32(districtId));
            command.Parameters.AddWithValue("@assembly_master_id", Convert.ToInt32(assemblyId));

            // Execute the command and read the results
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var slotVotes = (string[])reader.GetValue(4);
                string boothAuxy = reader.IsDBNull(3) ? (string?)null : reader.GetString(3); string boothName = "";
                if (boothAuxy == null || boothAuxy == "0")
                {
                    boothName = reader.IsDBNull(2) ? null : reader.GetString(2) + "-" + reader.GetString(1);
                }
                else
                {
                    boothName = reader.IsDBNull(2) ? null : reader.GetString(2) + reader.GetString(3) + "-" + reader.GetString(1);
                    //boothName = reader.IsDBNull(1) ? null : reader.GetString(1) + "(" + reader.GetString(2) + reader.GetString(3) + ")";

                }
                var eventActivityCount = new BoothWiseVoterTurnOutSlotWise
                {
                    Key = GenerateRandomAlphanumericString(6), // You need to define this method to generate a random alphanumeric string
                    MasterId = reader.IsDBNull(0) ? (int?)null : reader.GetInt32(0),


                    Name = boothName,
                    Type = "Booth",
                    SlotVotes = slotVotes,
                    //AssignedSOMobile = reader.IsDBNull(16) ? null : reader.GetString(16),
                    //AssignedSOName = reader.IsDBNull(17) ? null : reader.GetString(17)

                };

                // Add the object to the list
                eventActivityList.Add(eventActivityCount);
            }


            return eventActivityList;
        }

        #endregion


        #region SO Pendency Screen
        public async Task<List<SectorOfficerPendency>> GetDistrictWiseSOCountEventWiseCount(string stateMasterId)
        {
            var sopendencyList = new List<SectorOfficerPendency>();

            // Establish a connection to the PostgreSQL database
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("Postgres"));
            await connection.OpenAsync();

            var command = new NpgsqlCommand("SELECT * FROM getsopendency_districtwise(@state_master_id)", connection);
            command.Parameters.AddWithValue("@state_master_id", Convert.ToInt32(stateMasterId));

            // Execute the command and read the results
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var listso = new SectorOfficerPendency
                {
                    Key = GenerateRandomAlphanumericString(6),
                    MasterId = reader.GetInt32(1),
                    Name = reader.GetString(2) + "(" + reader.GetInt32(3).ToString() + ")",
                    //    Name = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Type = "District",
                    SOBoothAllocated_NotAllocated = reader.GetInt32(4).ToString() + "(" + reader.GetInt32(5).ToString() + ")",
                    // SOBoothAllocated_NotAllocated = reader.GetInt32(4).ToString(),
                    PartyDispatch = reader.GetInt32(7).ToString() + "(" + reader.GetInt32(6).ToString() + ")",
                    PartyArrived = reader.GetInt32(9).ToString() + "(" + reader.GetInt32(8).ToString() + ")",
                    SetupPollingStation = reader.GetInt32(11).ToString() + "(" + reader.GetInt32(10).ToString() + ")",
                    MockPollDone = reader.GetInt32(13).ToString() + "(" + reader.GetInt32(12).ToString() + ")",
                    PollStarted = reader.GetInt32(15).ToString() + "(" + reader.GetInt32(14).ToString() + ")",
                    FinalVotesValue = reader.GetInt32(17).ToString() + "(" + reader.GetInt32(16).ToString() + ")",
                    //QueueValue 
                    PollEnded = reader.GetInt32(19).ToString() + "(" + reader.GetInt32(18).ToString() + ")",
                    MCEVMOff = reader.GetInt32(21).ToString() + "(" + reader.GetInt32(20).ToString() + ")",
                    PartyDeparted = reader.GetInt32(23).ToString() + "(" + reader.GetInt32(22).ToString() + ")",
                    PartyReachedAtCollection = reader.GetInt32(25).ToString() + "(" + reader.GetInt32(24).ToString() + ")",
                    EVMDeposited = reader.GetInt32(27).ToString() + "(" + reader.GetInt32(26).ToString() + ")",
                    Children = new List<object>()
                };

                // Add the object to the list
                sopendencyList.Add(listso);
            }


            return sopendencyList;

        }


        public async Task<List<SoCountEventWiseReportModel>> GetSectorOfficerPendency_CsharpMethod(BoothReportModel boothReportModel)
        {
            List<SoCountEventWiseReportModel> modelso = new List<SoCountEventWiseReportModel>();
            if (boothReportModel.StateMasterId != 0)
            {
                var districtList = await _context.DistrictMaster
                    .Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictStatus)
                    .ToListAsync();



                foreach (var district in districtList)
                {
                    if (district.DistrictMasterId == 43 || district.DistrictMasterId == 4)
                    {

                        //find total so
                        var asembCodeList = await _context.AssemblyMaster
                            .Where(p => p.StateMasterId == boothReportModel.StateMasterId && p.AssemblyStatus && p.DistrictMasterId == district.DistrictMasterId)
                            .Select(p => p.AssemblyCode)
                            .ToListAsync();

                        int totalSO = await _context.SectorOfficerMaster
                                        .Where(p => p.StateMasterId == boothReportModel.StateMasterId &&
                                                    p.SoStatus &&
                                                    asembCodeList.Contains(p.SoAssemblyCode)).CountAsync();

                        var totalSO_list = await _context.SectorOfficerMaster
                               .Where(p => p.StateMasterId == boothReportModel.StateMasterId &&
                                           p.SoStatus &&
                                           asembCodeList.Contains(p.SoAssemblyCode)).ToListAsync();

                        SoCountEventWiseReportModel report = new SoCountEventWiseReportModel
                        {
                            District = district.DistrictName,
                            NoOfSo = totalSO,


                        };
                        int pendingPartyDispatchSO = 0; int pendingPartyReachedSO = 0;
                        int SOBoothNotAlloted = 0;
                        foreach (var m in totalSO_list)
                        {

                            var boothlistMasterid = await _context.BoothMaster.Where(p => p.StateMasterId == boothReportModel.StateMasterId && p.BoothStatus && p.AssignedTo == m.SOMasterId.ToString()).Select(p => p.BoothMasterId).ToListAsync();

                            if (boothlistMasterid.Count > 0)
                            {
                                //check for all booths of so that ispartydispatch event done
                                var allDispatched = await _context.ElectionInfoMaster.Where(ei => boothlistMasterid.Contains(ei.BoothMasterId) && ei.IsPartyDispatched == true)
                .Select(ei => ei.BoothMasterId)
                .Distinct()
                .ToListAsync();
                                //check for all booths of so that ispartyreach event done
                                var allReached = await _context.ElectionInfoMaster.Where(ei => boothlistMasterid.Contains(ei.BoothMasterId) && ei.IsPartyReached == true)
                .Select(ei => ei.BoothMasterId)
                .Distinct()
                .ToListAsync();
                                //compare the total booth of so is equal to alldispatched booths, if not then assign in pendingSo varaiable
                                if (boothlistMasterid.Count != allDispatched.Count)
                                {
                                    // report.PartyDispatch = boothlistMasterid.Count - allDispatched.Count;

                                    pendingPartyDispatchSO = pendingPartyDispatchSO + 1;

                                }
                                //similar to reach event
                                if (boothlistMasterid.Count != allReached.Count)
                                {
                                    //report.PartyReach = boothlistMasterid.Count - allReached.Count;
                                    pendingPartyReachedSO = pendingPartyReachedSO + 1;


                                }
                            }
                            else
                            {
                                SOBoothNotAlloted = SOBoothNotAlloted + 1;

                            }



                        }

                        // now here main assignment of returning variable dispatchCount
                        if (pendingPartyDispatchSO > 0)
                        {
                            report.PartyDispatch = pendingPartyDispatchSO;
                        }
                        // now here main assignment of returning variable reachCount
                        if (pendingPartyReachedSO > 0)
                        {
                            report.PartyReach = pendingPartyReachedSO;
                        }

                        if (SOBoothNotAlloted > 0)
                        {
                            report.SoBoothNotAllocated = SOBoothNotAlloted;
                        }



                        modelso.Add(report);

                    }




                }
            }
            return modelso;
        }


        public async Task<List<SectorOfficerPendencyAssembly>> GetAssemblyWiseSOCountEventWiseCount(string stateId, string districtId)
        {
            var eventActivityList = new List<SectorOfficerPendencyAssembly>();


            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("Postgres"));
            await connection.OpenAsync();

            var command = new NpgsqlCommand("SELECT * FROM getsopendency_assemblywise(@state_masterid, @district_masterid) ORDER BY assembly_code ASC", connection);
            command.Parameters.AddWithValue("@state_masterid", Convert.ToInt32(stateId));
            command.Parameters.AddWithValue("@district_masterid", Convert.ToInt32(districtId));

            // Execute the command and read the results
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                // Create a new AssemblyEventActivityCount object and populate its properties from the reader
                var eventActivityCount = new SectorOfficerPendencyAssembly
                {
                    Key = GenerateRandomAlphanumericString(6), // You need to define this method to generate a random alphanumeric string
                    MasterId = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),//assemblyMasterId
                    //Name = reader.IsDBNull(4) ? null : reader.GetString(4),//assemblyname
                    Name = reader.IsDBNull(5) ? ((int?)null).ToString() : reader.GetInt32(5).ToString() + "-" + (reader.IsDBNull(4) ? null : reader.GetString(4)),
                    Type = "Assembly",
                    StateMasterId = Convert.ToInt32(stateId),
                    DistrictMasterId = Convert.ToInt32(districtId),
                    AssemblyCode = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5),
                    TotalSoCount = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6),
                    SOBoothAllocated_NotAllocated = reader.GetInt32(7).ToString() + "(" + reader.GetInt32(8).ToString() + ")",

                    PartyDispatch = reader.GetInt32(10).ToString() + "(" + reader.GetInt32(9).ToString() + ")",
                    PartyArrived = reader.GetInt32(12).ToString() + "(" + reader.GetInt32(11).ToString() + ")",
                    SetupPollingStation = reader.GetInt32(14).ToString() + "(" + reader.GetInt32(13).ToString() + ")",
                    MockPollDone = reader.GetInt32(16).ToString() + "(" + reader.GetInt32(15).ToString() + ")",
                    PollStarted = reader.GetInt32(18).ToString() + "(" + reader.GetInt32(17).ToString() + ")",
                    FinalVotesValue = reader.GetInt32(20).ToString() + "(" + reader.GetInt32(19).ToString() + ")",
                    //QueueValue 
                    PollEnded = reader.GetInt32(22).ToString() + "(" + reader.GetInt32(21).ToString() + ")",
                    MCEVMOff = reader.GetInt32(24).ToString() + "(" + reader.GetInt32(23).ToString() + ")",
                    PartyDeparted = reader.GetInt32(26).ToString() + "(" + reader.GetInt32(25).ToString() + ")",
                    PartyReachedAtCollection = reader.GetInt32(28).ToString() + "(" + reader.GetInt32(27).ToString() + ")",
                    EVMDeposited = reader.GetInt32(30).ToString() + "(" + reader.GetInt32(29).ToString() + ")",
                    Children = new List<object>()
                };

                // Add the object to the list
                eventActivityList.Add(eventActivityCount);
            }




            //Now call those assemblies which do not have any booths as they are not cominmg in above function, calling seperatly
            await using var connection2 = new NpgsqlConnection(_configuration.GetConnectionString("Postgres"));
            await connection2.OpenAsync();
            var command2 = new NpgsqlCommand("SELECT * FROM findsopendency_boothnotexists(@state_master_id, @district_master_id) ", connection2);
            command2.Parameters.AddWithValue("@state_master_id", Convert.ToInt32(stateId));
            command2.Parameters.AddWithValue("@district_master_id", Convert.ToInt32(districtId));

            using var reader2 = await command2.ExecuteReaderAsync();

            while (await reader2.ReadAsync())
            {
                // Create a new AssemblyEventActivityCount object and populate its properties from the reader
                var eventActivityCount2 = new SectorOfficerPendencyAssembly
                {
                    Key = GenerateRandomAlphanumericString(6), // You need to define this method to generate a random alphanumeric string
                    MasterId = reader2.IsDBNull(0) ? (int?)null : reader2.GetInt32(0), //assemblyMasterId
                    Name = reader2.IsDBNull(1) ? null : reader2.GetString(1), //assemblyname
                    Type = "Assembly",
                    StateMasterId = Convert.ToInt32(stateId),
                    DistrictMasterId = Convert.ToInt32(districtId),
                    AssemblyCode = reader2.IsDBNull(2) ? (int?)null : reader2.GetInt32(2),
                    TotalSoCount = reader2.IsDBNull(3) ? (int?)null : reader2.GetInt32(3),
                    SOBoothAllocated_NotAllocated = reader2.GetInt32(4).ToString() + "(" + reader2.GetInt32(5).ToString() + ")",
                    PartyDispatch = reader2.GetInt32(7).ToString() + "(" + reader2.GetInt32(6).ToString() + ")",
                    PartyArrived = reader2.GetInt32(9).ToString() + "(" + reader2.GetInt32(8).ToString() + ")",
                    SetupPollingStation = reader2.GetInt32(11).ToString() + "(" + reader2.GetInt32(10).ToString() + ")",
                    MockPollDone = reader2.GetInt32(13).ToString() + "(" + reader2.GetInt32(12).ToString() + ")",
                    PollStarted = reader2.GetInt32(15).ToString() + "(" + reader2.GetInt32(14).ToString() + ")",
                    FinalVotesValue = reader2.GetInt32(17).ToString() + "(" + reader2.GetInt32(16).ToString() + ")",
                    //QueueValue 
                    PollEnded = reader2.GetInt32(19).ToString() + "(" + reader2.GetInt32(18).ToString() + ")",
                    MCEVMOff = reader2.GetInt32(21).ToString() + "(" + reader2.GetInt32(20).ToString() + ")",
                    PartyDeparted = reader2.GetInt32(23).ToString() + "(" + reader2.GetInt32(22).ToString() + ")",
                    PartyReachedAtCollection = reader2.GetInt32(25).ToString() + "(" + reader2.GetInt32(24).ToString() + ")",
                    EVMDeposited = reader2.GetInt32(27).ToString() + "(" + reader2.GetInt32(26).ToString() + ")",
                    Children = new List<object>()
                };

                // Add the object to the list
                eventActivityList.Add(eventActivityCount2);
            }

            // Sort the eventActivityList by AssemblyCode in ascending order
            eventActivityList = eventActivityList.OrderBy(a => a.AssemblyCode).ToList();


            return eventActivityList;
        }


        public async Task<List<SectorOfficerPendencybySoNames>> GetSONamesEventWiseCount(string stateId, string districtId, string assemblyId)
        {
            //var getAssemblyRecord = _context.AssemblyMaster.FirstOrDefault(d => d.AssemblyMasterId == Convert.ToInt32(assemblyId));

            var soListmain = new List<SectorOfficerPendencybySoNames>();

            // Establish a connection to the PostgreSQL database
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("Postgres"));
            await connection.OpenAsync();

            var command = new NpgsqlCommand("SELECT * FROM sogetlistbyassembly(@state_master_id, @district_master_id, @assembly_master_id)", connection);
            command.Parameters.AddWithValue("@state_master_id", Convert.ToInt32(stateId));
            command.Parameters.AddWithValue("@district_master_id", Convert.ToInt32(districtId));
            command.Parameters.AddWithValue("@assembly_master_id", Convert.ToInt32(assemblyId));

            // Execute the command and read the results
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                // Create a new EventActivityBoothWise object and populate its properties from the reader
                var soList = new SectorOfficerPendencybySoNames
                {
                    Key = GenerateRandomAlphanumericString(6), // You need to define this method to generate a random alphanumeric string
                    MasterId = reader.IsDBNull(0) ? (int?)null : reader.GetInt32(0),
                    Name = reader.IsDBNull(1) ? null : reader.GetString(1),
                    Type = "SO", // Assuming this is the type for SO
                    AssignedSOMobile = reader.IsDBNull(2) ? null : reader.GetString(2),
                    AssignedSOName = reader.IsDBNull(1) ? null : reader.GetString(1),

                    PartyDispatch = reader.IsDBNull(4) ? null : reader.GetString(4),
                    PartyArrived = reader.IsDBNull(5) ? null : reader.GetString(5),
                    SetupPollingStation = reader.IsDBNull(6) ? null : reader.GetString(6),
                    MockPollDone = reader.IsDBNull(7) ? null : reader.GetString(7),
                    PollStarted = reader.IsDBNull(8) ? null : reader.GetString(8),
                    PollEnded = reader.IsDBNull(9) ? null : reader.GetString(9),
                    MCEVMOff = reader.IsDBNull(10) ? null : reader.GetString(10),
                    PartyDeparted = reader.IsDBNull(11) ? null : reader.GetString(11),
                    EVMDeposited = reader.IsDBNull(12) ? null : reader.GetString(12),
                    PartyReachedAtCollection = reader.IsDBNull(13) ? null : reader.GetString(13),
                    FinalVotesValue = reader.IsDBNull(14) ? null : reader.GetString(14),
                    Children = new List<object>()
                };

                // Add the object to the list
                soListmain.Add(soList);
            }

            return soListmain;
        }

        public async Task<List<SectorOfficerPendencyBooth>> GetBoothWiseSOEventWiseCount(string soMasterId)
        {

            var getsoRecord = _context.SectorOfficerMaster.FirstOrDefault(d => d.SOMasterId == Convert.ToInt32(soMasterId));
            var getAssemblyRecord = _context.AssemblyMaster.FirstOrDefault(d => d.AssemblyCode == getsoRecord.SoAssemblyCode && d.StateMasterId == getsoRecord.StateMasterId);
            var eventActivityList = new List<SectorOfficerPendencyBooth>();

            // Establish a connection to the PostgreSQL database
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("Postgres"));
            await connection.OpenAsync();

            //var command = new NpgsqlCommand("SELECT * FROM getboothwiseeventlistbyid_sopendency(@state_master_id, @district_master_id, @assembly_master_id)", connection);
            var command = new NpgsqlCommand("SELECT * FROM getboothwiseeventlistbyid_sopendency_NEW(@so_master_id,@assembly_master_id)", connection);
            command.Parameters.AddWithValue("@so_master_id", Convert.ToInt32(soMasterId));
            command.Parameters.AddWithValue("@assembly_master_id", getAssemblyRecord.AssemblyMasterId);

            // Execute the command and read the results
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                // Create a new EventActivityBoothWise object and populate its properties from the reader
                var eventActivityBoothWise = new SectorOfficerPendencyBooth
                {
                    Key = GenerateRandomAlphanumericString(6), // You need to define this method to generate a random alphanumeric string
                    MasterId = reader.IsDBNull(0) ? (int?)null : reader.GetInt32(0),
                    Name = reader.IsDBNull(1) ? null : reader.GetString(1),
                    Type = "Booth", // Assuming this is the type for booth
                    AssignedSOMobile = reader.IsDBNull(3) ? null : reader.GetString(3),
                    AssignedSOName = reader.IsDBNull(2) ? null : reader.GetString(2),
                    PartyDispatch = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4),
                    PartyArrived = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5),
                    SetupPollingStation = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6),
                    MockPollDone = reader.IsDBNull(7) ? (int?)null : reader.GetInt32(7),
                    PollStarted = reader.IsDBNull(8) ? (int?)null : reader.GetInt32(8),
                    VoterTurnOutValue = reader.IsDBNull(16) ? (int?)null : reader.GetInt32(16),
                    QueueValue = reader.IsDBNull(14) ? (int?)null : reader.GetInt32(14),
                    FinalVotesValue = reader.IsDBNull(15) ? (int?)null : reader.GetInt32(15),
                    PollEnded = reader.IsDBNull(9) ? (int?)null : reader.GetInt32(9),
                    MCEVMOff = reader.IsDBNull(10) ? (int?)null : reader.GetInt32(10),
                    PartyDeparted = reader.IsDBNull(11) ? (int?)null : reader.GetInt32(11),
                    EVMDeposited = reader.IsDBNull(12) ? (int?)null : reader.GetInt32(12),
                    PartyReachedAtCollection = reader.IsDBNull(13) ? (int?)null : reader.GetInt32(13)
                };

                // Add the object to the list
                eventActivityList.Add(eventActivityBoothWise);
            }

            return eventActivityList;
        }



        /* public async Task<List<SectorOfficerPendencyBooth>> GetBoothWiseSOEventWiseCount(string stateId, string districtId, string assemblyId)
        {
            var eventActivityList = new List<SectorOfficerPendencyBooth>();

            // Establish a connection to the PostgreSQL database
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("Postgres"));
            await connection.OpenAsync();

            var command = new NpgsqlCommand("SELECT * FROM getboothwiseeventlistbyid_sopendency(@state_master_id, @district_master_id, @assembly_master_id)", connection);
            command.Parameters.AddWithValue("@state_master_id", Convert.ToInt32(stateId));
            command.Parameters.AddWithValue("@district_master_id", Convert.ToInt32(districtId));
            command.Parameters.AddWithValue("@assembly_master_id", Convert.ToInt32(assemblyId));

            // Execute the command and read the results
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                // Create a new EventActivityBoothWise object and populate its properties from the reader
                var eventActivityBoothWise = new SectorOfficerPendencyBooth
                {
                    Key = GenerateRandomAlphanumericString(6), // You need to define this method to generate a random alphanumeric string
                    MasterId = reader.IsDBNull(0) ? (int?)null : reader.GetInt32(0),
                    Name = reader.IsDBNull(1) ? null : reader.GetString(1),
                    Type = "Booth", // Assuming this is the type for booth
                    AssignedSOMobile = reader.IsDBNull(3) ? null : reader.GetString(3),
                    AssignedSOName = reader.IsDBNull(2) ? null : reader.GetString(2),
                    PartyDispatch = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4),
                    PartyArrived = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5),
                    SetupPollingStation = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6),
                    MockPollDone = reader.IsDBNull(7) ? (int?)null : reader.GetInt32(7),
                    PollStarted = reader.IsDBNull(8) ? (int?)null : reader.GetInt32(8),
                    VoterTurnOutValue = reader.IsDBNull(16) ? (int?)null : reader.GetInt32(16),
                    QueueValue = reader.IsDBNull(14) ? (int?)null : reader.GetInt32(14),
                    FinalVotesValue = reader.IsDBNull(15) ? (int?)null : reader.GetInt32(15),
                    PollEnded = reader.IsDBNull(9) ? (int?)null : reader.GetInt32(9),
                    MCEVMOff = reader.IsDBNull(10) ? (int?)null : reader.GetInt32(10),
                    PartyDeparted = reader.IsDBNull(11) ? (int?)null : reader.GetInt32(11),
                    EVMDeposited = reader.IsDBNull(12) ? (int?)null : reader.GetInt32(12),
                    PartyReachedAtCollection = reader.IsDBNull(13) ? (int?)null : reader.GetInt32(13)
                };

                // Add the object to the list
                eventActivityList.Add(eventActivityBoothWise);
            }

            return eventActivityList;
        }*/
        #endregion

        #region BLOBoothMaster
        public async Task<List<BoothMaster>> GetBLOBoothById(string bloBoothMasterId)
        {
            var boothList = await _context.BoothMaster.Where(d => d.AssignedToBLO == bloBoothMasterId).ToListAsync();
            if (boothList is not null)
            {
                return boothList;
            }
            else
            {
                return null;
            }
        }
        public async Task<SectorOfficerProfile> GetBLOOfficerProfile(string bloMasterId)
        {
            int bloIdInt = Convert.ToInt32(bloMasterId);

            var result = await (from blo in _context.BLOMaster
                                where blo.BLOMasterId == bloIdInt && blo.BLOStatus == true
                                join state in _context.StateMaster on blo.StateMasterId equals state.StateMasterId
                                join district in _context.DistrictMaster on blo.DistrictMasterId equals district.DistrictMasterId
                                join assembly in _context.AssemblyMaster on blo.AssemblyMasterId equals assembly.AssemblyMasterId
                                select new SectorOfficerProfile
                                {
                                    StateName = state.StateName,
                                    DistrictName = district.DistrictName,
                                    AssemblyName = assembly.AssemblyName,
                                    AssemblyCode = assembly.AssemblyCode.ToString(),
                                    SoName = blo.BLOName,
                                    OfficerRole = "BLO",
                                    ElectionType = "LS",
                                    // Uncomment and modify the following line if BoothNo is required
                                    BoothNo = _context.BoothMaster.Where(p => p.AssignedToBLO == bloMasterId).OrderBy(p => p.BoothCode_No).Select(p => p.BoothCode_No.ToString()).ToList()
                                }).FirstOrDefaultAsync();

            return result;
        }
        public async Task<List<CombinedMaster>> GetBlosListById(string stateMasterId, string districtMasterId, string assemblyMasterId)
        {
            IQueryable<CombinedMaster> solist = Enumerable.Empty<CombinedMaster>().AsQueryable();
            var isStateActive = _context.StateMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId)).FirstOrDefault();
            var isDistrictActive = _context.DistrictMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId)).FirstOrDefault();
            var isAssemblyActive = _context.AssemblyMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId)).FirstOrDefault();
            if (isStateActive.StateStatus && isDistrictActive.DistrictStatus && isAssemblyActive.AssemblyStatus)
            {
                if (districtMasterId == "0")
                {
                    solist = from so in _context.BLOMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId)) // outer sequence
                             join asem in _context.AssemblyMaster
                             on so.AssemblyMasterId equals asem.AssemblyMasterId
                             join pc in _context.ParliamentConstituencyMaster
                             on asem.PCMasterId equals pc.PCMasterId
                             where asem.StateMasterId == Convert.ToInt32(stateMasterId) && asem.AssemblyMasterId == Convert.ToInt32(assemblyMasterId) // key selector
                             join state in _context.StateMaster
                              on pc.StateMasterId equals state.StateMasterId

                             select new CombinedMaster
                             { // result selector 
                                 StateName = state.StateName,
                                 //DistrictId = dist.DistrictMasterId,
                                 //DistrictName = dist.DistrictName,
                                 //DistrictCode = dist.DistrictCode,
                                 PCMasterId = pc.PCMasterId,
                                 PCName = pc.PcName,
                                 AssemblyId = asem.AssemblyMasterId,
                                 AssemblyName = asem.AssemblyName,
                                 AssemblyCode = asem.AssemblyCode,
                                 soName = so.BLOName,
                                 soMobile = so.BLOMobile,
                                 soMasterId = so.BLOMasterId,
                                 RecentOTP = so.OTP,
                                 OTPExpireTime = so.OTPExpireTime,
                                 OTPAttempts = so.OTPAttempts,
                                 IsStatus = so.BLOStatus


                             };
                }
                else
                {
                    solist = from so in _context.BLOMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId)) // outer sequence
                             join asem in _context.AssemblyMaster
                             on so.AssemblyMasterId equals asem.AssemblyMasterId
                             join dist in _context.DistrictMaster
                             on asem.DistrictMasterId equals dist.DistrictMasterId
                             where asem.DistrictMasterId == Convert.ToInt32(districtMasterId) && asem.AssemblyMasterId == Convert.ToInt32(assemblyMasterId) // key selector
                             join state in _context.StateMaster
                              on dist.StateMasterId equals state.StateMasterId

                             select new CombinedMaster
                             { // result selector 
                                 StateName = state.StateName,
                                 DistrictId = dist.DistrictMasterId,
                                 DistrictName = dist.DistrictName,
                                 DistrictCode = dist.DistrictCode,
                                 AssemblyId = asem.AssemblyMasterId,
                                 AssemblyName = asem.AssemblyName,
                                 AssemblyCode = asem.AssemblyCode,
                                 soName = so.BLOName,
                                 soMobile = so.BLOMobile,
                                 soMasterId = so.BLOMasterId,
                                 RecentOTP = so.OTP,
                                 OTPExpireTime = so.OTPExpireTime,
                                 OTPAttempts = so.OTPAttempts,
                                 IsStatus = so.BLOStatus


                             };

                }


                return await solist.ToListAsync();
            }
            else
            {
                return null;
            }
        }
        public async Task<Response> UpdateBLOOfficer(BLOMaster updatedBLOMaster)
        {
            var existingBLOOfficer = await _context.BLOMaster
                                                       .FirstOrDefaultAsync(so => so.BLOMasterId == updatedBLOMaster.BLOMasterId);
            var getPcMasterId = _context.AssemblyMaster.FirstOrDefaultAsync(d => d.AssemblyMasterId == updatedBLOMaster.AssemblyMasterId).Result.PCMasterId;
            existingBLOOfficer.PCMasterId = getPcMasterId;

            var isSoExistMobileNumber = _context.SectorOfficerMaster.Where(d => d.SoMobile == updatedBLOMaster.BLOMobile).FirstOrDefault();
            if (isSoExistMobileNumber == null)
            {

                if (existingBLOOfficer == null)
                {

                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "SO User" + existingBLOOfficer.BLOName + " " + "Not found" };
                }

                if (existingBLOOfficer.BLOStatus == true)
                {
                    //check if true (then state,district,assembly acive
                    var assemblyActive = _context.AssemblyMaster.Where(p => p.AssemblyMasterId == updatedBLOMaster.AssemblyMasterId && p.StateMasterId == updatedBLOMaster.StateMasterId).Select(p => p.AssemblyStatus).FirstOrDefault();
                    if (assemblyActive == true)
                    { // Check if the mobile number is unique among other sector officers (excluding the current one being updated)
                        var isMobileUnique = await _context.BLOMaster.AnyAsync(so => so.BLOMobile == updatedBLOMaster.BLOMobile);
                        if (string.Equals(updatedBLOMaster.BLOMobile, existingBLOOfficer.BLOMobile, StringComparison.OrdinalIgnoreCase))
                        {

                            existingBLOOfficer.StateMasterId = updatedBLOMaster.StateMasterId;
                            existingBLOOfficer.DistrictMasterId = updatedBLOMaster.DistrictMasterId;
                            existingBLOOfficer.AssemblyMasterId = updatedBLOMaster.AssemblyMasterId;
                            existingBLOOfficer.BLOName = updatedBLOMaster.BLOName;
                            existingBLOOfficer.BLOMobile = updatedBLOMaster.BLOMobile;
                            existingBLOOfficer.BLOUpdatedAt = BharatDateTime();

                            _context.BLOMaster.Update(existingBLOOfficer);
                            await _context.SaveChangesAsync();


                            return new Response { Status = RequestStatusEnum.OK, Message = "BLO User " + existingBLOOfficer.BLOName + " " + "updated successfully" };
                        }
                        else
                        {
                            if (isMobileUnique == false)
                            {
                                existingBLOOfficer.StateMasterId = updatedBLOMaster.StateMasterId;
                                existingBLOOfficer.DistrictMasterId = updatedBLOMaster.DistrictMasterId;
                                existingBLOOfficer.AssemblyMasterId = updatedBLOMaster.AssemblyMasterId;
                                existingBLOOfficer.BLOName = updatedBLOMaster.BLOName;
                                existingBLOOfficer.BLOMobile = updatedBLOMaster.BLOMobile;
                                existingBLOOfficer.BLOUpdatedAt = BharatDateTime();

                                _context.BLOMaster.Update(existingBLOOfficer);
                                await _context.SaveChangesAsync();
                                return new Response { Status = RequestStatusEnum.OK, Message = "BLO User " + existingBLOOfficer.BLOName + " " + "updated successfully" };

                            }
                            else
                            {
                                var existSo = _context.BLOMaster.Where(so => so.BLOMobile == updatedBLOMaster.BLOMobile).FirstOrDefault();
                                var assembly = _context.AssemblyMaster.Where(d => d.AssemblyMasterId == existSo.AssemblyMasterId).FirstOrDefault();

                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "BLO User WIth given Mobile Number : " + updatedBLOMaster.BLOMobile + " " + "Already Exists with following Details " + " " + existSo.BLOMobile + " , " + " AssemblyCode - " + assembly.AssemblyCode + " " + "ARO Assembly Name - " + " " + assembly.AssemblyName };

                            }

                        }

                    }
                    else
                    {
                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Assembly is not Active" };

                    }
                }
                else if (existingBLOOfficer.BLOStatus == false)
                {// if assigned any booths not allowed to deactivate

                    var boothListSo = _context.BoothMaster.Where(p => p.AssignedToBLO == existingBLOOfficer.BLOMasterId.ToString() && p.StateMasterId == existingBLOOfficer.StateMasterId).ToList();

                    if (boothListSo.Count == 0)
                    {
                        //release booths first


                        // Check if the mobile number is unique among other sector officers (excluding the current one being updated)
                        var isMobileUnique = await _context.BLOMaster.AnyAsync(so => so.BLOMobile == updatedBLOMaster.BLOMobile);
                        if (string.Equals(updatedBLOMaster.BLOMobile, existingBLOOfficer.BLOMobile, StringComparison.OrdinalIgnoreCase))
                        {

                            existingBLOOfficer.StateMasterId = updatedBLOMaster.StateMasterId;
                            existingBLOOfficer.DistrictMasterId = updatedBLOMaster.DistrictMasterId;
                            existingBLOOfficer.AssemblyMasterId = updatedBLOMaster.AssemblyMasterId;
                            existingBLOOfficer.BLOName = updatedBLOMaster.BLOName;
                            existingBLOOfficer.BLOMobile = updatedBLOMaster.BLOMobile;
                            existingBLOOfficer.BLOUpdatedAt = BharatDateTime();

                            _context.BLOMaster.Update(existingBLOOfficer);
                            await _context.SaveChangesAsync();


                            return new Response { Status = RequestStatusEnum.OK, Message = "BLO User " + existingBLOOfficer.BLOName + " " + "updated successfully" };
                        }
                        else
                        {
                            if (isMobileUnique == false)
                            {
                                existingBLOOfficer.StateMasterId = updatedBLOMaster.StateMasterId;
                                existingBLOOfficer.DistrictMasterId = updatedBLOMaster.DistrictMasterId;
                                existingBLOOfficer.AssemblyMasterId = updatedBLOMaster.AssemblyMasterId;
                                existingBLOOfficer.BLOName = updatedBLOMaster.BLOName;
                                existingBLOOfficer.BLOMobile = updatedBLOMaster.BLOMobile;
                                existingBLOOfficer.BLOUpdatedAt = BharatDateTime();

                                _context.BLOMaster.Update(existingBLOOfficer);
                                await _context.SaveChangesAsync();
                                return new Response { Status = RequestStatusEnum.OK, Message = "BLO User " + existingBLOOfficer.BLOName + " " + "updated successfully" };

                            }
                            else
                            {
                                var existSo = _context.BLOMaster.Where(so => so.BLOMobile == updatedBLOMaster.BLOMobile).FirstOrDefault();
                                var assembly = _context.AssemblyMaster.Where(d => d.AssemblyMasterId == existSo.AssemblyMasterId).FirstOrDefault();

                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "BLO User WIth given Mobile Number : " + updatedBLOMaster.BLOMobile + " " + "Already Exists with following Details" + existSo.BLOName + " AssemblyCode" + assembly.AssemblyCode + " ARO Assembly Name" + assembly.AssemblyName };

                            }

                        }

                    }
                    else
                    {
                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Kindly Release Booths of this Sector Officer first in order to deactivate record." };

                    }


                }
                else
                {
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Status Can't be Empty" };
                }
            }
            else
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = "The mobile number you entered already exists in the Sector Officer data and cannot be used again. Please provide a different number." };

            }

        }
        public async Task<Response> AddBLOOfficer(BLOMaster bLOMaster)
        {
            var isExist = _context.BLOMaster.Where(d => d.BLOMobile == bLOMaster.BLOMobile && d.StateMasterId == bLOMaster.StateMasterId).FirstOrDefault();
            var isExistCount = _context.BLOMaster.Where(d => d.BLOMobile == bLOMaster.BLOMobile).Count();
            var getPcMasterId = _context.AssemblyMaster.FirstOrDefaultAsync(d => d.AssemblyMasterId == bLOMaster.AssemblyMasterId).Result.PCMasterId;

            var isSoExistMobileNumber = _context.SectorOfficerMaster.Where(d => d.SoMobile == bLOMaster.BLOMobile).FirstOrDefault();
            if (isSoExistMobileNumber == null)
            {
                if (isExistCount > 2)
                {
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "SO User " + bLOMaster.BLOName + " " + "Already Exist more than for two states" };
                }


                if (isExist == null)
                {

                    bLOMaster.BLOCreatedAt = BharatDateTime();
                    bLOMaster.PCMasterId = getPcMasterId;
                    _context.BLOMaster.Add(bLOMaster);
                    _context.SaveChanges();
                    return new Response { Status = RequestStatusEnum.OK, Message = "BLO User " + bLOMaster.BLOName + " " + "Added Successfully" };






                }
                else
                {
                    var existSo = _context.BLOMaster.Where(so => so.BLOMobile == bLOMaster.BLOMobile).FirstOrDefault();
                    var assembly = _context.AssemblyMaster.Where(d => d.AssemblyMasterId == existSo.AssemblyMasterId && d.StateMasterId == existSo.StateMasterId).FirstOrDefault();

                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "BLO User WIth given Mobile Number : " + bLOMaster.BLOMobile + " " + "Already Exists with following Details " + " " + existSo.BLOName + " , " + " AssemblyCode - " + assembly.AssemblyCode + " " + "ARO Assembly Name - " + " " + assembly.AssemblyName };


                }
            }
            else
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = "The mobile number you entered already exists in the Sector Officer data and cannot be used again. Please provide a different number." };
            }
        }
        public async Task<List<CombinedMaster>> GetAssignedBoothListByBLOId(string stateMasterId, string districtMasterId, string assemblyMasterId, string bloId)
        {

            var assignedBLOboothlist = from bt in _context.BoothMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId) && d.AssignedToBLO == bloId)
                                       join asem in _context.AssemblyMaster
                                       on bt.AssemblyMasterId equals asem.AssemblyMasterId
                                       join dist in _context.DistrictMaster
                                       on asem.DistrictMasterId equals dist.DistrictMasterId
                                       join state in _context.StateMaster
                                        on dist.StateMasterId equals state.StateMasterId

                                       select new CombinedMaster
                                       {
                                           StateId = Convert.ToInt32(stateMasterId),
                                           StateName = state.StateName,
                                           DistrictId = dist.DistrictMasterId,
                                           DistrictName = dist.DistrictName,
                                           DistrictCode = dist.DistrictCode,
                                           AssemblyId = asem.AssemblyMasterId,
                                           AssemblyName = asem.AssemblyName,
                                           AssemblyCode = asem.AssemblyCode,
                                           BoothMasterId = bt.BoothMasterId,
                                           BoothName = bt.BoothName,
                                           //BoothAuxy = bt.BoothNoAuxy,
                                           BoothAuxy = (bt.BoothNoAuxy == "0") ? string.Empty : bt.BoothNoAuxy,
                                           IsStatus = bt.BoothStatus,
                                           BoothCode_No = bt.BoothCode_No,

                                           //IsAssigned = bt.IsAssigned,
                                           soMasterId = Convert.ToInt32(bloId)


                                       };
            var count = assignedBLOboothlist.Count();
            return await assignedBLOboothlist.ToListAsync();
        }

        public async Task<List<CombinedMaster>> GetUnassignedBLOBoothListById(string stateMasterId, string districtMasterId, string assemblyMasterId)
        {
            var isStateActive = _context.StateMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId)).FirstOrDefault();
            var isDistrictActive = _context.DistrictMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId)).FirstOrDefault();
            var isAssemblyActive = _context.AssemblyMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId)).FirstOrDefault();
            if (isStateActive.StateStatus && isDistrictActive.DistrictStatus && isAssemblyActive.AssemblyStatus)
            {

                var unAssignedBLOboothlist = from bt in _context.BoothMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId) && d.BoothStatus == true && (d.AssignedToBLO == null || d.AssignedToBLO == string.Empty)) // outer sequenc)
                                             join asem in _context.AssemblyMaster
                                             on bt.AssemblyMasterId equals asem.AssemblyMasterId
                                             join dist in _context.DistrictMaster
                                             on asem.DistrictMasterId equals dist.DistrictMasterId
                                             join state in _context.StateMaster
                                              on dist.StateMasterId equals state.StateMasterId
                                             orderby bt.BoothNoAuxy
                                             select new CombinedMaster
                                             {
                                                 StateId = Convert.ToInt32(stateMasterId),
                                                 DistrictId = dist.DistrictMasterId,
                                                 AssemblyId = asem.AssemblyMasterId,
                                                 AssemblyName = asem.AssemblyName,
                                                 AssemblyCode = asem.AssemblyCode,
                                                 BoothMasterId = bt.BoothMasterId,
                                                 BoothName = bt.BoothName,
                                                 //BoothAuxy = bt.BoothNoAuxy,
                                                 BoothAuxy = (bt.BoothNoAuxy == "0") ? string.Empty : bt.BoothNoAuxy,
                                                 //IsAssigned = bt.IsAssigned,
                                                 IsStatus = bt.BoothStatus,
                                                 BoothCode_No = bt.BoothCode_No


                                             };
                var sortedBoothList = await unAssignedBLOboothlist.ToListAsync();

                // Convert string BoothCode_No to integers for sorting
                sortedBoothList = sortedBoothList.OrderBy(d => int.TryParse(d.BoothCode_No, out int code) ? code : int.MaxValue).ToList();

                return sortedBoothList;
            }
            else
            {
                return null;
            }
        }
        public async Task<Response> BLOBoothMapping(List<BoothMaster> boothMasters)
        {
            string anyBoothLocationFalse = "";
            foreach (var boothMaster in boothMasters)

            {
                var existingBooth = _context.BoothMaster.Where(d =>
                        d.StateMasterId == boothMaster.StateMasterId &&
                        d.DistrictMasterId == boothMaster.DistrictMasterId &&
                        d.AssemblyMasterId == boothMaster.AssemblyMasterId && d.BoothMasterId == boothMaster.BoothMasterId).FirstOrDefault();


                if (existingBooth != null)
                {
                    //if (existingBooth.LocationMasterId > 0)
                    //{
                    // checking booth must b active and should have location
                    if (existingBooth.BoothStatus == true)
                    {


                        var bloExists = _context.BLOMaster.Any(p => p.BLOMasterId == Convert.ToInt32(boothMaster.AssignedToBLO));
                        if (bloExists == true)
                        {

                            // check that booth i
                            // existingBooth.AssignedBy = boothMaster.AssignedBy;
                            existingBooth.AssignedToBLO = boothMaster.AssignedToBLO;
                            existingBooth.AssignedOnTime = DateTime.UtcNow;
                            //existingBooth.IsAssigned = boothMaster.IsAssigned;
                            _context.BoothMaster.Update(existingBooth);
                            _context.SaveChanges();
                        }
                        else
                        {
                            return new Response { Status = RequestStatusEnum.NotFound, Message = "BLO Not Found" };
                        }



                    }
                    else
                    {
                        return new Response { Status = RequestStatusEnum.NotFound, Message = "Booth is Not Active" };

                    }

                }
                else
                {
                    return new Response { Status = RequestStatusEnum.NotFound, Message = "Booth Not Found" };

                }
            }
            if (anyBoothLocationFalse == string.Empty)
            {
                return new Response { Status = RequestStatusEnum.OK, Message = "Booths assigned to BLO successfully!" };
            }
            else
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Following Booth Number Locations is not active :" + anyBoothLocationFalse + " " + "Kindly Active this Booth's Location, or Add Location if not added." };


            }


        }

        public async Task<BLOOfficerCustom> GetBLObyId(string bloMasterId)
        {
            var bloRecord = await _context.BLOMaster
                .Where(d => d.BLOMasterId == Convert.ToInt32(bloMasterId))
                .Select(so => new BLOOfficerCustom
                {
                    StateMasterId = so.StateMasterId,
                    StateName = _context.StateMaster.Where(p => p.StateMasterId == so.StateMasterId).Select(p => p.StateName).FirstOrDefault(),
                    DistrictMasterId = so.DistrictMasterId,
                    DistrictName = _context.DistrictMaster.Where(p => p.DistrictMasterId == so.DistrictMasterId).Select(p => p.DistrictName).FirstOrDefault(),
                    AssemblyName = _context.AssemblyMaster.Where(p => p.AssemblyMasterId == so.AssemblyMasterId).Select(p => p.AssemblyName).FirstOrDefault(),
                    AssemblyMasterId = so.AssemblyMasterId,
                    AssemblyCode = _context.AssemblyMaster.Where(p => p.AssemblyMasterId == so.AssemblyMasterId).Select(p => p.AssemblyCode).FirstOrDefault(),
                    bloName = so.BLOName,
                    bloMobile = so.BLOMobile,
                    bloMasterId = so.BLOMasterId,
                    IsStatus = so.BLOStatus
                })
                .FirstOrDefaultAsync();

            return bloRecord;
        }

        public async Task<Response> ReleaseBoothBLO(BoothMaster boothMaster)
        {
            if (boothMaster.BoothMasterId != null)
            {
                //if (boothMaster.IsAssigned == false)
                //{
                var electionInfoRecord = await _context.ElectionInfoMaster.FirstOrDefaultAsync(e => e.BoothMasterId == boothMaster.BoothMasterId);
                //if (electionInfoRecord == null)

                //{
                var existingbooth = await _context.BoothMaster.FirstOrDefaultAsync(so => so.BoothMasterId == boothMaster.BoothMasterId && so.StateMasterId == boothMaster.StateMasterId && so.DistrictMasterId == so.DistrictMasterId && so.AssemblyMasterId == boothMaster.AssemblyMasterId);
                if (existingbooth == null)
                {
                    return new Response { Status = RequestStatusEnum.NotFound, Message = "Booth Record not found." };
                }
                else
                {
                    //if (existingbooth.IsAssigned == true)
                    //{


                    existingbooth.AssignedToBLO = string.Empty;
                    _context.BoothMaster.Update(existingbooth);
                    await _context.SaveChangesAsync();

                    return new Response { Status = RequestStatusEnum.OK, Message = "Booth " + existingbooth.BoothName.Trim() + " Unassigned successfully for BLO!" };
                    //}
                    //else
                    //{
                    //    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Booth " + existingbooth.BoothName.Trim() + " already Unassigned!" };
                    //}
                }

                //}
                //else
                //{
                //    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Cannot release booth as Event Activity has been performed on it." };
                //}
                //}
                //else
                //{
                //    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Please unassign first!" };


                //}
            }
            else
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Record not found!" };

            }
        }
        #endregion

        #region Randomization

        public async Task<ServiceResponse> AddRandomization(PPR pPR)
        {
            var existingPPR = await _context.PPR.FirstOrDefaultAsync(d => d.PPRMasterId == pPR.PPRMasterId);

            if (existingPPR == null)
            {
                _context.PPR.Add(pPR);
                await _context.SaveChangesAsync();
                return new ServiceResponse { IsSucceed = true, Message = "Added Successfully" };
            }
            else
            {
                existingPPR.StateMasterId = pPR.StateMasterId;
                existingPPR.DistrictMasterId = pPR.DistrictMasterId;
                existingPPR.RandomizationTaskDetailMasterId = pPR.RandomizationTaskDetailMasterId;
                existingPPR.CurrentRound = pPR.CurrentRound;
                existingPPR.DateOfRound = pPR.DateOfRound;
                existingPPR.DateOfCompletedRound = pPR.DateOfCompletedRound;
                existingPPR.DateOfPostponedRound = pPR.DateOfPostponedRound;
                _context.PPR.Update(existingPPR);
                _context.SaveChanges();
                return new ServiceResponse { IsSucceed = true, Message = "Updated Successfully" };
            }
        }

        public async Task<int> GetRoundCountByRandomizationTaskId(int? randomizationTaskId, int? stateMasterId)
        {
            var totalRound = await _context.RandomizationTaskDetail.Where(d => d.RandomizationTaskDetailMasterId == randomizationTaskId && d.StateMasterId == stateMasterId).FirstOrDefaultAsync();
            if (totalRound is not null)
                return totalRound.NumberOfRound;
            else
                return 0;
        }
        public async Task<List<RandomizationList>> GetRandomizationListByStateId(int stateMasterId)
        {
            // Retrieve the state information
            var state = await _context.StateMaster
                                      .Where(s => s.StateMasterId == stateMasterId)
                                      .Select(s => new { s.StateMasterId, s.StateName })
                                      .FirstOrDefaultAsync();

            // Retrieve the district information
            var districtList = await _context.DistrictMaster
                                             .Where(d => d.StateMasterId == stateMasterId)
                                             .ToListAsync();

            // Retrieve the PPR records
            var pprList = await _context.PPR
                                        .Where(p => p.StateMasterId == stateMasterId)
                                        .ToListAsync();

            // Retrieve the task details
            var taskList = await _context.RandomizationTaskDetail
                                         .Where(t => t.StateMasterId == stateMasterId)
                                         .ToListAsync();

            // Create the list of RandomizationList objects
            var randomizationList = pprList.Select(ppr => new RandomizationList
            {
                PPRMasterId = ppr.PPRMasterId,
                StateMasterId = state.StateMasterId,
                StateName = state.StateName,
                DistrictMasterId = ppr.DistrictMasterId ?? 0, // Default to 0 if no district found
                DistrictName = districtList.FirstOrDefault(d => d.DistrictMasterId == ppr.DistrictMasterId)?.DistrictName ?? "Unknown", // Default to "Unknown" if no district found
                TaskId = ppr.RandomizationTaskDetailMasterId,
                TaskName = taskList.FirstOrDefault(t => t.RandomizationTaskDetailMasterId == ppr.RandomizationTaskDetailMasterId)?.TaskName ?? "Unknown", // Default to "Unknown" if no task found
                TotalRound = taskList.FirstOrDefault(t => t.RandomizationTaskDetailMasterId == ppr.RandomizationTaskDetailMasterId)?.NumberOfRound ?? 0, // Default to "Unknown" if no task found
                RoundNumber = ppr.CurrentRound,
                StartDate = ppr.DateOfRound,
                EndDate = ppr.DateOfCompletedRound,
                PostponedDate = ppr.DateOfPostponedRound
            }).ToList();

            return randomizationList;
        }
        public async Task<RandomizationList> GetRandomizationById(int pprMasterId)
        {
            var pprRecord = await _context.PPR.FirstOrDefaultAsync(d => d.PPRMasterId == pprMasterId);
            // Retrieve the task details
            var task = await _context.RandomizationTaskDetail
                          .Where(t => t.RandomizationTaskDetailMasterId == pprRecord.RandomizationTaskDetailMasterId)
                          .Select(t => new { t.TaskName, t.RandomizationTaskDetailMasterId, t.NumberOfRound })
                          .FirstOrDefaultAsync();
            var randomization = new RandomizationList
            {

                PPRMasterId = pprRecord.PPRMasterId,
                StateMasterId = pprRecord.StateMasterId,
                DistrictMasterId = pprRecord.DistrictMasterId,
                TaskId = task?.RandomizationTaskDetailMasterId ?? 0,
                TaskName = task?.TaskName ?? "Unknown",
                TotalRound = task?.NumberOfRound ?? 0,
                RoundNumber = pprRecord.CurrentRound,
                StartDate = pprRecord.DateOfRound,
                EndDate = pprRecord.DateOfCompletedRound,
                PostponedDate = pprRecord.DateOfPostponedRound
            };

            return randomization;
        }
        public async Task<List<RandomizationTableList>> GetRandomizationTableListByStateId(int stateMasterId)
        {
            var recordOfRounds = await _context.PPR
                .Where(d => d.StateMasterId == stateMasterId)
                .ToListAsync();
            var randomizationTasks = await _context.RandomizationTaskDetail.ToListAsync();
            var state = await _context.StateMaster.FirstOrDefaultAsync(d => d.StateMasterId == stateMasterId);
            var districtList = await _context.DistrictMaster.Where(d => d.StateMasterId == stateMasterId).ToListAsync();

            var groupedTasks = recordOfRounds
                .GroupBy(p => new { p.DistrictMasterId, p.RandomizationTaskDetailMasterId })
                .GroupBy(g => g.Key.DistrictMasterId)
                .Select(districtGroup => new RandomizationTableList
                {
                    StateMasterId = stateMasterId,
                    StateName = state?.StateName, // Check if state is not null
                    DistrictMasterId = districtGroup.Key.Value, // Ensure DistrictMasterId is not null
                    DistrictName = districtList.FirstOrDefault(d => d.StateMasterId == stateMasterId && d.DistrictMasterId == districtGroup.Key.Value)?.DistrictName, // Use FirstOrDefault and check for null
                    Tasks = districtGroup
                        .Select(taskGroup => new
                        {
                            TaskName = randomizationTasks.FirstOrDefault(t => t.RandomizationTaskDetailMasterId == taskGroup.Key.RandomizationTaskDetailMasterId)?.TaskName, // Use FirstOrDefault and check for null
                            Rounds = taskGroup.Select(p => new RoundDetails
                            {
                                RoundNumber = p.CurrentRound,
                                StartDate = p.DateOfRound,
                                EndDate = p.DateOfCompletedRound,
                                PostponedDate = p.DateOfPostponedRound
                            }).ToList()
                        })
                        .Where(t => t.TaskName != null) // Ensure TaskName is not null before proceeding
                        .GroupBy(x => x.TaskName) // Group by TaskName to get latest task
                        .Select(g => g.First()) // Select only the first (latest) task with its rounds
                        .Select(t => new RandomizationTaskRounds
                        {
                            TaskName = t.TaskName,
                            Rounds = t.Rounds
                        })
                        .ToList()
                }).ToList();

            return groupedTasks;
        }


        public async Task<RandomizationTableList> GetRandomizationListByDistrictId(int stateMasterId, int districtMasterId)
        {
            var recordOfRounds = await _context.PPR
                .Where(d => d.StateMasterId == stateMasterId && d.DistrictMasterId == districtMasterId)
                .ToListAsync();

            var randomizationTasks = await _context.RandomizationTaskDetail.ToListAsync();
            var state = await _context.StateMaster.FirstOrDefaultAsync(d => d.StateMasterId == stateMasterId);
            var district = await _context.DistrictMaster.FirstOrDefaultAsync(d => d.StateMasterId == stateMasterId && d.DistrictMasterId == districtMasterId);
            var taskGroups = recordOfRounds
                .GroupBy(p => p.RandomizationTaskDetailMasterId)
                .Select(group => new RandomizationTaskRounds
                {
                    TaskName = randomizationTasks.First(t => t.RandomizationTaskDetailMasterId == group.Key).TaskName,
                    Rounds = group.Select(p => new RoundDetails
                    {
                        RoundNumber = p.CurrentRound,
                        StartDate = p.DateOfRound,
                        EndDate = p.DateOfCompletedRound,
                        PostponedDate = p.DateOfPostponedRound
                    }).ToList()
                }).ToList();

            return new RandomizationTableList
            {
                StateMasterId = stateMasterId,
                StateName = state.StateName,
                DistrictMasterId = districtMasterId,
                DistrictName = district.DistrictName,
                Tasks = taskGroups
            };
        }


        public async Task<ServiceResponse> AddRandomizationTaskDetail(RandomizationTaskDetail randomizationTaskDetail)
        {
            var existingTaskDetail = await _context.RandomizationTaskDetail
                .FirstOrDefaultAsync(d => d.StateMasterId == randomizationTaskDetail.StateMasterId
                                          && d.TaskName == randomizationTaskDetail.TaskName);

            if (existingTaskDetail != null)
            {
                return new ServiceResponse { IsSucceed = false, Message = "Task with the same name already exists for this state" };
            }
            else
            {
                _context.RandomizationTaskDetail.Add(randomizationTaskDetail);
                await _context.SaveChangesAsync();
                return new ServiceResponse { IsSucceed = true, Message = "Added Successfully" };
            }
        }
        public async Task<int> GetCurrentRoundByRandomizationById(int? stateMasterId, int? districtMasterId, int? randomizationTaskDetailMasterId)
        {
            // Ensure all parameters are not null
            if (stateMasterId == null || districtMasterId == null || randomizationTaskDetailMasterId == null)
            {
                throw new ArgumentNullException("One or more parameters are null");
            }

            // Query to get the current round
            var currentRound = await _context.PPR
                .Where(d => d.StateMasterId == stateMasterId &&
                            d.DistrictMasterId == districtMasterId &&
                            d.RandomizationTaskDetailMasterId == randomizationTaskDetailMasterId)
                .OrderByDescending(d => d.DateOfRound)
                .Select(d => d.CurrentRound)
                .FirstOrDefaultAsync(); // Use FirstOrDefaultAsync to get the single integer result

            return currentRound;
        }

        public async Task<List<RandomizationTaskDetail>> GetRandomizationTaskListByStateId(int stateMasterId)
        {
            return await _context.RandomizationTaskDetail.Where(d => d.StateMasterId == stateMasterId).ToListAsync();
        }


        public async Task<ServiceResponse> UpdateRandomizationById(PPR ppr)
        {
            var record = await _context.PPR.FirstOrDefaultAsync(d => d.PPRMasterId == ppr.PPRMasterId);

            if (record == null)
            {
                return new ServiceResponse { IsSucceed = false, Message = "Record not found" };
            }

            record.StateMasterId = ppr.StateMasterId;
            record.DistrictMasterId = ppr.DistrictMasterId;
            record.RandomizationTaskDetailMasterId = ppr.RandomizationTaskDetailMasterId;
            record.CurrentRound = ppr.CurrentRound;
            record.DateOfRound = ppr.DateOfRound;
            record.DateOfCompletedRound = ppr.DateOfCompletedRound;
            record.DateOfPostponedRound = ppr.DateOfPostponedRound;

            _context.PPR.Update(record);
            await _context.SaveChangesAsync();

            return new ServiceResponse { IsSucceed = true, Message = "Record updated successfully" };
        }

        #endregion



        #region GetBoothByLocation

        public async Task<List<CombinedMaster>> GetBoothByLocation(string latitude, string longitude)
        {
            List<CombinedMaster> combinedMasters = new List<CombinedMaster>();

            // Establish a connection to the PostgreSQL database
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("Postgres"));
            await connection.OpenAsync();

            // Prepare and execute the PostgreSQL function call
            await using var cmd = new NpgsqlCommand("SELECT * FROM getboothbylocation(@latitude::double precision, @longitude::double precision)", connection);
            cmd.Parameters.AddWithValue("@latitude", latitude);
            cmd.Parameters.AddWithValue("@longitude", longitude);

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                CombinedMaster combinedMaster = new CombinedMaster
                {
                    StateId = reader.GetInt32(0),
                    DistrictId = reader.GetInt32(1),
                    AssemblyId = reader.GetInt32(2),
                    AssemblyName = reader.GetString(3),
                    AssemblyCode = reader.GetInt32(4),
                    BoothMasterId = reader.GetInt32(5),
                    BoothName = reader.GetString(6),
                    SecondLanguage = reader.IsDBNull(7) ? null : reader.GetString(7),
                    BoothAuxy = reader.IsDBNull(8) ? null : reader.GetString(8),
                    BoothCode_No = reader.GetString(9),
                    IsAssigned = reader.GetBoolean(10),
                    IsStatus = reader.GetBoolean(11),
                    LocationMasterId = reader.IsDBNull(12) ? null : (int?)reader.GetInt32(12)
                };

                combinedMasters.Add(combinedMaster);
            }

            return combinedMasters;
        }


        #endregion



        #region BLOCount
        public async Task<List<BLOBoothAssignedQueueCount>> GetBLOQueueCount(BoothReportModel boothReportModel)
        {
            List<BLOBoothAssignedQueueCount> list = new List<BLOBoothAssignedQueueCount>();

            var state = _context.StateMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.StateStatus == true).Select(d => new { d.StateName, d.StateCode }).FirstOrDefault();
            var district = new { DistrictName = "", DistrictCode = "" };
            var pcMaster = new { PcName = "", PcCodeNo = "" };

            if (boothReportModel.DistrictMasterId is not 0 && boothReportModel.StateMasterId is not 0)
            {
                district = _context.DistrictMaster.Where(d => d.DistrictMasterId == boothReportModel.DistrictMasterId && d.StateMasterId == boothReportModel.StateMasterId && d.DistrictStatus == true).Select(d => new { d.DistrictName, d.DistrictCode }).FirstOrDefault();
            }


            // Establish a connection to the PostgreSQL database
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("Postgres"));
            await connection.OpenAsync();

            // Create a NpgsqlCommand object to execute the function
            var command = new NpgsqlCommand("SELECT * FROM find_assigned_blos_assembly_wise_QIS_Count(@state_id, @district_id)", connection);
            command.Parameters.AddWithValue("@state_id", Convert.ToInt32(boothReportModel.StateMasterId));
            command.Parameters.AddWithValue("@district_id", Convert.ToInt32(boothReportModel.DistrictMasterId));
            // Execute the command and read the results
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                // Create a new EventActivityWiseBooth object and populate its properties from the reader


                var bLOBoothAssignedQueueCount = new BLOBoothAssignedQueueCount
                {

                    Header = $"{state.StateName}({state.StateCode})",
                    Title = $"{state.StateName},({district.DistrictName})",
                    Type = "ACWisebyDistrict",

                    AssemblyName = reader.GetString(0),
                    BLOMasterId = reader.GetInt32(1),
                    BLOName = reader.GetString(2),
                    BLOMobile = reader.GetString(3),
                    //BoothName = reader.GetString(5), it returns comma seprtd
                    QueueCount = reader.IsDBNull(6) ? null : reader.GetInt32(6),
                    LastQueueEnterDateTime = reader.IsDBNull(8) ? null : reader.GetString(8),
                    BoothName = reader.GetString(9)
                };

                // Add the object to the list
                list.Add(bLOBoothAssignedQueueCount);
            }

            // Return the list of EventActivityWiseBooth objects
            return list;






            return list;
        }


        public async Task<List<BLOBoothAssignedQueueCount>> GetUnassignedBLOs(BoothReportModel boothReportModel)
        {
            List<BLOBoothAssignedQueueCount> list = new List<BLOBoothAssignedQueueCount>();

            var state = _context.StateMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.StateStatus == true).Select(d => new { d.StateName, d.StateCode }).FirstOrDefault();
            var district = new { DistrictName = "", DistrictCode = "" };
            var pcMaster = new { PcName = "", PcCodeNo = "" };

            if (boothReportModel.DistrictMasterId is not 0 && boothReportModel.StateMasterId is not 0)
            {
                district = _context.DistrictMaster.Where(d => d.DistrictMasterId == boothReportModel.DistrictMasterId && d.StateMasterId == boothReportModel.StateMasterId && d.DistrictStatus == true).Select(d => new { d.DistrictName, d.DistrictCode }).FirstOrDefault();
            }


            // Establish a connection to the PostgreSQL database
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("Postgres"));
            await connection.OpenAsync();

            // Create a NpgsqlCommand object to execute the function
            var command = new NpgsqlCommand("SELECT * FROM find_unassigned_blos_assembly_wise(@state_id, @district_id)", connection);
            command.Parameters.AddWithValue("@state_id", Convert.ToInt32(boothReportModel.StateMasterId));
            command.Parameters.AddWithValue("@district_id", Convert.ToInt32(boothReportModel.DistrictMasterId));
            // Execute the command and read the results
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                // Create a new EventActivityWiseBooth object and populate its properties from the reader
                var bLOBoothAssignedQueueCount = new BLOBoothAssignedQueueCount
                {

                    Header = $"{state.StateName}({state.StateCode})",
                    Title = $"{state.StateName},({district.DistrictName})",
                    Type = "ACWisebyDistrict",

                    AssemblyName = reader.GetString(0),
                    BLOMasterId = reader.GetInt32(1),
                    BLOName = reader.GetString(2),
                    BLOMobile = reader.GetString(3)




                };

                // Add the object to the list
                list.Add(bLOBoothAssignedQueueCount);
            }

            // Return the list of EventActivityWiseBooth objects
            return list;






            return list;
        }
        public async Task<List<BLOBoothAssignedQueueCount>> GetAssignedBLOs(BoothReportModel boothReportModel)
        {
            List<BLOBoothAssignedQueueCount> list = new List<BLOBoothAssignedQueueCount>();

            var state = _context.StateMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.StateStatus == true).Select(d => new { d.StateName, d.StateCode }).FirstOrDefault();
            var district = new { DistrictName = "", DistrictCode = "" };
            var pcMaster = new { PcName = "", PcCodeNo = "" };

            if (boothReportModel.DistrictMasterId is not 0 && boothReportModel.StateMasterId is not 0)
            {
                district = _context.DistrictMaster.Where(d => d.DistrictMasterId == boothReportModel.DistrictMasterId && d.StateMasterId == boothReportModel.StateMasterId && d.DistrictStatus == true).Select(d => new { d.DistrictName, d.DistrictCode }).FirstOrDefault();
            }


            // Establish a connection to the PostgreSQL database
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("Postgres"));
            await connection.OpenAsync();

            // Create a NpgsqlCommand object to execute the function
            var command = new NpgsqlCommand("SELECT * FROM find_assigned_blos_assembly_wise_qis_count(@state_id, @district_id)", connection);
            command.Parameters.AddWithValue("@state_id", Convert.ToInt32(boothReportModel.StateMasterId));
            command.Parameters.AddWithValue("@district_id", Convert.ToInt32(boothReportModel.DistrictMasterId));
            // Execute the command and read the results
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                // Create a new EventActivityWiseBooth object and populate its properties from the reader
                var bLOBoothAssignedQueueCount = new BLOBoothAssignedQueueCount
                {

                    Header = $"{state.StateName}({state.StateCode})",
                    Title = $"{state.StateName},({district.DistrictName})",
                    Type = "ACWisebyDistrict",
                    AssemblyName = reader.GetString(0),
                    BLOMasterId = reader.GetInt32(1),
                    BLOName = reader.GetString(2),
                    BLOMobile = reader.GetString(3)




                };

                // Add the object to the list
                list.Add(bLOBoothAssignedQueueCount);
            }

            // Return the list of EventActivityWiseBooth objects
            return list;






            return list;
        }




        public async Task<List<BLOBoothAssignedQueueCount>> GetBLOQueueCountOpen(string statemasterid, string districtmasterid)
        {
            List<BLOBoothAssignedQueueCount> list = new List<BLOBoothAssignedQueueCount>();

            var state = _context.StateMaster.Where(d => d.StateMasterId == Convert.ToInt32(statemasterid) && d.StateStatus == true).Select(d => new { d.StateName, d.StateCode }).FirstOrDefault();
            var district = new { DistrictName = "", DistrictCode = "" };
            var pcMaster = new { PcName = "", PcCodeNo = "" };

            if (statemasterid != "0" && districtmasterid != "0")
            {
                district = _context.DistrictMaster.Where(d => d.DistrictMasterId == Convert.ToInt32(districtmasterid) && d.StateMasterId == Convert.ToInt32(statemasterid) && d.DistrictStatus == true).Select(d => new { d.DistrictName, d.DistrictCode }).FirstOrDefault();
            }


            // Establish a connection to the PostgreSQL database
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("Postgres"));
            await connection.OpenAsync();

            // Create a NpgsqlCommand object to execute the function
            var command = new NpgsqlCommand("SELECT * FROM find_assigned_blos_assembly_wise_QIS_Count(@state_id, @district_id)", connection);
            command.Parameters.AddWithValue("@state_id", Convert.ToInt32(statemasterid));
            command.Parameters.AddWithValue("@district_id", Convert.ToInt32(districtmasterid));
            // Execute the command and read the results
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                // Create a new EventActivityWiseBooth object and populate its properties from the reader


                var bLOBoothAssignedQueueCount = new BLOBoothAssignedQueueCount
                {

                    Header = $"{state.StateName}({state.StateCode})",
                    Title = $"{state.StateName},({district.DistrictName})",
                    Type = "ACWisebyDistrict",

                    AssemblyName = reader.GetString(0),
                    BLOMasterId = reader.GetInt32(1),
                    BLOName = reader.GetString(2),
                    BLOMobile = reader.GetString(3),
                    //BoothName = reader.GetString(5), it returns comma seprtd
                    QueueCount = reader.IsDBNull(6) ? null : reader.GetInt32(6),
                    LastQueueEnterDateTime = reader.IsDBNull(8) ? null : reader.GetString(8),
                    BoothName = reader.GetString(9)
                };

                // Add the object to the list
                list.Add(bLOBoothAssignedQueueCount);
            }

            // Return the list of EventActivityWiseBooth objects
            return list;






            return list;
        }
        #endregion


        #region MobileVersion
        public async Task<MobileVersion> GetMobileVersionById(string StateMasterId)
        {
            var mobileVersionRecord = await _context.MobileVersion.OrderByDescending(d => d.MobileVersionId).FirstOrDefaultAsync(d => d.StateMasterId == Convert.ToInt32(StateMasterId));

            return mobileVersionRecord;
        }

        public async Task<ServiceResponse> AddMobileVersion(MobileVersion mobileVersion)
        {
            try
            {

                // Add the mobile version to the context and save changes
                _context.MobileVersion.Add(mobileVersion);
                await _context.SaveChangesAsync();

                return new ServiceResponse { IsSucceed = true, Message = "Mobile version added successfully" };

            }
            catch (Exception ex)
            {
                return new ServiceResponse { IsSucceed = false, Message = "Not added " };
            }
        }

        #endregion

        #region KYC Public Details
        public async Task<ServiceResponse> AddKYCDetails(Kyc kyc)
        {
            _context.Kyc.Add(kyc);
            _context.SaveChanges();


            return new ServiceResponse { IsSucceed = true, Message = "Successfully added" };
        }
        public async Task<List<Kyc>> GetKYCDetails()
        {
            return await _context.Kyc.ToListAsync();
        }
        #endregion

        #region Election Type Master
        public async Task<List<ElectionTypeMaster>> GetAllElectionTypes()
        {

            var elecTypeData = await _context.ElectionTypeMaster.OrderBy(d => d.ElectionTypeMasterId)
    .Select(d => new ElectionTypeMaster
    {
        ElectionTypeMasterId = d.ElectionTypeMasterId,
        ElectionType = d.ElectionType,
        ElectionStatus = d.ElectionStatus,
        Hierarchy1 = d.Hierarchy1,
        Hierarchy2 = d.Hierarchy2

    })
    .ToListAsync();

            return elecTypeData;
        }
        public async Task<ElectionTypeMaster> GetElectionTypeById(string electionTypeId)
        {
            var electionTypeRecord = await _context.ElectionTypeMaster.Where(d => d.ElectionTypeMasterId == Convert.ToInt32(electionTypeId)).FirstOrDefaultAsync();

            return electionTypeRecord;
        }
        #endregion

        #region PSZone
        public async Task<Response> AddPSZone(PSZone pSZone)
        {
            try
            {
                var ispsZoneExist = await _context.PSZone.Where(p => p.PSZoneCode == pSZone.PSZoneCode && p.StateMasterId == pSZone.StateMasterId && p.DistrictMasterId == pSZone.DistrictMasterId && p.AssemblyMasterId == pSZone.AssemblyMasterId && p.ElectionTypeMasterId == pSZone.ElectionTypeMasterId).FirstOrDefaultAsync();

                if (ispsZoneExist == null)
                {

                    pSZone.PSZoneCreatedAt = BharatDateTime();
                    _context.PSZone.Add(pSZone);
                    _context.SaveChanges();

                    return new Response { Status = RequestStatusEnum.OK, Message = pSZone.PSZoneName + "Added Successfully" };



                }
                else
                {
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = ispsZoneExist.PSZoneName + "Same PS Zone Code Already Exists in the selected Election Type" };

                }

            }

            catch (Exception ex)
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = ex.Message };
            }

        }
        public async Task<List<PSZone>> GetPSZoneListById(int stateMasterId, int districtMasterId, int assemblyMasterId)
        {
            var getPsZone = await _context.PSZone.Where(d => d.StateMasterId == stateMasterId && d.DistrictMasterId == districtMasterId && d.AssemblyMasterId == assemblyMasterId).ToListAsync();
            if (getPsZone != null)
            {
                return getPsZone;
            }
            else
            {
                return null;
            }
        }
        public async Task<Response> UpdatePSZone(PSZone pSZone)
        {
            // Check if the PSZone entity exists in the database
            var existingPsZone = await _context.PSZone
                .Where(d => d.PSZoneMasterId == pSZone.PSZoneMasterId)
                .FirstOrDefaultAsync();

            if (existingPsZone == null)
            {
                return new Response
                {
                    Status = RequestStatusEnum.BadRequest,
                    Message = "PSZone not found."
                };
            }

            // Update the properties of the existing entity
            existingPsZone.PSZoneName = pSZone.PSZoneName;
            existingPsZone.PSZoneCode = pSZone.PSZoneCode;
            existingPsZone.PSZoneType = pSZone.PSZoneType;
            existingPsZone.ElectionTypeMasterId = pSZone.ElectionTypeMasterId;
            existingPsZone.StateMasterId = pSZone.StateMasterId;
            existingPsZone.DistrictMasterId = pSZone.DistrictMasterId;
            existingPsZone.AssemblyMasterId = pSZone.AssemblyMasterId;
            existingPsZone.PSZoneBooths = pSZone.PSZoneBooths;
            existingPsZone.PSZoneCategory = pSZone.PSZoneCategory;
            existingPsZone.SecondLanguage = pSZone.SecondLanguage;
            existingPsZone.PSZoneUpdatedAt = DateTime.UtcNow;
            existingPsZone.PSZoneStatus = pSZone.PSZoneStatus;

            // Save changes to the database
            try
            {
                await _context.SaveChangesAsync();
                return new Response
                {
                    Status = RequestStatusEnum.OK,
                    Message = "PSZone updated successfully."
                };
            }
            catch (Exception ex)
            {
                // Handle any errors that may have occurred
                return new Response
                {
                    Status = RequestStatusEnum.BadRequest,
                    Message = $"An error occurred: {ex.Message}"
                };
            }
        }
        public async Task<PSZone> GetPSZoneById(int stateMasterId, int districtMasterId, int assemblyMasterId, int pSZoneMasterId)
        {


            // Retrieve the PSZone entity from the database
            var psZone = await _context.PSZone
                .Where(d => d.StateMasterId == stateMasterId &&
                            d.DistrictMasterId == districtMasterId &&
                            d.AssemblyMasterId == assemblyMasterId &&
                            d.PSZoneMasterId == pSZoneMasterId)
                .FirstOrDefaultAsync();

            // Check if the PSZone entity exists
            if (psZone == null)
            {

                return null;
            }


            return psZone;
        }
        public async Task<Response> DeletePSZoneById(int stateMasterId, int districtMasterId, int assemblyMasterId, int pSZoneMasterId)
        {
            // Validate the input ID
            if (pSZoneMasterId <= 0)
            {
                return new Response
                {
                    Status = RequestStatusEnum.BadRequest,
                    Message = "Invalid PSZoneMasterId provided."
                };
            }

            // Check if the PSZone entity exists in the database
            var psZone = await _context.PSZone
                .Where(d => d.PSZoneMasterId == pSZoneMasterId && d.StateMasterId == stateMasterId && d.DistrictMasterId == districtMasterId && d.AssemblyMasterId == assemblyMasterId)
                .FirstOrDefaultAsync();

            if (psZone == null)
            {
                return new Response
                {
                    Status = RequestStatusEnum.BadRequest,
                    Message = "PSZone not found."
                };
            }

            // Perform the deletion
            try
            {
                _context.PSZone.Remove(psZone);
                await _context.SaveChangesAsync();

                return new Response
                {
                    Status = RequestStatusEnum.OK,
                    Message = "PSZone deleted successfully."
                };
            }
            catch (Exception ex)
            {
                // Handle any errors that may have occurred during deletion
                return new Response
                {
                    Status = RequestStatusEnum.BadRequest,
                    Message = $"An error occurred: {ex.Message}"
                };
            }
        }

        #endregion

        #region SarpanchWards
        public async Task<Response> AddSarpanchWards(SarpanchWards sarpanchWards)
        {
            try
            {
                var ispsZoneExist = await _context.SarpanchWards.Where(p => p.SarpanchWardsCode == sarpanchWards.SarpanchWardsCode && p.StateMasterId == sarpanchWards.StateMasterId && p.DistrictMasterId == sarpanchWards.DistrictMasterId && p.AssemblyMasterId == sarpanchWards.AssemblyMasterId && p.ElectionTypeMasterId == sarpanchWards.ElectionTypeMasterId).FirstOrDefaultAsync();

                if (ispsZoneExist == null)
                {

                    sarpanchWards.SarpanchWardsCreatedAt = BharatDateTime();
                    _context.SarpanchWards.Add(sarpanchWards);
                    _context.SaveChanges();

                    return new Response { Status = RequestStatusEnum.OK, Message = sarpanchWards.SarpanchWardsName + "Added Successfully" };



                }
                else
                {
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = ispsZoneExist.SarpanchWardsName + "Same PS Zone Code Already Exists in the selected Election Type" };

                }

            }

            catch (Exception ex)
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = ex.Message };
            }
        }
        public async Task<List<SarpanchWards>> GetSarpanchWardsListById(int stateMasterId, int districtMasterId, int assemblyMasterId, int boothMasterId)
        {
            var getPsZone = await _context.SarpanchWards.Where(d => d.StateMasterId == stateMasterId && d.DistrictMasterId == districtMasterId && d.AssemblyMasterId == assemblyMasterId && d.BoothMasterId == boothMasterId).ToListAsync();
            if (getPsZone != null)
            {
                return getPsZone;
            }
            else
            {
                return null;
            }
        }
        public async Task<Response> UpdateSarpanchWards(SarpanchWards sarpanchWards)
        {
            // Check if the SarpanchWards entity exists in the database
            var existingSarpanchWards = await _context.SarpanchWards
                .Where(d => d.SarpanchWardsMasterId == sarpanchWards.SarpanchWardsMasterId)
                .FirstOrDefaultAsync();

            if (existingSarpanchWards == null)
            {
                return new Response
                {
                    Status = RequestStatusEnum.BadRequest,
                    Message = "Wards not found."
                };
            }

            // Update the properties of the existing entity
            existingSarpanchWards.SarpanchWardsName = sarpanchWards.SarpanchWardsName;
            existingSarpanchWards.SarpanchWardsCode = sarpanchWards.SarpanchWardsCode;
            existingSarpanchWards.SarpanchWardsType = sarpanchWards.SarpanchWardsType;
            existingSarpanchWards.ElectionTypeMasterId = sarpanchWards.ElectionTypeMasterId;
            existingSarpanchWards.StateMasterId = sarpanchWards.StateMasterId;
            existingSarpanchWards.DistrictMasterId = sarpanchWards.DistrictMasterId;
            existingSarpanchWards.AssemblyMasterId = sarpanchWards.AssemblyMasterId;
            existingSarpanchWards.BoothMasterId = sarpanchWards.BoothMasterId;
            existingSarpanchWards.SarpanchWardsCategory = sarpanchWards.SarpanchWardsCategory;
            existingSarpanchWards.SarpanchWardsUpdatedAt = DateTime.UtcNow;
            existingSarpanchWards.SarpanchWardsDeletedAt = sarpanchWards.SarpanchWardsDeletedAt;
            existingSarpanchWards.SarpanchWardsStatus = sarpanchWards.SarpanchWardsStatus;
            existingSarpanchWards.SecondLanguage = sarpanchWards.SecondLanguage;

            // Save changes to the database
            try
            {
                await _context.SaveChangesAsync();
                return new Response
                {
                    Status = RequestStatusEnum.OK,
                    Message = "Sarpanch Wards updated successfully."
                };
            }
            catch (Exception ex)
            {
                // Handle any errors that may have occurred
                return new Response
                {
                    Status = RequestStatusEnum.BadRequest,
                    Message = $"An error occurred: {ex.Message}"
                };
            }
        }
        public async Task<SarpanchWards> GetSarpanchWardsById(int stateMasterId, int districtMasterId, int assemblyMasterId, int boothMasterId, int wardsMasterId)
        {
            var sarpanchWards = await _context.SarpanchWards
                .Where(w => w.StateMasterId == stateMasterId &&
                            w.DistrictMasterId == districtMasterId &&
                            w.AssemblyMasterId == assemblyMasterId &&
                            w.BoothMasterId == boothMasterId &&
                            w.SarpanchWardsMasterId == wardsMasterId)
                .FirstOrDefaultAsync();

            if (sarpanchWards == null)
            {
                return null;
            }

            return sarpanchWards;
        }


        public async Task<Response> DeleteSarpanchWardsById(int stateMasterId, int districtMasterId, int assemblyMasterId, int boothMasterId, int wardsMasterId)
        {
            var sarpanchWards = await _context.SarpanchWards
                .Where(w => w.StateMasterId == stateMasterId &&
                            w.DistrictMasterId == districtMasterId &&
                            w.AssemblyMasterId == assemblyMasterId &&
                            w.BoothMasterId == boothMasterId &&
                            w.SarpanchWardsMasterId == wardsMasterId)
                .FirstOrDefaultAsync();

            if (sarpanchWards == null)
            {
                return new Response
                {
                    Status = RequestStatusEnum.BadRequest,
                    Message = "Sarpanch Ward not found."
                };
            }

            _context.SarpanchWards.Remove(sarpanchWards);

            try
            {
                await _context.SaveChangesAsync();
                return new Response
                {
                    Status = RequestStatusEnum.OK,
                    Message = "Sarpanch Ward deleted successfully."
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Status = RequestStatusEnum.BadRequest,
                    Message = $"An error occurred: {ex.Message}"
                };
            }
        }

        #endregion
    }
}
