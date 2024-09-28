using EAMS.Helper;
using EAMS.ViewModels.PSFormViewModel;
using EAMS_ACore;
using EAMS_ACore.HelperModels;
using EAMS_ACore.IAuthRepository;
using EAMS_ACore.IExternal;
using EAMS_ACore.IRepository;
using EAMS_ACore.Models;
using EAMS_ACore.Models.BLOModels;
using EAMS_ACore.Models.ElectionType;
using EAMS_ACore.Models.EventActivityModels;
using EAMS_ACore.Models.Polling_Personal_Randomisation_Models;
using EAMS_ACore.Models.Polling_Personal_Randomization_Models;
using EAMS_ACore.Models.PollingStationFormModels;
using EAMS_ACore.Models.PublicModels;
using EAMS_ACore.Models.QueueModel;
using EAMS_ACore.ReportModels;
using EAMS_ACore.SignalRModels;
using EAMS_DAL.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Claims;


namespace EAMS_DAL.Repository
{

    public class EamsRepository : IEamsRepository
    {
        private readonly EamsContext _context;
        private readonly IAuthRepository _authRepository;
        private readonly ILogger<EamsRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly ICacheService _cacheService;
        public EamsRepository(EamsContext context, IAuthRepository authRepository, ILogger<EamsRepository> logger,
            IConfiguration configuration, ICacheService cacheService)
        {
            _context = context;
            _authRepository = authRepository;
            _logger = logger;
            _configuration = configuration;
            _cacheService = cacheService;
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

                case "FOMaster":
                    var isFOExist = await _context.FieldOfficerMaster.Where(d => d.FieldOfficerMasterId == Convert.ToInt32(updateMasterStatus.Id)).FirstOrDefaultAsync();
                    if (isFOExist != null)
                    {
                        isFOExist.FieldOfficerStatus = updateMasterStatus.IsStatus;
                        _context.FieldOfficerMaster.Update(isFOExist);
                        _context.SaveChanges();
                        string foMessage = isFOExist.FieldOfficerStatus ? "FO Activated Successfully" : "FO Deactivated Successfully";
                        return new ServiceResponse { IsSucceed = true, Message = foMessage };
                    }
                    else
                    {
                        return new ServiceResponse { IsSucceed = false, Message = "Field Officer Record Not Found." };
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
                            if (isBoothExist.AssignedTo == null || String.IsNullOrEmpty(isBoothExist.AssignedTo))
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
                                        return new ServiceResponse { IsSucceed = true, Message = "Booth Activated Successfully." };

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

                case "Level4H":
                    var level4 = await _context.FourthLevelH
                        .Where(d => d.FourthLevelHMasterId == Convert.ToInt32(updateMasterStatus.Id))
                        .FirstOrDefaultAsync();

                    if (level4 == null)
                    {
                        return new ServiceResponse { IsSucceed = false, Message = "Record Not Found." };
                    }

                    if (updateMasterStatus.IsStatus == false)
                    {
                        var blockZonePanchayatCount = await _context.PSZonePanchayat
                          .Where(d => d.FourthLevelHMasterId == level4.FourthLevelHMasterId).CountAsync();

                        if (blockZonePanchayatCount != 0)
                        {
                            return new ServiceResponse { IsSucceed = false, Message = $"Panchayats {blockZonePanchayatCount} are active under this {level4.HierarchyName}. Make sure they are Inactive first" };
                        }
                    }

                    level4.HierarchyStatus = updateMasterStatus.IsStatus;
                    _context.FourthLevelH.Update(level4);
                    await _context.SaveChangesAsync();

                    string message = level4.HierarchyStatus ? "Activated Successfully" : "Deactivated Successfully";
                    return new ServiceResponse { IsSucceed = true, Message = message };

                case "BlockZonePanchayat":
                    var blockZonePanchayat = await _context.PSZonePanchayat
                        .Where(d => d.PSZonePanchayatMasterId == Convert.ToInt32(updateMasterStatus.Id)).Include(d => d.ElectionTypeMaster)
                        .FirstOrDefaultAsync();

                    if (blockZonePanchayat == null)
                    {
                        return new ServiceResponse { IsSucceed = false, Message = "Record Not Found." };
                    }

                    if (updateMasterStatus.IsStatus == false)
                    {
                        var spWardCount = await _context.GPPanchayatWards
                            .Where(d => d.FourthLevelHMasterId == blockZonePanchayat.FourthLevelHMasterId && d.GPPanchayatWardsStatus == true).CountAsync();

                        var boothCount = await _context.BoothMaster
                          .Where(d => d.FourthLevelHMasterId == blockZonePanchayat.FourthLevelHMasterId && d.BoothStatus == true).CountAsync();
                        if (spWardCount != 0 && boothCount != 0)
                        {
                            return new ServiceResponse { IsSucceed = false, Message = $"Panchayats {spWardCount} and Booths {boothCount} are active under this {blockZonePanchayat.PSZonePanchayatName}. Make sure they are Inactive first" };
                        }
                    }

                    blockZonePanchayat.PSZonePanchayatStatus = updateMasterStatus.IsStatus;
                    _context.PSZonePanchayat.Update(blockZonePanchayat);
                    await _context.SaveChangesAsync();

                    string messageSp = blockZonePanchayat.PSZonePanchayatStatus ? "Panchayat Activated Successfully" : "Panchayat Deactivated Successfully";
                    return new ServiceResponse { IsSucceed = true, Message = messageSp };

                case "SPWards":
                    var spWards = await _context.GPPanchayatWards
                        .Where(d => d.GPPanchayatWardsMasterId == Convert.ToInt32(updateMasterStatus.Id))
                        .FirstOrDefaultAsync();

                    if (spWards == null)
                    {
                        return new ServiceResponse { IsSucceed = false, Message = "Record Not Found." };
                    }

                    spWards.GPPanchayatWardsStatus = updateMasterStatus.IsStatus;
                    _context.GPPanchayatWards.Update(spWards);
                    await _context.SaveChangesAsync();

                    string messageSpWard = spWards.GPPanchayatWardsStatus ? "Ward Activated Successfully" : "Ward Deactivated Successfully";
                    return new ServiceResponse { IsSucceed = true, Message = messageSpWard };

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


                case "BoothMaster":
                    var isBoothExist = await _context.BoothMaster.Where(d => d.BoothMasterId == Convert.ToInt32(updateMasterStatus.Id)).FirstOrDefaultAsync();
                    if (isBoothExist != null)
                    {
                        var electionInfoRecord = await _context.ElectionInfoMaster
      .Where(d => d.StateMasterId == isBoothExist.StateMasterId && d.DistrictMasterId == isBoothExist.DistrictMasterId && d.AssemblyMasterId == isBoothExist.AssemblyMasterId && d.BoothMasterId == isBoothExist.BoothMasterId)
      .FirstOrDefaultAsync();
                        if (electionInfoRecord == null)
                        {
                            if (String.IsNullOrEmpty(isBoothExist.AssignedTo))
                            {
                                if (String.IsNullOrEmpty(isBoothExist.AssignedToBLO))
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
                case "BlockZonePanchayat":
                    var panchayatRecord = await _context.PSZonePanchayat.Where(d => d.PSZonePanchayatMasterId == Convert.ToInt32(updateMasterStatus.Id)).FirstOrDefaultAsync();

                    var isBoothExistofPanchyat = await _context.BoothMaster.Where(d => d.PSZonePanchayatMasterId == Convert.ToInt32(updateMasterStatus.Id)).CountAsync();
                    var isWardExistofPanchyat = await _context.GPPanchayatWards.Where(d => d.FourthLevelHMasterId == Convert.ToInt32(updateMasterStatus.Id)).CountAsync();


                    if (panchayatRecord != null)
                    {

                        if (panchayatRecord.ElectionTypeMasterId == 1)
                        {
                            if (isBoothExistofPanchyat > 0 && isWardExistofPanchyat > 0)
                            {
                                return new ServiceResponse { IsSucceed = false, Message = "Data Found aganist this Panchayt in Booth and Ward, can't deelete !" };

                            }

                            else if (isBoothExistofPanchyat > 0 && isWardExistofPanchyat == 0)
                            {
                                return new ServiceResponse { IsSucceed = false, Message = "Data Found aganist this Panchayt in Booths, can't deelete !" };

                            }
                            else if (isBoothExistofPanchyat == 0 && isWardExistofPanchyat > 0)
                            {
                                return new ServiceResponse { IsSucceed = false, Message = "Data Found aganist this Panchayt in Ward, can't deelete !" };

                            }
                            else
                            {
                                _context.PSZonePanchayat.Remove(panchayatRecord);
                                await _context.SaveChangesAsync();
                                return new ServiceResponse { IsSucceed = true, Message = "Panchayat Deleted Successfully" };

                            }
                        }
                        else if (panchayatRecord.ElectionTypeMasterId == 2)///panchyat samiti
                        {
                            if (isBoothExistofPanchyat > 0)
                            {
                                return new ServiceResponse { IsSucceed = false, Message = "Data Found aganist this Panchayt in Booth, can't deelete !" };

                            }

                            else
                            {
                                _context.PSZonePanchayat.Remove(panchayatRecord);
                                await _context.SaveChangesAsync();
                                return new ServiceResponse { IsSucceed = true, Message = "Panchayat Deleted Successfully" };

                            }
                        }
                        else
                        {
                            return new ServiceResponse { IsSucceed = false, Message = "Panchayat can only be deleted for Gram Panchayat or Panchayat Samiti elections." };
                        }
                    }
                    else
                    {
                        return new ServiceResponse { IsSucceed = false, Message = "Panchayat Record Not Found." };

                    }
                case "FourthLevel":
                    var fourthLevelRecord = await _context.FourthLevelH.Where(d => d.FourthLevelHMasterId == Convert.ToInt32(updateMasterStatus.Id)).FirstOrDefaultAsync();
                    if (fourthLevelRecord != null)
                    {

                        if (fourthLevelRecord.ElectionTypeMasterId == 1 || fourthLevelRecord.ElectionTypeMasterId == 2)
                        {
                            var blockZonePanchyatRecord = await _context.PSZonePanchayat.Where(d => d.FourthLevelHMasterId == Convert.ToInt32(updateMasterStatus.Id)).CountAsync();

                            if (blockZonePanchyatRecord > 0)
                            {
                                return new ServiceResponse { IsSucceed = false, Message = "Data Found aganist this Sub Local Body Record in Block-Zone Panchayat, can't delete !" };

                            }
                            else
                            {
                                _context.FourthLevelH.Remove(fourthLevelRecord);
                                await _context.SaveChangesAsync();
                                return new ServiceResponse { IsSucceed = true, Message = "Sub Local Body Record Deleted Successfully" };

                            }
                        }
                        else if (fourthLevelRecord.ElectionTypeMasterId == 3 || fourthLevelRecord.ElectionTypeMasterId == 4 || fourthLevelRecord.ElectionTypeMasterId == 5 || fourthLevelRecord.ElectionTypeMasterId == 6)
                        {
                            var isBoothExistofFourthLevel = await _context.BoothMaster.Where(d => d.FourthLevelHMasterId == Convert.ToInt32(updateMasterStatus.Id)).CountAsync();

                            if (isBoothExistofFourthLevel > 0)
                            {
                                return new ServiceResponse { IsSucceed = false, Message = "Data Found aganist this SubLocal Body Record in Booth, can't delete !" };

                            }

                            else
                            {
                                _context.FourthLevelH.Remove(fourthLevelRecord);
                                await _context.SaveChangesAsync();
                                return new ServiceResponse { IsSucceed = true, Message = "Sub Local Body record Deleted Successfully" };

                            }
                        }
                        else
                        {
                            return new ServiceResponse { IsSucceed = false, Message = "Election Type is not valid to delete this Sub local Body Record." };
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
                        var fourthLevelExistsInAssembly = await _context.FourthLevelH
                            .Where(d => d.StateMasterId == assemblyMaster.StateMasterId && d.DistrictMasterId == assemblyMaster.DistrictMasterId && d.AssemblyMasterId == assemblyMaster.AssemblyMasterId)
                            .ToListAsync();

                        if (fourthLevelExistsInAssembly.Count > 0)
                        {
                            return new ServiceResponse
                            {
                                IsSucceed = false,
                                Message = "Sub Local Bodies data Present aganist this Record, Can't delete!"
                            };
                        }
                        else
                        {
                            _context.AssemblyMaster.Remove(assemblyMaster);
                            await _context.SaveChangesAsync();
                            return new ServiceResponse { IsSucceed = true, Message = "Record Deleted Succesfully." };

                        }
                        // }



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
                            return new ServiceResponse { IsSucceed = false, Message = "Can’t delete as Local Body record exists against this District." };

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
                case "StateMaster":

                    var stateRecord = await _context.StateMaster.FirstOrDefaultAsync(d => d.StateMasterId == Convert.ToInt32(updateMasterStatus.Id));

                    if (stateRecord != null)
                    {

                        var districtsActiveOfState = await _context.DistrictMaster
                            .Where(d => d.StateMasterId == stateRecord.StateMasterId)
                            .ToListAsync();

                        if (districtsActiveOfState.Count > 0)
                        {
                            return new ServiceResponse { IsSucceed = false, Message = "District Records are present in aganist this State,can't delete!" };
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


                case "SOMaster":
                    var isSOExist = await _context.FieldOfficerMaster.Where(d => d.FieldOfficerMasterId == Convert.ToInt32(updateMasterStatus.Id)).FirstOrDefaultAsync();
                    if (isSOExist != null)
                    {
                        var boothsAllocated = await _context.BoothMaster.Where(p => p.AssignedTo == isSOExist.FieldOfficerMasterId.ToString()).ToListAsync();
                        if (boothsAllocated.Count == 0)
                        {
                            // isSOExist.SoStatus = updateMasterStatus.IsStatus;
                            _context.FieldOfficerMaster.Remove(isSOExist);
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
            var soRecords = _context.FieldOfficerMaster
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
    .FirstOrDefault(p => (p.StateCode == stateMaster.StateCode || p.StateName == stateMaster.StateName));


                if (stateExist == null)
                {
                    stateMaster.StateCreatedAt = BharatDateTime();
                    _context.StateMaster.Add(stateMaster);
                    _context.SaveChanges();

                    return new Response { Status = RequestStatusEnum.OK, Message = $"State Added Successfully {stateMaster.StateName}" };
                }
                else
                {

                    return new Response { Status = RequestStatusEnum.BadRequest, Message = $"State Already Exists  {stateMaster.StateName} {stateMaster.StateCode}" };
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
                var isStateActive = _context.StateMaster.FirstOrDefault(d => d.StateMasterId == Convert.ToInt32(stateMasterId));
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
                    var getAssemblyForElectionType = await GetAssemblyByDistrictIdForElectionType(districtMaster.DistrictMasterId);

                    if (isExistName.Count == 0)
                    {
                        var districtMasterRecord = _context.DistrictMaster.FirstOrDefault(d => d.DistrictMasterId == districtMaster.DistrictMasterId);

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
                var isExist = _context.DistrictMaster.FirstOrDefault(p => (p.DistrictName == districtMaster.DistrictName && p.StateMasterId == districtMaster.StateMasterId));
                var isStateActive = _context.StateMaster.FirstOrDefault(p => p.StateMasterId == districtMaster.StateMasterId);
                if (isStateActive.StateStatus)
                    if (isExist == null)
                    {
                        var isExistCode = _context.DistrictMaster.Where(p => p.DistrictCode == districtMaster.DistrictCode && p.StateMasterId == districtMaster.StateMasterId).FirstOrDefault();
                        if (isExistCode == null)
                        {
                            districtMaster.DistrictCreatedAt = BharatDateTime();
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

                var isAssemblyCodeExist = await _context.AssemblyMaster.Where(p => p.AssemblyCode == assemblyMaster.AssemblyCode && p.StateMasterId == assemblyMaster.StateMasterId && p.ElectionTypeMasterId == assemblyMaster.ElectionTypeMasterId && p.AssemblyMasterId != assemblyMaster.AssemblyMasterId && p.DistrictMasterId == assemblyMaster.DistrictMasterId).ToListAsync();
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

                                return new Response { Status = RequestStatusEnum.OK, Message = "Assembly Updated Successfully " + assemblyMaster.AssemblyName };
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
                                assembliesMasterRecord.PCMasterId = null;
                                assembliesMasterRecord.TotalBooths = assemblyMaster.TotalBooths;
                                assembliesMasterRecord.ElectionTypeMasterId = assemblyMaster.ElectionTypeMasterId;
                                _context.AssemblyMaster.Update(assembliesMasterRecord);
                                await _context.SaveChangesAsync();

                                return new Response { Status = RequestStatusEnum.OK, Message = "Assembly Updated Successfully " + assemblyMaster.AssemblyName };
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
        public async Task<AssemblyMaster> GetAssemblyByDistrictIdForElectionType(int districtMasterId)
        {
            var assemblyRecord = await _context.AssemblyMaster.Where(d => d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.ElectionTypeMasterId == 1).FirstOrDefaultAsync();
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

        #region FO Master
        public async Task<List<FieldOfficerMaster>> GetFieldOfficersListById(int stateMasterId, int districtMasterId, int assemblyMasterId)
        {
            var foList = await _context.FieldOfficerMaster.Where(d => d.StateMasterId == stateMasterId && d.DistrictMasterId == districtMasterId && d.AssemblyMasterId == assemblyMasterId).ToListAsync();
            return foList;
        }

        public async Task<FieldOfficerProfile> GetSectorOfficerProfile2(string soId)
        {
            //var sectorOfficerProfile = await (from so in _context.SectorOfficerMaster
            //                                  where so.SOMasterId == Convert.ToInt32(soId)
            //                                  join state in _context.StateMaster on so.StateMasterId equals state.StateMasterId
            //                                  join assembly in _context.AssemblyMaster on so.SoAssemblyCode equals assembly.AssemblyCode
            //                                  join district in _context.DistrictMaster on assembly.DistrictMasterId equals district.DistrictMasterId


            //                                  where so.SoStatus && so.StateMasterId == state.StateMasterId
            //                                  select new SectorOfficerProfile
            //                                  {
            //                                      StateName = state.StateName,
            //                                      DistrictName = district.DistrictName,
            //                                      AssemblyName = assembly.AssemblyName,
            //                                      AssemblyCode = assembly.AssemblyCode.ToString(),
            //                                      SoName = so.SoName,
            //                                      ElectionType = so.ElectionTypeMasterId == 1 ? "LS" : (so.ElectionTypeMasterId == 2 ? "VS" : null),
            //                                      BoothNo = _context.BoothMaster.Where(p => p.AssignedTo == soId)
            //                                                                       .OrderBy(p => p.BoothCode_No)
            //                                                                       .Select(p => p.BoothCode_No.ToString())
            //                                                                       .ToList()
            //                                  }).FirstOrDefaultAsync();

            //return sectorOfficerProfile;
            return null;
        }


        //public async Task<FieldOfficerProfile> GetFieldOfficerProfile(string foId)
        //{
        //    var foRecord = _context.FieldOfficerMaster.Where(d => d.FieldOfficerMasterId == Convert.ToInt32(foId) && d.FieldOfficerStatus == true).FirstOrDefault();
        //    var foAssembly = _context.AssemblyMaster.Where(d => d.AssemblyMasterId == foRecord.AssemblyMasterId && d.StateMasterId == foRecord.StateMasterId).FirstOrDefault();
        //    var foDistrict = _context.DistrictMaster.Where(d => d.DistrictMasterId == foAssembly.DistrictMasterId && d.StateMasterId == foAssembly.StateMasterId).FirstOrDefault();
        //    var foState = _context.StateMaster.Where(d => d.StateMasterId == foDistrict.StateMasterId).FirstOrDefault();
        //    var electionType = _context.ElectionTypeMaster.Where(d => d.ElectionTypeMasterId == foDistrict.StateMasterId).FirstOrDefault();
        //    FieldOfficerProfile fieldOfficerProfile = new FieldOfficerProfile()
        //    {
        //        StateMasterId = foRecord.StateMasterId,
        //        StateName = foState.StateName,
        //        DistrictMasterId = foRecord.DistrictMasterId,
        //        DistrictName = foDistrict.DistrictName,
        //        AssemblyMasterId = foRecord.AssemblyMasterId,
        //        AssemblyName = foAssembly.AssemblyName,
        //        FoName = foRecord.FieldOfficerOfficeName,
        //        OfficerRole = "FO",
        //        ElectionTypeMasterId= foRecord.ElectionTypeMasterId,
        //        ElectionTypeName = electionType.ElectionType,
        //        BoothNo = _context.BoothMaster.Where(p => p.AssignedTo == foId).OrderBy(p => Convert.ToInt32(p.BoothCode_No)).Select(p => p.BoothCode_No.ToString()).ToList()


        //    };
        //    return fieldOfficerProfile;
        //}
        public async Task<FieldOfficerProfile> GetFieldOfficerProfile(string foId)
        {

            // Use a join to fetch the Field Officer along with related data
            var fieldOfficerProfile = await (from fo in _context.FieldOfficerMaster
                                             join asm in _context.AssemblyMaster
                                             on fo.AssemblyMasterId equals asm.AssemblyMasterId
                                             join dist in _context.DistrictMaster
                                             on asm.DistrictMasterId equals dist.DistrictMasterId
                                             join state in _context.StateMaster
                                             on dist.StateMasterId equals state.StateMasterId
                                             join electionType in _context.ElectionTypeMaster
                                             on fo.ElectionTypeMasterId equals electionType.ElectionTypeMasterId
                                             where fo.FieldOfficerMasterId == Convert.ToInt32(foId) && fo.FieldOfficerStatus == true
                                             select new FieldOfficerProfile
                                             {
                                                 StateMasterId = state.StateMasterId,
                                                 StateName = state.StateName,
                                                 DistrictMasterId = dist.DistrictMasterId,
                                                 DistrictName = dist.DistrictName,
                                                 AssemblyMasterId = asm.AssemblyMasterId,
                                                 AssemblyName = asm.AssemblyName,
                                                 FoName = fo.FieldOfficerOfficeName,
                                                 Role = "FO",
                                                 ElectionTypeMasterId = fo.ElectionTypeMasterId,
                                                 ElectionTypeName = electionType.ElectionType,
                                                 BoothNo = _context.BoothMaster
                                                         .Where(b => b.AssignedTo == foId)
                                                         .OrderBy(b => Convert.ToInt32(b.BoothCode_No))
                                                         .Select(b => b.BoothCode_No.ToString())
                                                         .ToList()
                                             }).FirstOrDefaultAsync();

            return fieldOfficerProfile;
        }

        public async Task<Response> AddFieldOfficer(FieldOfficerMaster fieldOfficerViewModel)
        {
            // Check if FieldOfficer with the same mobile number, election type, and state already exists
            var existingOfficerMobile = await _context.FieldOfficerMaster
                                                .FirstOrDefaultAsync(d => d.FieldOfficerMobile == fieldOfficerViewModel.FieldOfficerMobile
                                                                         && d.ElectionTypeMasterId == fieldOfficerViewModel.ElectionTypeMasterId
                                                                         && d.StateMasterId == fieldOfficerViewModel.StateMasterId);


            // If more than two officers already exist with the same mobile number for this election type, return an error response
            if (existingOfficerMobile is not null)
            {
                return new Response
                {
                    Status = RequestStatusEnum.BadRequest,
                    Message = $"FO User {fieldOfficerViewModel.FieldOfficerName} Already Exists in this election "
                };
            }

            // If no duplicates exist, add the new FieldOfficer and save changes
            _context.FieldOfficerMaster.Add(fieldOfficerViewModel);
            await _context.SaveChangesAsync();

            // Return success response
            return new Response
            {
                Status = RequestStatusEnum.OK,
                Message = "Field Officer added successfully"
            };
        }
        public async Task<Response> UpdateFieldOfficer(FieldOfficerMaster updatedFieldOfficer)
        {
            // Fetch the existing officer and check for uniqueness of the mobile number in one query
            var existingOfficer = await _context.FieldOfficerMaster
                .Where(d => d.FieldOfficerMasterId == updatedFieldOfficer.FieldOfficerMasterId)
                .Select(d => new
                {
                    Officer = d,
                    IsMobileDuplicate = _context.FieldOfficerMaster.Any(m =>
                        m.FieldOfficerMobile == updatedFieldOfficer.FieldOfficerMobile
                        && m.FieldOfficerMasterId != d.FieldOfficerMasterId && m.ElectionTypeMasterId == updatedFieldOfficer.ElectionTypeMasterId),
                    ExistingOfficerWithSameMobile = _context.FieldOfficerMaster.FirstOrDefault(m =>
                        m.FieldOfficerMobile == updatedFieldOfficer.FieldOfficerMobile
                        && m.ElectionTypeMasterId == updatedFieldOfficer.ElectionTypeMasterId
                        && m.StateMasterId == updatedFieldOfficer.StateMasterId
                        && m.DistrictMasterId == updatedFieldOfficer.DistrictMasterId && m.ElectionTypeMasterId == updatedFieldOfficer.ElectionTypeMasterId)
                })
                .FirstOrDefaultAsync();

            if (existingOfficer == null)
            {
                return new Response
                {
                    Status = RequestStatusEnum.BadRequest,
                    Message = "No Record Found"
                };
            }



            if (updatedFieldOfficer.FieldOfficerMobile != existingOfficer.Officer.FieldOfficerMobile
                && existingOfficer.IsMobileDuplicate)
            {
                return new Response
                {
                    Status = RequestStatusEnum.BadRequest,
                    Message = "Mobile Number Already Exists"
                };
            }

            // Map updated fields from updatedFieldOfficer to the existing officer
            existingOfficer.Officer.StateMasterId = updatedFieldOfficer.StateMasterId;
            existingOfficer.Officer.DistrictMasterId = updatedFieldOfficer.DistrictMasterId;
            existingOfficer.Officer.AssemblyMasterId = updatedFieldOfficer.AssemblyMasterId;
            existingOfficer.Officer.FieldOfficerName = updatedFieldOfficer.FieldOfficerName;
            existingOfficer.Officer.FieldOfficerMobile = updatedFieldOfficer.FieldOfficerMobile;
            existingOfficer.Officer.FieldOfficerDesignation = updatedFieldOfficer.FieldOfficerDesignation;
            existingOfficer.Officer.FieldOfficerOfficeName = updatedFieldOfficer.FieldOfficerOfficeName;
            existingOfficer.Officer.FieldOfficerUpdatedAt = BharatDateTime();
            existingOfficer.Officer.FieldOfficerStatus = updatedFieldOfficer.FieldOfficerStatus;
            existingOfficer.Officer.OTPGeneratedTime = updatedFieldOfficer.OTPGeneratedTime;
            existingOfficer.Officer.OTP = updatedFieldOfficer.OTP;
            existingOfficer.Officer.OTPExpireTime = updatedFieldOfficer.OTPExpireTime;
            existingOfficer.Officer.OTPAttempts = updatedFieldOfficer.OTPAttempts;
            existingOfficer.Officer.RefreshToken = updatedFieldOfficer.RefreshToken;
            existingOfficer.Officer.RefreshTokenExpiryTime = updatedFieldOfficer.RefreshTokenExpiryTime;
            existingOfficer.Officer.AppPin = updatedFieldOfficer.AppPin;
            existingOfficer.Officer.IsLocked = updatedFieldOfficer.IsLocked;
            existingOfficer.Officer.ElectionTypeMasterId = updatedFieldOfficer.ElectionTypeMasterId;

            _context.FieldOfficerMaster.Update(existingOfficer.Officer);
            await _context.SaveChangesAsync();

            return new Response
            {
                Status = RequestStatusEnum.OK,
                Message = "Field Officer updated successfully"
            };
        }


        public async Task<Response> UpdateFieldOfficerValidate(FieldOfficerMaster updatedFieldOfficer)
        {
            // Check if the record exists based on the FieldOfficerMasterId
            var existingOfficer = await _context.FieldOfficerMaster
                                                .FirstOrDefaultAsync(d => d.FieldOfficerMasterId == updatedFieldOfficer.FieldOfficerMasterId);

            if (existingOfficer == null)
            {
                // Return a response if the record is not found
                return new Response
                {
                    Status = RequestStatusEnum.BadRequest,
                    Message = "No Record Found"
                };
            }

            // Map all fields from updatedFieldOfficer to existingOfficer
            existingOfficer.StateMasterId = updatedFieldOfficer.StateMasterId;
            existingOfficer.DistrictMasterId = updatedFieldOfficer.DistrictMasterId;
            existingOfficer.AssemblyMasterId = updatedFieldOfficer.AssemblyMasterId;
            existingOfficer.FieldOfficerName = updatedFieldOfficer.FieldOfficerName;
            existingOfficer.FieldOfficerDesignation = updatedFieldOfficer.FieldOfficerDesignation;
            existingOfficer.FieldOfficerOfficeName = updatedFieldOfficer.FieldOfficerOfficeName;
            existingOfficer.FieldOfficerMobile = updatedFieldOfficer.FieldOfficerMobile;
            existingOfficer.FieldOfficerUpdatedAt = BharatDateTime(); // Set the updated time to the current time
            existingOfficer.FieldOfficerStatus = updatedFieldOfficer.FieldOfficerStatus;
            existingOfficer.OTPGeneratedTime = updatedFieldOfficer.OTPGeneratedTime;
            existingOfficer.OTP = updatedFieldOfficer.OTP;
            existingOfficer.OTPExpireTime = updatedFieldOfficer.OTPExpireTime;
            existingOfficer.OTPAttempts = updatedFieldOfficer.OTPAttempts;
            existingOfficer.RefreshToken = updatedFieldOfficer.RefreshToken;
            existingOfficer.RefreshTokenExpiryTime = updatedFieldOfficer.RefreshTokenExpiryTime;
            existingOfficer.AppPin = updatedFieldOfficer.AppPin;
            existingOfficer.IsLocked = updatedFieldOfficer.IsLocked;
            existingOfficer.ElectionTypeMasterId = updatedFieldOfficer.ElectionTypeMasterId;

            _context.FieldOfficerMaster.Update(existingOfficer);

            _context.SaveChanges();

            // Return a success response
            return new Response
            {
                Status = RequestStatusEnum.OK,
                Message = "Field Officer updated successfully"
            };
        }

        /// <summary this api for Portal>
        public async Task<List<CombinedMaster>> GetBoothListByFoId(int stateMasterId, int districtMasterId, int assemblyMasterId, int foId)
        {

            var boothlist = from bt in _context.BoothMaster.Where(d => d.StateMasterId == stateMasterId
                            && d.DistrictMasterId == districtMasterId
                            && d.AssemblyMasterId == assemblyMasterId && d.AssignedTo == foId.ToString())
                            join fourthLevelH in _context.FourthLevelH on bt.FourthLevelHMasterId equals fourthLevelH.FourthLevelHMasterId
                            join asem in _context.AssemblyMaster
                            on bt.AssemblyMasterId equals asem.AssemblyMasterId
                            join dist in _context.DistrictMaster
                            on asem.DistrictMasterId equals dist.DistrictMasterId
                            join state in _context.StateMaster
                             on dist.StateMasterId equals state.StateMasterId

                            select new CombinedMaster
                            {
                                StateId = stateMasterId,
                                StateName = state.StateName,
                                DistrictId = dist.DistrictMasterId,
                                DistrictName = dist.DistrictName,
                                DistrictCode = dist.DistrictCode,
                                AssemblyId = asem.AssemblyMasterId,
                                AssemblyName = asem.AssemblyName,
                                AssemblyCode = asem.AssemblyCode,
                                FourthLevelHMasterId = fourthLevelH.FourthLevelHMasterId,
                                FourthLevelHName = fourthLevelH.HierarchyName,
                                BoothMasterId = bt.BoothMasterId,
                                BoothName = bt.BoothName,
                                //BoothAuxy = bt.BoothNoAuxy,
                                BoothAuxy = (bt.BoothNoAuxy == "0") ? string.Empty : bt.BoothNoAuxy,
                                IsStatus = bt.BoothStatus,
                                BoothCode_No = bt.BoothCode_No,
                                IsAssigned = bt.IsAssigned,
                                FieldOfficerMasterId = foId,
                                IsBoothInterrupted = bt.IsBoothInterrupted


                            };
            var count = boothlist.Count();
            return await boothlist.ToListAsync();
        }
        /// </summary>
        /// <summary this api for Mobile App>


        public async Task<List<CombinedMaster>> GetBoothListForFo(int stateMasterId, int districtMasterId, int assemblyMasterId, int foId)
        {
            // Step 1: Get booth list with joins
            var boothlist = from bt in _context.BoothMaster
                                .AsNoTracking()
                                .Where(d => d.StateMasterId == stateMasterId &&
                                            d.DistrictMasterId == districtMasterId &&
                                            d.AssemblyMasterId == assemblyMasterId &&
                                            d.AssignedTo == foId.ToString())
                            join fourthLevelH in _context.FourthLevelH.AsNoTracking() on bt.FourthLevelHMasterId equals fourthLevelH.FourthLevelHMasterId
                            join asem in _context.AssemblyMaster.AsNoTracking() on bt.AssemblyMasterId equals asem.AssemblyMasterId
                            join dist in _context.DistrictMaster.AsNoTracking() on asem.DistrictMasterId equals dist.DistrictMasterId
                            join state in _context.StateMaster.AsNoTracking() on dist.StateMasterId equals state.StateMasterId
                            select new CombinedMaster
                            {
                                StateId = stateMasterId,
                                StateName = state.StateName,
                                DistrictId = dist.DistrictMasterId,
                                DistrictName = dist.DistrictName,
                                DistrictCode = dist.DistrictCode,
                                AssemblyId = asem.AssemblyMasterId,
                                AssemblyName = asem.AssemblyName,
                                AssemblyCode = asem.AssemblyCode,
                                FourthLevelHMasterId = fourthLevelH.FourthLevelHMasterId,
                                FourthLevelHName = fourthLevelH.HierarchyName,
                                BoothMasterId = bt.BoothMasterId,
                                BoothName = bt.BoothName,
                                BoothAuxy = bt.BoothNoAuxy == "0" ? string.Empty : bt.BoothNoAuxy,
                                IsStatus = bt.BoothStatus,
                                BoothCode_No = bt.BoothCode_No,
                                IsAssigned = bt.IsAssigned,
                                FieldOfficerMasterId = foId,
                                ElectionTypeMasterId = bt.ElectionTypeMasterId,
                                IsBoothInterrupted = bt.IsBoothInterrupted,
                                IsVTInterrupted = bt.IsVTInterrupted

                            };

            var boothListResult = await boothlist.ToListAsync();

            // Step 2: Fetch Election Info records in a batch instead of inside the loop
            var boothIds = boothListResult.Select(b => b.BoothMasterId).ToList();
            var electionInfoRecords = await _context.ElectionInfoMaster
                .AsNoTracking()
                .Where(e => boothIds.Contains(e.BoothMasterId) &&
                            e.StateMasterId == stateMasterId)
                .ToListAsync();

            // Step 3: Fetch first event from cache or database before the loop
            var getFirstEvent = await _cacheService.GetDataAsync<EventMaster>("GetFirstEvent");
            if (getFirstEvent is null)
            {
                getFirstEvent = await GetFirstSequenceEventById(stateMasterId, boothListResult.FirstOrDefault()?.ElectionTypeMasterId ?? 0);
                await _cacheService.SetDataAsync("GetFirstEvent", getFirstEvent, BharatTimeDynamic(0, 0, 0, 5, 0));
            }

            // Step 4: Update each booth's event data
            // Convert electionInfoRecords to a dictionary for faster lookups by BoothMasterId and ElectionTypeMasterId
            var electionInfoDict = electionInfoRecords.ToDictionary(e => (e.BoothMasterId, e.ElectionTypeMasterId));

            foreach (var booth in boothListResult)
            {
                // Try to find matching electionInfo in the dictionary
                if (electionInfoDict.TryGetValue((booth.BoothMasterId, booth.ElectionTypeMasterId), out var electionInfo))
                {
                    var updateEventActivity = new UpdateEventActivity
                    {
                        StateMasterId = booth.StateId,
                        DistrictMasterId = booth.DistrictId,
                        AssemblyMasterId = booth.AssemblyId,
                        ElectionTypeMasterId = booth.ElectionTypeMasterId,
                        EventMasterId = electionInfo.EventMasterId,
                        EventSequence = electionInfo.EventSequence,
                        EventABBR = electionInfo.EventABBR,
                        EventStatus = electionInfo.EventStatus
                    };

                    if (electionInfo.EventStatus == true)
                    {
                        var eventInfo = await GetNextEvent(updateEventActivity);
                        booth.EventMasterId = eventInfo.EventMasterId;
                        booth.EventSequence = eventInfo.EventSequence;
                        booth.EventABBR = eventInfo.EventABBR;
                        booth.EventName = eventInfo.EventName;
                    }
                    else
                    {
                        // Set booth event info from electionInfo if status is not true
                        booth.EventMasterId = electionInfo.EventMasterId;
                        booth.EventSequence = electionInfo.EventSequence;
                        booth.EventABBR = electionInfo.EventABBR;
                        booth.EventName = electionInfo.EventName;
                    }
                }
                else
                {
                    // Assign values from getFirstEvent if no matching electionInfo is found
                    booth.EventMasterId = getFirstEvent.EventMasterId;
                    booth.EventSequence = getFirstEvent.EventSequence;
                    booth.EventABBR = getFirstEvent.EventABBR;
                    booth.EventName = getFirstEvent.EventName;
                }
            }

            return boothListResult;
        }

        public async Task<FieldOfficerMasterList> GetFieldOfficerById(int fieldOfficerMasterId)
        {
            var foRecord = await _context.FieldOfficerMaster
                .Where(fo => fo.FieldOfficerMasterId == fieldOfficerMasterId)
                .Join(_context.StateMaster,
                      fo => fo.StateMasterId,
                      sm => sm.StateMasterId,
                      (fo, sm) => new { FieldOfficerMaster = fo, StateMaster = sm })
                .Join(_context.DistrictMaster,
                      joined => joined.FieldOfficerMaster.DistrictMasterId,
                      dm => dm.DistrictMasterId,
                      (joined, dm) => new { joined.FieldOfficerMaster, joined.StateMaster, DistrictMaster = dm })
                .Join(_context.AssemblyMaster,
                      joined => joined.FieldOfficerMaster.AssemblyMasterId,
                      am => am.AssemblyMasterId,
                      (joined, am) => new { joined.FieldOfficerMaster, joined.StateMaster, joined.DistrictMaster, AssemblyMaster = am })
                .Join(_context.ElectionTypeMaster,
                      joined => joined.FieldOfficerMaster.ElectionTypeMasterId,
                      etm => etm.ElectionTypeMasterId,
                      (joined, etm) => new FieldOfficerMasterList
                      {
                          FieldOfficerMasterId = joined.FieldOfficerMaster.FieldOfficerMasterId,
                          StateMasterId = joined.StateMaster.StateMasterId,
                          StateName = joined.StateMaster.StateName,
                          DistrictMasterId = joined.DistrictMaster.DistrictMasterId,
                          DistrictName = joined.DistrictMaster.DistrictName,
                          AssemblyMasterId = joined.AssemblyMaster.AssemblyMasterId,
                          AssemblyCode = joined.AssemblyMaster.AssemblyCode,
                          AssemblyName = joined.AssemblyMaster.AssemblyName,
                          FieldOfficerName = joined.FieldOfficerMaster.FieldOfficerName,
                          FieldOfficerDesignation = joined.FieldOfficerMaster.FieldOfficerDesignation,
                          FieldOfficerOfficeName = joined.FieldOfficerMaster.FieldOfficerOfficeName,
                          FieldOfficerMobile = joined.FieldOfficerMaster.FieldOfficerMobile,
                          FieldOfficerStatus = joined.FieldOfficerMaster.FieldOfficerStatus,
                          OTPGeneratedTime = joined.FieldOfficerMaster.OTPGeneratedTime,
                          OTP = joined.FieldOfficerMaster.OTP,
                          OTPExpireTime = joined.FieldOfficerMaster.OTPExpireTime,
                          OTPAttempts = joined.FieldOfficerMaster.OTPAttempts,
                          IsLocked = joined.FieldOfficerMaster.IsLocked,
                          ElectionTypeMasterId = joined.FieldOfficerMaster.ElectionTypeMasterId,
                          ElectionTypeName = etm.ElectionType
                      })
                .FirstOrDefaultAsync();

            return foRecord;
        }



        #endregion
        #region AROResult
        public async Task<Response> AddAROResult(AROResultMaster aROResultMaster)
        {
            // Check if FieldOfficer with the same mobile number, election type, and state already exists
            var existingOfficerMobile = await _context.AROResultMaster
                                                .FirstOrDefaultAsync(d => d.AROMobile == aROResultMaster.AROMobile
                                                                         && d.ElectionTypeMasterId == aROResultMaster.ElectionTypeMasterId
                                                                         && d.StateMasterId == aROResultMaster.StateMasterId);


            // If more than two officers already exist with the same mobile number for this election type, return an error response
            if (existingOfficerMobile is not null)
            {
                return new Response
                {
                    Status = RequestStatusEnum.BadRequest,
                    Message = $"ARO User {aROResultMaster.AROName} Already Exists in this election "
                };
            }

            // If no duplicates exist, add the new FieldOfficer and save changes
            _context.AROResultMaster.Add(aROResultMaster);
            await _context.SaveChangesAsync();

            // Return success response
            return new Response
            {
                Status = RequestStatusEnum.OK,
                Message = "ARO added successfully"
            };
        }

        public async Task<AROResultMasterList> GetAROResultById(int aROMasterId)
        {
            var aroRecord = await _context.AROResultMaster
                .Where(aro => aro.AROMasterId == aROMasterId)
                .Join(_context.StateMaster,
                      aro => aro.StateMasterId,
                      sm => sm.StateMasterId,
                      (aro, sm) => new { AROResultMaster = aro, StateMaster = sm })
                .Join(_context.DistrictMaster,
                      joined => joined.AROResultMaster.DistrictMasterId,
                      dm => dm.DistrictMasterId,
                      (joined, dm) => new { joined.AROResultMaster, joined.StateMaster, DistrictMaster = dm })
                .Join(_context.AssemblyMaster,
                      joined => joined.AROResultMaster.AssemblyMasterId,
                      am => am.AssemblyMasterId,
                      (joined, am) => new { joined.AROResultMaster, joined.StateMaster, joined.DistrictMaster, AssemblyMaster = am })
                .Join(_context.FourthLevelH,
                      joined => joined.AROResultMaster.FourthLevelHMasterId,
                      flh => flh.FourthLevelHMasterId,
                      (joined, flh) => new { joined.AROResultMaster, joined.StateMaster, joined.DistrictMaster, joined.AssemblyMaster, FourthLevelH = flh })
                .Join(_context.ElectionTypeMaster,
                      joined => joined.AROResultMaster.ElectionTypeMasterId,
                      etm => etm.ElectionTypeMasterId,
                      (joined, etm) => new AROResultMasterList
                      {
                          AROMasterId = joined.AROResultMaster.AROMasterId,
                          StateMasterId = joined.StateMaster.StateMasterId,
                          StateName = joined.StateMaster.StateName,
                          DistrictMasterId = joined.DistrictMaster.DistrictMasterId,
                          DistrictName = joined.DistrictMaster.DistrictName,
                          AssemblyMasterId = joined.AssemblyMaster.AssemblyMasterId,
                          AssemblyCode = joined.AssemblyMaster.AssemblyCode,
                          AssemblyName = joined.AssemblyMaster.AssemblyName,
                          FourthLevelHMasterId = joined.FourthLevelH.FourthLevelHMasterId,
                          HierarchyName = joined.FourthLevelH.HierarchyName,
                          AROName = joined.AROResultMaster.AROName,
                          ARODesignation = joined.AROResultMaster.ARODesignation,
                          AROOfficeName = joined.AROResultMaster.AROOfficeName,
                          AROMobile = joined.AROResultMaster.AROMobile,
                          IsStatus = joined.AROResultMaster.IsStatus,
                          OTPGeneratedTime = joined.AROResultMaster.OTPGeneratedTime,
                          OTP = joined.AROResultMaster.OTP,
                          OTPExpireTime = joined.AROResultMaster.OTPExpireTime,
                          OTPAttempts = joined.AROResultMaster.OTPAttempts,
                          IsLocked = joined.AROResultMaster.IsLocked,
                          ElectionTypeMasterId = joined.AROResultMaster.ElectionTypeMasterId,
                          ElectionTypeName = etm.ElectionType
                      })
                .FirstOrDefaultAsync();

            return aroRecord;
        }
        public async Task<Response> UpdateAROResult(AROResultMaster aROResultMaster)
        {
            // Fetch the existing officer and check for uniqueness of the mobile number in one query
            var existingOfficer = await _context.AROResultMaster
                .Where(d => d.AROMasterId == aROResultMaster.AROMasterId)
                .Select(d => new
                {
                    Officer = d,
                    IsMobileDuplicate = _context.AROResultMaster.Any(m =>
                        m.AROMobile == aROResultMaster.AROMobile
                        && m.AROMasterId != d.AROMasterId && m.ElectionTypeMasterId == aROResultMaster.ElectionTypeMasterId),
                    ExistingOfficerWithSameMobile = _context.FieldOfficerMaster.FirstOrDefault(m =>
                        m.FieldOfficerMobile == aROResultMaster.AROMobile
                        && m.ElectionTypeMasterId == aROResultMaster.ElectionTypeMasterId
                        && m.StateMasterId == aROResultMaster.StateMasterId
                        && m.DistrictMasterId == aROResultMaster.DistrictMasterId && m.ElectionTypeMasterId == aROResultMaster.ElectionTypeMasterId && m.ElectionTypeMasterId == aROResultMaster.ElectionTypeMasterId)
                })
                .FirstOrDefaultAsync();

            if (existingOfficer == null)
            {
                return new Response
                {
                    Status = RequestStatusEnum.BadRequest,
                    Message = "No Record Found"
                };
            }



            if (aROResultMaster.AROMobile != existingOfficer.Officer.AROMobile
                && existingOfficer.IsMobileDuplicate)
            {
                return new Response
                {
                    Status = RequestStatusEnum.BadRequest,
                    Message = "Mobile Number Already Exists"
                };
            }

            // Map updated fields from updatedFieldOfficer to the existing officer
            existingOfficer.Officer.StateMasterId = aROResultMaster.StateMasterId;
            existingOfficer.Officer.DistrictMasterId = aROResultMaster.DistrictMasterId;
            existingOfficer.Officer.AssemblyMasterId = aROResultMaster.AssemblyMasterId;
            existingOfficer.Officer.FourthLevelHMasterId = aROResultMaster.FourthLevelHMasterId;
            existingOfficer.Officer.AROName = aROResultMaster.AROName;
            existingOfficer.Officer.AROMobile = aROResultMaster.AROMobile;
            existingOfficer.Officer.ARODesignation = aROResultMaster.ARODesignation;
            existingOfficer.Officer.AROOfficeName = aROResultMaster.AROOfficeName;
            existingOfficer.Officer.AROUpdatedAt = BharatDateTime();
            existingOfficer.Officer.IsStatus = aROResultMaster.IsStatus;
            existingOfficer.Officer.OTPGeneratedTime = aROResultMaster.OTPGeneratedTime;
            existingOfficer.Officer.OTP = aROResultMaster.OTP;
            existingOfficer.Officer.OTPExpireTime = aROResultMaster.OTPExpireTime;
            existingOfficer.Officer.OTPAttempts = aROResultMaster.OTPAttempts;
            existingOfficer.Officer.RefreshToken = aROResultMaster.RefreshToken;
            existingOfficer.Officer.RefreshTokenExpiryTime = aROResultMaster.RefreshTokenExpiryTime;
            existingOfficer.Officer.AppPin = aROResultMaster.AppPin;
            existingOfficer.Officer.IsLocked = aROResultMaster.IsLocked;
            existingOfficer.Officer.ElectionTypeMasterId = aROResultMaster.ElectionTypeMasterId;

            _context.AROResultMaster.Update(existingOfficer.Officer);
            await _context.SaveChangesAsync();

            return new Response
            {
                Status = RequestStatusEnum.OK,
                Message = "Field Officer updated successfully"
            };
        }
        public async Task<Response> UpdateAROValidate(AROResultMaster aROResultMaster)
        {
            // Check if the record exists based on the FieldOfficerMasterId
            var existingOfficer = await _context.AROResultMaster
                                                .FirstOrDefaultAsync(d => d.AROMasterId == aROResultMaster.AROMasterId);

            if (existingOfficer == null)
            {
                // Return a response if the record is not found
                return new Response
                {
                    Status = RequestStatusEnum.BadRequest,
                    Message = "No Record Found"
                };
            }

            // Map all fields from updatedFieldOfficer to existingOfficer
            existingOfficer.StateMasterId = aROResultMaster.StateMasterId;
            existingOfficer.DistrictMasterId = aROResultMaster.DistrictMasterId;
            existingOfficer.AssemblyMasterId = aROResultMaster.AssemblyMasterId;
            existingOfficer.AROName = aROResultMaster.AROName;
            existingOfficer.ARODesignation = aROResultMaster.ARODesignation;
            existingOfficer.AROOfficeName = aROResultMaster.AROOfficeName;
            existingOfficer.AROMobile = aROResultMaster.AROMobile;
            existingOfficer.AROUpdatedAt = BharatDateTime(); // Set the updated time to the current time
            existingOfficer.IsStatus = aROResultMaster.IsStatus;
            existingOfficer.OTPGeneratedTime = aROResultMaster.OTPGeneratedTime;
            existingOfficer.OTP = aROResultMaster.OTP;
            existingOfficer.OTPExpireTime = aROResultMaster.OTPExpireTime;
            existingOfficer.OTPAttempts = aROResultMaster.OTPAttempts;
            existingOfficer.RefreshToken = aROResultMaster.RefreshToken;
            existingOfficer.RefreshTokenExpiryTime = aROResultMaster.RefreshTokenExpiryTime;
            existingOfficer.AppPin = aROResultMaster.AppPin;
            existingOfficer.IsLocked = aROResultMaster.IsLocked;
            existingOfficer.ElectionTypeMasterId = aROResultMaster.ElectionTypeMasterId;

            _context.AROResultMaster.Update(aROResultMaster);

            _context.SaveChanges();

            // Return a success response
            return new Response
            {
                Status = RequestStatusEnum.OK,
                Message = "Field Officer updated successfully"
            };
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
                                    ElectionTypeName = elec.ElectionType,


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
        public async Task<List<CombinedMaster>> GetBoothListByFourthLevelId(int stateMasterId,
            int districtMasterId,
            int assemblyMasterId,
            int fourthLevelHMasterId)
        {


            // Fetch the relevant FourthLevelH entity along with necessary relations
            var isActive = await _context.FourthLevelH
                .Include(f => f.StateMaster)
                .Include(f => f.DistrictMaster)
                .Include(f => f.AssemblyMaster)
                .FirstOrDefaultAsync(f => f.StateMasterId == stateMasterId
                                          && f.DistrictMasterId == districtMasterId
                                          && f.AssemblyMasterId == assemblyMasterId
                                          && f.FourthLevelHMasterId == fourthLevelHMasterId);

            // Check for null and status before proceeding
            if (isActive == null ||
                !isActive.StateMaster.StateStatus ||
                !isActive.DistrictMaster.DistrictStatus ||
                !isActive.AssemblyMaster.AssemblyStatus ||
                !isActive.HierarchyStatus)
            {
                return null; // Return null if any status check fails
            }

            // Perform the main query
            IQueryable<CombinedMaster> boothListQuery = from bt in _context.BoothMaster
                                                        join asem in _context.AssemblyMaster
                                                            on bt.AssemblyMasterId equals asem.AssemblyMasterId
                                                        join dist in _context.DistrictMaster
                                                            on asem.DistrictMasterId equals dist.DistrictMasterId
                                                        join state in _context.StateMaster
                                                            on dist.StateMasterId equals state.StateMasterId
                                                        join elec in _context.ElectionTypeMaster
                                                            on bt.ElectionTypeMasterId equals elec.ElectionTypeMasterId
                                                        join frth in _context.FourthLevelH
                                                            on bt.FourthLevelHMasterId equals frth.FourthLevelHMasterId
                                                        where bt.StateMasterId == stateMasterId
                                                          && bt.DistrictMasterId == districtMasterId
                                                          && bt.AssemblyMasterId == assemblyMasterId
                                                          && bt.FourthLevelHMasterId == fourthLevelHMasterId
                                                        select new CombinedMaster
                                                        {
                                                            StateId = stateMasterId,
                                                            DistrictId = dist.DistrictMasterId,
                                                            AssemblyId = asem.AssemblyMasterId,
                                                            AssemblyName = asem.AssemblyName,
                                                            AssemblyCode = asem.AssemblyCode,
                                                            BoothMasterId = bt.BoothMasterId,
                                                            BoothName = $"{bt.BoothName}{(bt.BoothNoAuxy != "0" ? $"-{bt.BoothNoAuxy}" : "")}({bt.BoothCode_No})",
                                                            SecondLanguage = bt.SecondLanguage,
                                                            BoothAuxy = bt.BoothNoAuxy,
                                                            BoothCode_No = bt.BoothCode_No,
                                                            IsAssigned = bt.IsAssigned,
                                                            IsStatus = bt.BoothStatus,
                                                            LocationMasterId = bt.LocationMasterId,
                                                            ElectionTypeMasterId = bt.ElectionTypeMasterId,
                                                            ElectionTypeName = elec.ElectionType,
                                                            FourthLevelHMasterId = bt.FourthLevelHMasterId,
                                                            FourthLevelHName = frth.HierarchyName,
                                                            IsPrimaryBooth = bt.IsPrimaryBooth
                                                        };

            // Fetch and sort the booth list
            var boothListResult = await boothListQuery.ToListAsync();

            // Sort by BoothCode_No if it's convertible to an integer, otherwise place it at the end
            var sortedBoothList = boothListResult
                .OrderBy(d => int.TryParse(d.BoothCode_No, out int code) ? code : int.MaxValue)
                .ToList();

            return sortedBoothList;
        }
        public async Task<List<CombinedMaster>> GetBoothListByPSZonePanchayatId(
            int stateMasterId,
            int districtMasterId,
            int assemblyMasterId,
            int fourthLevelHMasterId,
            int psZonePanchayatMasterId)
        {


            var isActive = await _context.PSZonePanchayat
                .Include(f => f.StateMaster)
                .Include(f => f.DistrictMaster)
                .Include(f => f.AssemblyMaster)
                .Include(d => d.FourthLevelH)
                .FirstOrDefaultAsync(f => f.StateMasterId == stateMasterId
                                          && f.DistrictMasterId == districtMasterId
                                          && f.AssemblyMasterId == assemblyMasterId
                                          && f.FourthLevelHMasterId == fourthLevelHMasterId && f.PSZonePanchayatMasterId == psZonePanchayatMasterId);

            // Check if all required statuses are active
            if (isActive == null ||
                !isActive.StateMaster.StateStatus ||
                !isActive.DistrictMaster.DistrictStatus ||
                !isActive.AssemblyMaster.AssemblyStatus ||
                !isActive.FourthLevelH.HierarchyStatus || !isActive.PSZonePanchayatStatus)
            {
                return null; // Return null if any status check fails
            }

            // Query the BoothMaster table based on provided filters
            IQueryable<CombinedMaster> boothListQuery = from bt in _context.BoothMaster
                .Where(bt => bt.StateMasterId == stateMasterId
                             && bt.DistrictMasterId == districtMasterId
                             && bt.AssemblyMasterId == assemblyMasterId
                             && bt.FourthLevelHMasterId == fourthLevelHMasterId
                             && bt.PSZonePanchayatMasterId == psZonePanchayatMasterId)
                                                        join asem in _context.AssemblyMaster on bt.AssemblyMasterId equals asem.AssemblyMasterId
                                                        join dist in _context.DistrictMaster on asem.DistrictMasterId equals dist.DistrictMasterId
                                                        join state in _context.StateMaster on dist.StateMasterId equals state.StateMasterId
                                                        join elec in _context.ElectionTypeMaster on bt.ElectionTypeMasterId equals elec.ElectionTypeMasterId
                                                        join frth in _context.FourthLevelH on bt.FourthLevelHMasterId equals frth.FourthLevelHMasterId
                                                        join zp in _context.PSZonePanchayat on bt.PSZonePanchayatMasterId equals zp.PSZonePanchayatMasterId into zpJoin
                                                        from zp in zpJoin.DefaultIfEmpty() // Left join to handle null cases
                                                        select new CombinedMaster
                                                        {
                                                            StateId = stateMasterId,
                                                            DistrictId = dist.DistrictMasterId,
                                                            AssemblyId = asem.AssemblyMasterId,
                                                            AssemblyName = asem.AssemblyName,
                                                            AssemblyCode = asem.AssemblyCode,
                                                            BoothMasterId = bt.BoothMasterId,
                                                            BoothName = $"{bt.BoothName}{(bt.BoothNoAuxy != "0" ? $"-{bt.BoothNoAuxy}" : "")}({bt.BoothCode_No})",
                                                            SecondLanguage = bt.SecondLanguage,
                                                            BoothAuxy = bt.BoothNoAuxy,
                                                            BoothCode_No = bt.BoothCode_No,
                                                            IsAssigned = bt.IsAssigned,
                                                            IsStatus = bt.BoothStatus,
                                                            LocationMasterId = bt.LocationMasterId,
                                                            ElectionTypeMasterId = bt.ElectionTypeMasterId,
                                                            ElectionTypeName = elec.ElectionType,
                                                            FourthLevelHMasterId = bt.FourthLevelHMasterId,
                                                            FourthLevelHName = frth.HierarchyName,
                                                            PSZoneMasterId = bt.PSZonePanchayatMasterId,
                                                            PSZonePanchayatName = zp != null ? zp.PSZonePanchayatName : null,
                                                            IsPrimaryBooth = bt.IsPrimaryBooth
                                                        };

            // Fetch and sort by BoothCode_No (parsed as int where possible)
            var boothList = await boothListQuery.ToListAsync();
            boothList = boothList.OrderBy(b => int.TryParse(b.BoothCode_No, out int code) ? code : int.MaxValue).ToList();

            return boothList;
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

        public async Task<List<CombinedMaster>> GetUnassignedBoothListById(int stateMasterId, int districtMasterId, int assemblyMasterId)
        {
            var isStateActive = await _context.StateMaster.FirstOrDefaultAsync(d => d.StateMasterId == stateMasterId);
            var isDistrictActive = await _context.DistrictMaster.FirstOrDefaultAsync(d => d.StateMasterId == stateMasterId && d.DistrictMasterId == districtMasterId);
            var isAssemblyActive = await _context.AssemblyMaster.FirstOrDefaultAsync(d => d.StateMasterId == stateMasterId && d.DistrictMasterId == districtMasterId && d.AssemblyMasterId == assemblyMasterId);
            if (isStateActive.StateStatus && isDistrictActive.DistrictStatus && isAssemblyActive.AssemblyStatus)
            {

                var boothlist = from bt in _context.BoothMaster.Where(d => d.StateMasterId == stateMasterId && d.DistrictMasterId == districtMasterId && d.AssemblyMasterId == assemblyMasterId && d.IsAssigned == false && d.BoothStatus == true) // outer sequenc)
                                join fourthLevelH in _context.FourthLevelH on bt.FourthLevelHMasterId equals fourthLevelH.FourthLevelHMasterId
                                join asem in _context.AssemblyMaster
                                            on bt.AssemblyMasterId equals asem.AssemblyMasterId
                                join dist in _context.DistrictMaster
                                on asem.DistrictMasterId equals dist.DistrictMasterId
                                join state in _context.StateMaster
                                 on dist.StateMasterId equals state.StateMasterId
                                orderby bt.BoothNoAuxy
                                select new CombinedMaster
                                {
                                    StateId = stateMasterId,
                                    DistrictId = dist.DistrictMasterId,
                                    AssemblyId = asem.AssemblyMasterId,
                                    AssemblyName = asem.AssemblyName,
                                    AssemblyCode = asem.AssemblyCode,
                                    FourthLevelHMasterId = fourthLevelH.FourthLevelHMasterId,
                                    FourthLevelHName = fourthLevelH.HierarchyName,
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
        public async Task<Response> AddBooth2(BoothMaster boothMaster)
        {
            try
            {
                if (boothMaster == null)
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Booth master data is null" };

                if (boothMaster.BoothNoAuxy == "0" + "")

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
        public async Task<Response> AddBooth(BoothMaster boothMaster)
        {
            try
            {
                if (boothMaster == null)
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Booth master data is null" };
                //check full hierchythatids are in order or not
                bool recordExistOfMasterIds = false;
                if (boothMaster.ElectionTypeMasterId == 2)
                {
                    recordExistOfMasterIds = await _context.PSZonePanchayat.AnyAsync(d =>
                                        d.StateMasterId == boothMaster.StateMasterId &&
                                        d.DistrictMasterId == boothMaster.DistrictMasterId &&
                                        d.AssemblyMasterId == boothMaster.AssemblyMasterId &&
                                        d.ElectionTypeMasterId == boothMaster.ElectionTypeMasterId &&
                                        d.FourthLevelHMasterId == boothMaster.FourthLevelHMasterId &&
                                        d.PSZonePanchayatMasterId == boothMaster.PSZonePanchayatMasterId);
                }
                else
                {
                    recordExistOfMasterIds = await _context.FourthLevelH.AnyAsync(d =>
                                         d.StateMasterId == boothMaster.StateMasterId &&
                                         d.DistrictMasterId == boothMaster.DistrictMasterId &&
                                         d.AssemblyMasterId == boothMaster.AssemblyMasterId &&
                                         d.ElectionTypeMasterId == boothMaster.ElectionTypeMasterId &&
                                         d.FourthLevelHMasterId == boothMaster.FourthLevelHMasterId);
                }
                if (recordExistOfMasterIds == true)

                {
                    var auxList = new List<string>()
                    {"0",
                    "A",
                        "B",
                        "C",
                        "D",
                        "E"
                    };
                    if (auxList.Any(aux => boothMaster.BoothNoAuxy.Contains(aux)))
                    {
                        bool checkBoothName = false;

                        if (boothMaster.ElectionTypeMasterId == 2) // for panchayat samiti chekc pszonepachat table
                        {

                            checkBoothName = await _context.BoothMaster.AnyAsync(d =>
                                             d.StateMasterId == boothMaster.StateMasterId &&
                                             d.DistrictMasterId == boothMaster.DistrictMasterId &&
                                             d.AssemblyMasterId == boothMaster.AssemblyMasterId &&
                                             d.ElectionTypeMasterId == boothMaster.ElectionTypeMasterId &&
                                             d.FourthLevelHMasterId == boothMaster.FourthLevelHMasterId &&
                                             d.BoothCode_No.Equals(boothMaster.BoothCode_No)
                                             && d.PSZonePanchayatMasterId == boothMaster.PSZonePanchayatMasterId);
                        }
                        else
                        {

                            checkBoothName = await _context.BoothMaster.AnyAsync(d =>
                                             d.StateMasterId == boothMaster.StateMasterId &&
                                             d.DistrictMasterId == boothMaster.DistrictMasterId &&
                                             d.AssemblyMasterId == boothMaster.AssemblyMasterId &&
                                             d.ElectionTypeMasterId == boothMaster.ElectionTypeMasterId &&
                                              d.FourthLevelHMasterId == boothMaster.FourthLevelHMasterId &&
                                             d.BoothCode_No.Equals(boothMaster.BoothCode_No));
                        }

                        if (checkBoothName is true)
                            return new Response { Status = RequestStatusEnum.BadRequest, Message = $"The booth Code {boothMaster.BoothCode_No} already exists. You can proceed with an auxiliary booth instead of {boothMaster.BoothNoAuxy} " };
                        else
                            //
                            if (boothMaster.ElectionTypeMasterId == 1)//gram
                        {
                            var existingBooths = await _context.BoothMaster.Where(p =>

                             p.StateMasterId == boothMaster.StateMasterId &&
                             p.AssemblyMasterId == boothMaster.AssemblyMasterId &&
                             p.ElectionTypeMasterId == boothMaster.ElectionTypeMasterId && p.FourthLevelHMasterId == boothMaster.FourthLevelHMasterId).ToListAsync();
                            if (existingBooths.Any())
                            {
                                if (existingBooths.Count > 0 && boothMaster.IsPrimaryBooth == true)
                                {
                                    foreach (var existingBooth in existingBooths)
                                    {
                                        existingBooth.IsPrimaryBooth = false;
                                        _context.BoothMaster.Update(existingBooth);
                                    }
                                    boothMaster.BoothCreatedAt = BharatDateTime();
                                    _context.BoothMaster.Add(boothMaster);
                                    await _context.SaveChangesAsync();
                                    return new Response { Status = RequestStatusEnum.OK, Message = $"Booth {boothMaster.BoothName} added successfully!" };
                                }
                                else
                                {// as it is save otherwise
                                    boothMaster.BoothCreatedAt = BharatDateTime(); // Assuming BharatDateTime() returns the current date/time.
                                    _context.BoothMaster.Add(boothMaster);
                                    await _context.SaveChangesAsync();
                                    return new Response { Status = RequestStatusEnum.OK, Message = $"Booth {boothMaster.BoothName} added successfully!" };

                                }
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
                            boothMaster.BoothCreatedAt = BharatDateTime(); // Assuming BharatDateTime() returns the current date/time.
                            _context.BoothMaster.Add(boothMaster);
                            await _context.SaveChangesAsync();
                            return new Response { Status = RequestStatusEnum.OK, Message = $"Booth {boothMaster.BoothName} added successfully!" };
                        }

                    }
                    else
                    {
                        //check as per panch
                        //ayt samiti electiontype isprimary booth....update old if they are isprimary
                        List<BoothMaster> existingBooths = null;
                        if (boothMaster.ElectionTypeMasterId == 2)
                        {
                            existingBooths = await _context.BoothMaster.Where(p =>
                                        p.BoothCode_No == boothMaster.BoothCode_No &&
                                         p.StateMasterId == boothMaster.StateMasterId &&
                                         p.AssemblyMasterId == boothMaster.AssemblyMasterId &&
                                         p.ElectionTypeMasterId == boothMaster.ElectionTypeMasterId && p.FourthLevelHMasterId == boothMaster.FourthLevelHMasterId && p.PSZonePanchayatMasterId == boothMaster.PSZonePanchayatMasterId).ToListAsync();


                            //if (assemblyActive)
                            //{
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
                            //}
                            //else
                            //{
                            //    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Assembly is not active for this booth. Kindly activate the Assembly in order to Add Booth." };
                            //}

                        }
                        else
                        { // for other elections
                            existingBooths = await _context.BoothMaster.Where(p =>
                            p.BoothCode_No == boothMaster.BoothCode_No &&
                             p.StateMasterId == boothMaster.StateMasterId &&
                             p.AssemblyMasterId == boothMaster.AssemblyMasterId &&
                             p.ElectionTypeMasterId == boothMaster.ElectionTypeMasterId && p.FourthLevelHMasterId == boothMaster.FourthLevelHMasterId).ToListAsync();
                        }


                        if (existingBooths.Any())
                        {
                            var existingAuxCodes = existingBooths.Select(b => new { b.BoothNoAuxy, b.BoothCode_No });
                            if (existingAuxCodes.Any(c => c.BoothNoAuxy.Equals(boothMaster.BoothNoAuxy) && c.BoothCode_No.Equals(boothMaster.BoothCode_No)))
                            {
                                return new Response { Status = RequestStatusEnum.BadRequest, Message = $"{boothMaster.BoothName} with AuxilaryCode {boothMaster.BoothNoAuxy} and BoothCode {boothMaster.BoothCode_No} already exists" };
                            }
                            else
                            {//here wehn aux added by user with unique 

                                if (boothMaster.ElectionTypeMasterId == 1)
                                {

                                    // Update existing booths if new booth is set true primary
                                    if (existingBooths.Count > 0 && boothMaster.IsPrimaryBooth == true)
                                    {
                                        foreach (var existingBooth in existingBooths)
                                        {
                                            existingBooth.IsPrimaryBooth = false;
                                            _context.BoothMaster.Update(existingBooth);
                                        }
                                        boothMaster.BoothCreatedAt = BharatDateTime();
                                        _context.BoothMaster.Add(boothMaster);
                                        await _context.SaveChangesAsync();
                                        return new Response { Status = RequestStatusEnum.OK, Message = $"Booth {boothMaster.BoothName} added successfully!" };
                                    }
                                    else
                                    {// as it is save otherwise
                                        boothMaster.BoothCreatedAt = BharatDateTime(); // Assuming BharatDateTime() returns the current date/time.
                                        _context.BoothMaster.Add(boothMaster);
                                        await _context.SaveChangesAsync();
                                        return new Response { Status = RequestStatusEnum.OK, Message = $"Booth {boothMaster.BoothName} added successfully!" };

                                    }
                                }


                                else
                                {
                                    boothMaster.BoothCreatedAt = BharatDateTime(); // Assuming BharatDateTime() returns the current date/time.
                                    _context.BoothMaster.Add(boothMaster);
                                    await _context.SaveChangesAsync();
                                    return new Response { Status = RequestStatusEnum.OK, Message = $"Booth {boothMaster.BoothName} added successfully!" };

                                }


                            }
                        }
                        else
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

                    }
                }
                else
                {
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Please check the hierarchy; the record does not exist due to incorrect order." };
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
        //                if (existingbooth != null)
        //                {
        //                    if (boothMaster.ElectionTypeMasterId != null)
        //                    {
        //                        var electionAssemblyTypeId = _context.AssemblyMaster
        //                            .Where(s => s.AssemblyMasterId == boothMaster.AssemblyMasterId)
        //                            .Select(s => s.ElectionTypeMasterId)
        //                            .FirstOrDefault(); // Assuming you expect only one result or want the first one


        //                        if (boothMaster.ElectionTypeMasterId == electionAssemblyTypeId)
        //                        {
        //                            var electionInfoRecord = _context.ElectionInfoMaster
        //                                  .Where(d => d.StateMasterId == boothMaster.StateMasterId && d.DistrictMasterId == boothMaster.DistrictMasterId && d.AssemblyMasterId == boothMaster.AssemblyMasterId && d.BoothMasterId == boothMaster.BoothMasterId)
        //                                .FirstOrDefault();

        //                            //means election_info null,also booth not mapped
        //                            if (electionInfoRecord == null && (existingbooth.AssignedTo == null || existingbooth.AssignedTo == ""))
        //                            {
        //                                if (boothMaster.BoothStatus == false)
        //                                {
        //                                    //check if Isprimary=true,update others false

        //                                    existingbooth.LocationMasterId = null;
        //                                    existingbooth.BoothName = boothMaster.BoothName;
        //                                    existingbooth.BoothCode_No = boothMaster.BoothCode_No;
        //                                    existingbooth.ElectionTypeMasterId = boothMaster.ElectionTypeMasterId;
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
        //                                    return new Response { Status = RequestStatusEnum.OK, Message = "Booth Record Updated Sucessfully, and Booth is Unmapped from Location." };

        //                                }


        //                                else if (boothMaster.BoothStatus == true)

        //                                {
        //                                    var isassmblytrue = _context.AssemblyMaster.Any(s => s.AssemblyMasterId == boothMaster.AssemblyMasterId && s.AssemblyStatus == true);
        //                                    if (isassmblytrue == false)
        //                                    {
        //                                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Assembly must be active and Booth Location can't be null in order to activate Booth." };

        //                                    }
        //                                    else
        //                                    {
        //                                        //check if Isprimary=true,update others false
        //                                        existingbooth.BoothName = boothMaster.BoothName;
        //                                        existingbooth.BoothCode_No = boothMaster.BoothCode_No;
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


        //                                    }
        //                                }

        //                                else
        //                                {
        //                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Please Select Active/InActive Status" };

        //                                }
        //                            }

        //                            //means info null, but booth mapped, that case 4 fields can editable--> changed method
        //                            else if (electionInfoRecord == null && existingbooth.AssignedTo != null)
        //                            {  // can update only 4 fields
        //                                if (existingbooth.BoothName == boothMaster.BoothName && existingbooth.BoothCode_No == boothMaster.BoothCode_No &&
        //                                    existingbooth.DistrictMasterId == boothMaster.DistrictMasterId && existingbooth.AssemblyMasterId == boothMaster.AssemblyMasterId
        //                                    && existingbooth.BoothStatus == boothMaster.BoothStatus && existingbooth.BoothNoAuxy == boothMaster.BoothNoAuxy && existingbooth.ElectionTypeMasterId == boothMaster.ElectionTypeMasterId)
        //                                {

        //                                    if (boothMaster.BoothStatus == false)
        //                                    {
        //                                        existingbooth.LocationMasterId = null;
        //                                        existingbooth.BoothUpdatedAt = BharatDateTime();
        //                                        existingbooth.ElectionTypeMasterId = boothMaster.ElectionTypeMasterId;
        //                                        existingbooth.TotalVoters = boothMaster.TotalVoters;
        //                                        existingbooth.Male = boothMaster.Male;
        //                                        existingbooth.Female = boothMaster.Female;
        //                                        existingbooth.Transgender = boothMaster.Transgender;
        //                                        _context.BoothMaster.Update(existingbooth);
        //                                        await _context.SaveChangesAsync();
        //                                        //return new Response { Status = RequestStatusEnum.OK, Message = "Booth Updated successfully except BoothName/Number/District/Assembly/Status/Aux Booth is Unmapped from Location and Booth is Inactive." };
        //                                        return new Response { Status = RequestStatusEnum.OK, Message = "Booth Updated successfully except BoothName/Number/District/Assembly/Status/Aux. Kindly Release Booth in order to update all fields" };

        //                                    }
        //                                    else if (boothMaster.BoothStatus == true)

        //                                    {
        //                                        var isassmblytrue = _context.AssemblyMaster.Any(s => s.AssemblyMasterId == boothMaster.AssemblyMasterId && s.AssemblyStatus == true);
        //                                        if (isassmblytrue == false)
        //                                        {
        //                                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Assembly must be active and Booth Location can't be null in order to activate Booth." };

        //                                        }
        //                                        else
        //                                        {


        //                                            existingbooth.ElectionTypeMasterId = boothMaster.ElectionTypeMasterId;
        //                                            existingbooth.BoothUpdatedAt = BharatDateTime();
        //                                            existingbooth.TotalVoters = boothMaster.TotalVoters;
        //                                            existingbooth.Male = boothMaster.Male;
        //                                            existingbooth.Female = boothMaster.Female;
        //                                            existingbooth.Transgender = boothMaster.Transgender;

        //                                            _context.BoothMaster.Update(existingbooth);
        //                                            await _context.SaveChangesAsync();
        //                                            return new Response { Status = RequestStatusEnum.OK, Message = "Booth Updated successfully except BoothName/Number/District/Assembly/Status/Aux. Kindly Release Booth in order to update all fields" };



        //                                        }
        //                                    }
        //                                    else
        //                                    {
        //                                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Please Select Active/InActive Status" };

        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = " Kindly Release Booth in order to update fields." };
        //                                }
        //                            }

        //                            else if (electionInfoRecord != null)
        //                            {
        //                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Election Info Record found aganist this Booth, thus can't change status" };

        //                            }
        //                            else
        //                            {
        //                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Problem while Updating Booth" };

        //                            }
        //                        }
        //                        else
        //                        {
        //                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "The election types of Booth and assembly are different." };

        //                        }
        //                    }
        //                    else
        //                    {
        //                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Election Type Can't be Null" };

        //                    }

        //                    //end
        //                }
        //                else
        //                {
        //                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Booth record Not Found." };

        //                }


        //                //}
        //                //else
        //                //{
        //                //    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Booth with Same Code Already Exists in the State: " + string.Join(", ", isExist.Select(p => $"{p.BoothName} ({p.BoothCode_No})")) };
        //                //}




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
                        if (existingbooth != null)
                        {
                            if (boothMaster.ElectionTypeMasterId != null)
                            {
                                var electionAssemblyTypeId = _context.AssemblyMaster
                                    .Where(s => s.AssemblyMasterId == boothMaster.AssemblyMasterId)
                                    .Select(s => s.ElectionTypeMasterId)
                                    .FirstOrDefault(); // Assuming you expect only one result or want the first one


                                if (boothMaster.ElectionTypeMasterId == electionAssemblyTypeId)
                                {
                                    var electionInfoRecord = _context.ElectionInfoMaster
                                          .Where(d => d.StateMasterId == boothMaster.StateMasterId && d.DistrictMasterId == boothMaster.DistrictMasterId && d.AssemblyMasterId == boothMaster.AssemblyMasterId && d.BoothMasterId == boothMaster.BoothMasterId)
                                        .FirstOrDefault();
                                    //check if booths of Gram Panchyat case
                                    //{ 
                                    //means election_info null,also booth not mapped
                                    if (electionInfoRecord == null && (existingbooth.AssignedTo == null || existingbooth.AssignedTo == ""))
                                    {
                                        if (boothMaster.BoothStatus == false)
                                        {
                                            //check if Isprimary=true,update others false
                                            if (boothMaster.ElectionTypeMasterId == 1)// for gram
                                            {
                                                var existingBooths = await _context.BoothMaster.Where(p =>

                                                        p.StateMasterId == boothMaster.StateMasterId &&
                                                        p.AssemblyMasterId == boothMaster.AssemblyMasterId &&
                                                        p.ElectionTypeMasterId == boothMaster.ElectionTypeMasterId
                                                        && p.FourthLevelHMasterId == boothMaster.FourthLevelHMasterId).ToListAsync();

                                                if (existingBooths.Any())
                                                {
                                                    if (existingBooths.Count > 0 && boothMaster.IsPrimaryBooth == true)
                                                    {
                                                        foreach (var boothrecord in existingBooths)
                                                        {
                                                            boothrecord.IsPrimaryBooth = false;
                                                            _context.BoothMaster.Update(boothrecord);
                                                        }

                                                        // Update the single existing booth
                                                        existingbooth.LocationMasterId = null;
                                                        existingbooth.BoothName = boothMaster.BoothName;
                                                        existingbooth.BoothCode_No = boothMaster.BoothCode_No;
                                                        existingbooth.ElectionTypeMasterId = boothMaster.ElectionTypeMasterId;
                                                        existingbooth.Longitude = boothMaster.Longitude;
                                                        existingbooth.Latitude = boothMaster.Latitude;
                                                        existingbooth.BoothUpdatedAt = BharatDateTime();
                                                        existingbooth.TotalVoters = boothMaster.TotalVoters;
                                                        existingbooth.BoothStatus = boothMaster.BoothStatus;
                                                        existingbooth.Male = boothMaster.Male;
                                                        existingbooth.Female = boothMaster.Female;
                                                        existingbooth.Transgender = boothMaster.Transgender;
                                                        existingbooth.IsPrimaryBooth = boothMaster.IsPrimaryBooth;
                                                        existingbooth.BoothNoAuxy = boothMaster.BoothNoAuxy;
                                                        _context.BoothMaster.Update(existingbooth);

                                                        // Save all changes to the database
                                                        await _context.SaveChangesAsync();

                                                        return new Response { Status = RequestStatusEnum.OK, Message = "Booth Records Updated Successfully, and Booths are Unmapped from Location." };

                                                    }
                                                    else
                                                    {
                                                        existingbooth.LocationMasterId = null;
                                                        existingbooth.BoothName = boothMaster.BoothName;
                                                        existingbooth.BoothCode_No = boothMaster.BoothCode_No;
                                                        existingbooth.ElectionTypeMasterId = boothMaster.ElectionTypeMasterId;
                                                        existingbooth.Longitude = boothMaster.Longitude;
                                                        existingbooth.Latitude = boothMaster.Latitude;
                                                        existingbooth.BoothUpdatedAt = BharatDateTime();
                                                        existingbooth.TotalVoters = boothMaster.TotalVoters;
                                                        existingbooth.BoothStatus = boothMaster.BoothStatus;
                                                        existingbooth.Male = boothMaster.Male;
                                                        existingbooth.Female = boothMaster.Female;
                                                        existingbooth.Transgender = boothMaster.Transgender;
                                                        existingbooth.IsPrimaryBooth = boothMaster.IsPrimaryBooth;
                                                        existingbooth.BoothNoAuxy = boothMaster.BoothNoAuxy;
                                                        _context.BoothMaster.Update(existingbooth);
                                                        await _context.SaveChangesAsync();
                                                        return new Response { Status = RequestStatusEnum.OK, Message = "Booth Record Updated Sucessfully, and Booth is Unmapped from Location." };

                                                    }


                                                }
                                                else
                                                {
                                                    existingbooth.LocationMasterId = null;
                                                    existingbooth.BoothName = boothMaster.BoothName;
                                                    existingbooth.BoothCode_No = boothMaster.BoothCode_No;
                                                    existingbooth.ElectionTypeMasterId = boothMaster.ElectionTypeMasterId;
                                                    existingbooth.Longitude = boothMaster.Longitude;
                                                    existingbooth.Latitude = boothMaster.Latitude;
                                                    existingbooth.BoothUpdatedAt = BharatDateTime();
                                                    existingbooth.TotalVoters = boothMaster.TotalVoters;
                                                    existingbooth.BoothStatus = boothMaster.BoothStatus;
                                                    existingbooth.Male = boothMaster.Male;
                                                    existingbooth.Female = boothMaster.Female;
                                                    existingbooth.Transgender = boothMaster.Transgender;
                                                    existingbooth.IsPrimaryBooth = boothMaster.IsPrimaryBooth;
                                                    existingbooth.BoothNoAuxy = boothMaster.BoothNoAuxy;
                                                    _context.BoothMaster.Update(existingbooth);
                                                    await _context.SaveChangesAsync();
                                                    return new Response { Status = RequestStatusEnum.OK, Message = "Booth Record Updated Sucessfully, and Booth is Unmapped from Location." };
                                                }
                                            }
                                            else
                                            {
                                                existingbooth.LocationMasterId = null;
                                                existingbooth.BoothName = boothMaster.BoothName;
                                                existingbooth.BoothCode_No = boothMaster.BoothCode_No;
                                                existingbooth.ElectionTypeMasterId = boothMaster.ElectionTypeMasterId;
                                                existingbooth.Longitude = boothMaster.Longitude;
                                                existingbooth.Latitude = boothMaster.Latitude;
                                                existingbooth.BoothUpdatedAt = BharatDateTime();
                                                existingbooth.TotalVoters = boothMaster.TotalVoters;
                                                existingbooth.BoothStatus = boothMaster.BoothStatus;
                                                existingbooth.Male = boothMaster.Male;
                                                existingbooth.Female = boothMaster.Female;
                                                existingbooth.Transgender = boothMaster.Transgender;
                                                existingbooth.IsPrimaryBooth = boothMaster.IsPrimaryBooth;
                                                existingbooth.BoothNoAuxy = boothMaster.BoothNoAuxy;
                                                _context.BoothMaster.Update(existingbooth);
                                                await _context.SaveChangesAsync();
                                                return new Response { Status = RequestStatusEnum.OK, Message = "Booth Record Updated Sucessfully, and Booth is Unmapped from Location." };
                                            }
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
                                                //check if Isprimary=true,update others false
                                                if (boothMaster.ElectionTypeMasterId == 1)// for gram
                                                {
                                                    var existingBooths = await _context.BoothMaster.Where(p =>

                                                p.StateMasterId == boothMaster.StateMasterId &&
                                                p.AssemblyMasterId == boothMaster.AssemblyMasterId &&
                                                p.ElectionTypeMasterId == boothMaster.ElectionTypeMasterId
                                                && p.FourthLevelHMasterId == boothMaster.FourthLevelHMasterId).ToListAsync();

                                                    if (existingBooths.Any())
                                                    {
                                                        if (existingBooths.Count > 0 && boothMaster.IsPrimaryBooth == true)
                                                        {
                                                            foreach (var boothrecord in existingBooths)
                                                            {
                                                                boothrecord.IsPrimaryBooth = false;
                                                                _context.BoothMaster.Update(boothrecord);
                                                            }

                                                            existingbooth.BoothName = boothMaster.BoothName;
                                                            existingbooth.BoothCode_No = boothMaster.BoothCode_No;
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
                                                            existingbooth.IsPrimaryBooth = boothMaster.IsPrimaryBooth;
                                                            existingbooth.BoothNoAuxy = boothMaster.BoothNoAuxy;
                                                            _context.BoothMaster.Update(existingbooth);
                                                            await _context.SaveChangesAsync();

                                                            return new Response { Status = RequestStatusEnum.OK, Message = "Booth Updated Successfully. Kindly Map your Booth Location." };


                                                        }
                                                        else
                                                        {
                                                            existingbooth.BoothName = boothMaster.BoothName;
                                                            existingbooth.BoothCode_No = boothMaster.BoothCode_No;
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
                                                            existingbooth.BoothNoAuxy = boothMaster.BoothNoAuxy;
                                                            _context.BoothMaster.Update(existingbooth);
                                                            await _context.SaveChangesAsync();

                                                            return new Response { Status = RequestStatusEnum.OK, Message = "Booth Updated Successfully. Kindly Map your Booth Location." };


                                                        }


                                                    }
                                                    else
                                                    {
                                                        existingbooth.BoothName = boothMaster.BoothName;
                                                        existingbooth.BoothCode_No = boothMaster.BoothCode_No;
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
                                                        existingbooth.IsPrimaryBooth = boothMaster.IsPrimaryBooth;
                                                        existingbooth.BoothNoAuxy = boothMaster.BoothNoAuxy;
                                                        _context.BoothMaster.Update(existingbooth);
                                                        await _context.SaveChangesAsync();

                                                        return new Response { Status = RequestStatusEnum.OK, Message = "Booth Updated Successfully. Kindly Map your Booth Location." };

                                                    }
                                                }
                                                else
                                                {
                                                    existingbooth.BoothName = boothMaster.BoothName;
                                                    existingbooth.BoothCode_No = boothMaster.BoothCode_No;
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
                                                    existingbooth.BoothNoAuxy = boothMaster.BoothNoAuxy;
                                                    _context.BoothMaster.Update(existingbooth);
                                                    await _context.SaveChangesAsync();

                                                    return new Response { Status = RequestStatusEnum.OK, Message = "Booth Updated Successfully. Kindly Map your Booth Location." };

                                                }



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
                                                //check if Isprimary=true,update others false
                                                if (boothMaster.ElectionTypeMasterId == 1)// for gram
                                                {
                                                    var existingBooths = await _context.BoothMaster.Where(p =>

                                                    p.StateMasterId == boothMaster.StateMasterId &&
                                                    p.AssemblyMasterId == boothMaster.AssemblyMasterId &&
                                                    p.ElectionTypeMasterId == boothMaster.ElectionTypeMasterId
                                                    && p.FourthLevelHMasterId == boothMaster.FourthLevelHMasterId).ToListAsync();

                                                    if (existingBooths.Any())
                                                    {
                                                        if (existingBooths.Count > 0 && boothMaster.IsPrimaryBooth == true)
                                                        {
                                                            foreach (var boothrecord in existingBooths)
                                                            {
                                                                boothrecord.IsPrimaryBooth = false;
                                                                _context.BoothMaster.Update(boothrecord);
                                                            }
                                                            existingbooth.LocationMasterId = null;
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
                                                        else
                                                        {
                                                            existingbooth.LocationMasterId = null;
                                                            existingbooth.BoothUpdatedAt = BharatDateTime();
                                                            existingbooth.ElectionTypeMasterId = boothMaster.ElectionTypeMasterId;
                                                            existingbooth.TotalVoters = boothMaster.TotalVoters;
                                                            existingbooth.Male = boothMaster.Male;
                                                            existingbooth.Female = boothMaster.Female;
                                                            existingbooth.Transgender = boothMaster.Transgender;
                                                            existingbooth.BoothNoAuxy = boothMaster.BoothNoAuxy;
                                                            _context.BoothMaster.Update(existingbooth);
                                                            await _context.SaveChangesAsync();
                                                            //return new Response { Status = RequestStatusEnum.OK, Message = "Booth Updated successfully except BoothName/Number/District/Assembly/Status/Aux Booth is Unmapped from Location and Booth is Inactive." };
                                                            return new Response { Status = RequestStatusEnum.OK, Message = "Booth Updated successfully except BoothName/Number/District/Assembly/Status/Aux. Kindly Release Booth in order to update all fields" };

                                                        }


                                                    }
                                                    else
                                                    {
                                                        existingbooth.LocationMasterId = null;
                                                        existingbooth.BoothUpdatedAt = BharatDateTime();
                                                        existingbooth.ElectionTypeMasterId = boothMaster.ElectionTypeMasterId;
                                                        existingbooth.TotalVoters = boothMaster.TotalVoters;
                                                        existingbooth.Male = boothMaster.Male;
                                                        existingbooth.Female = boothMaster.Female;
                                                        existingbooth.Transgender = boothMaster.Transgender;
                                                        existingbooth.BoothNoAuxy = boothMaster.BoothNoAuxy;
                                                        _context.BoothMaster.Update(existingbooth);
                                                        await _context.SaveChangesAsync();
                                                        //return new Response { Status = RequestStatusEnum.OK, Message = "Booth Updated successfully except BoothName/Number/District/Assembly/Status/Aux Booth is Unmapped from Location and Booth is Inactive." };
                                                        return new Response { Status = RequestStatusEnum.OK, Message = "Booth Updated successfully except BoothName/Number/District/Assembly/Status/Aux. Kindly Release Booth in order to update all fields" };
                                                    }
                                                }
                                                else
                                                {
                                                    existingbooth.LocationMasterId = null;
                                                    existingbooth.BoothUpdatedAt = BharatDateTime();
                                                    existingbooth.ElectionTypeMasterId = boothMaster.ElectionTypeMasterId;
                                                    existingbooth.TotalVoters = boothMaster.TotalVoters;
                                                    existingbooth.Male = boothMaster.Male;
                                                    existingbooth.Female = boothMaster.Female;
                                                    existingbooth.Transgender = boothMaster.Transgender;
                                                    existingbooth.BoothNoAuxy = boothMaster.BoothNoAuxy;
                                                    _context.BoothMaster.Update(existingbooth);
                                                    await _context.SaveChangesAsync();
                                                    //return new Response { Status = RequestStatusEnum.OK, Message = "Booth Updated successfully except BoothName/Number/District/Assembly/Status/Aux Booth is Unmapped from Location and Booth is Inactive." };
                                                    return new Response { Status = RequestStatusEnum.OK, Message = "Booth Updated successfully except BoothName/Number/District/Assembly/Status/Aux. Kindly Release Booth in order to update all fields" };
                                                }

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
                                                    //check if Isprimary=true,update others false
                                                    if (boothMaster.ElectionTypeMasterId == 1)// for gram
                                                    {
                                                        var existingBooths = await _context.BoothMaster.Where(p =>

            p.StateMasterId == boothMaster.StateMasterId &&
            p.AssemblyMasterId == boothMaster.AssemblyMasterId &&
            p.ElectionTypeMasterId == boothMaster.ElectionTypeMasterId && p.FourthLevelHMasterId == boothMaster.FourthLevelHMasterId).ToListAsync();

                                                        if (existingBooths.Any())
                                                        {
                                                            if (existingBooths.Count > 0 && boothMaster.IsPrimaryBooth == true)
                                                            {
                                                                foreach (var boothrecord in existingBooths)
                                                                {
                                                                    boothrecord.IsPrimaryBooth = false;
                                                                    _context.BoothMaster.Update(boothrecord);
                                                                }
                                                                existingbooth.ElectionTypeMasterId = boothMaster.ElectionTypeMasterId;
                                                                existingbooth.BoothUpdatedAt = BharatDateTime();
                                                                existingbooth.TotalVoters = boothMaster.TotalVoters;
                                                                existingbooth.Male = boothMaster.Male;
                                                                existingbooth.Female = boothMaster.Female;
                                                                existingbooth.Transgender = boothMaster.Transgender;
                                                                existingbooth.IsPrimaryBooth = boothMaster.IsPrimaryBooth;
                                                                existingbooth.BoothNoAuxy = boothMaster.BoothNoAuxy;
                                                                _context.BoothMaster.Update(existingbooth);
                                                                await _context.SaveChangesAsync();
                                                                return new Response { Status = RequestStatusEnum.OK, Message = "Booth Updated successfully except BoothName/Number/District/Assembly/Status/Aux. Kindly Release Booth in order to update all fields" };

                                                            }
                                                            else
                                                            {
                                                                existingbooth.ElectionTypeMasterId = boothMaster.ElectionTypeMasterId;
                                                                existingbooth.BoothUpdatedAt = BharatDateTime();
                                                                existingbooth.TotalVoters = boothMaster.TotalVoters;
                                                                existingbooth.Male = boothMaster.Male;
                                                                existingbooth.Female = boothMaster.Female;
                                                                existingbooth.Transgender = boothMaster.Transgender;
                                                                existingbooth.IsPrimaryBooth = boothMaster.IsPrimaryBooth;
                                                                existingbooth.BoothNoAuxy = boothMaster.BoothNoAuxy;
                                                                _context.BoothMaster.Update(existingbooth);
                                                                await _context.SaveChangesAsync();
                                                                return new Response { Status = RequestStatusEnum.OK, Message = "Booth Updated successfully except BoothName/Number/District/Assembly/Status/Aux. Kindly Release Booth in order to update all fields" };


                                                            }


                                                        }
                                                        else
                                                        {
                                                            existingbooth.ElectionTypeMasterId = boothMaster.ElectionTypeMasterId;
                                                            existingbooth.BoothUpdatedAt = BharatDateTime();
                                                            existingbooth.TotalVoters = boothMaster.TotalVoters;
                                                            existingbooth.Male = boothMaster.Male;
                                                            existingbooth.Female = boothMaster.Female;
                                                            existingbooth.Transgender = boothMaster.Transgender;
                                                            existingbooth.BoothNoAuxy = boothMaster.BoothNoAuxy;
                                                            _context.BoothMaster.Update(existingbooth);
                                                            await _context.SaveChangesAsync();
                                                            return new Response { Status = RequestStatusEnum.OK, Message = "Booth Updated successfully except BoothName/Number/District/Assembly/Status/Aux. Kindly Release Booth in order to update all fields" };

                                                        }
                                                    }
                                                    else
                                                    {
                                                        existingbooth.ElectionTypeMasterId = boothMaster.ElectionTypeMasterId;
                                                        existingbooth.BoothUpdatedAt = BharatDateTime();
                                                        existingbooth.TotalVoters = boothMaster.TotalVoters;
                                                        existingbooth.Male = boothMaster.Male;
                                                        existingbooth.Female = boothMaster.Female;
                                                        existingbooth.Transgender = boothMaster.Transgender;
                                                        existingbooth.IsPrimaryBooth = boothMaster.IsPrimaryBooth;
                                                        existingbooth.BoothNoAuxy = boothMaster.BoothNoAuxy;
                                                        _context.BoothMaster.Update(existingbooth);
                                                        await _context.SaveChangesAsync();
                                                        return new Response { Status = RequestStatusEnum.OK, Message = "Booth Updated successfully except BoothName/Number/District/Assembly/Status/Aux. Kindly Release Booth in order to update all fields" };

                                                    }




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
                                        //allow editing of isprimary only

                                        //return new Response { Status = RequestStatusEnum.BadRequest, Message = "Election Info Record found aganist this Booth, thus can't change status" };
                                        if (boothMaster.ElectionTypeMasterId == 1)// for gram
                                        {
                                            var existingBooths = await _context.BoothMaster.Where(p =>

p.StateMasterId == boothMaster.StateMasterId &&
p.AssemblyMasterId == boothMaster.AssemblyMasterId &&
p.ElectionTypeMasterId == boothMaster.ElectionTypeMasterId && p.FourthLevelHMasterId == boothMaster.FourthLevelHMasterId).ToListAsync();

                                            if (existingBooths.Any())
                                            {
                                                if (existingBooths.Count > 0 && boothMaster.IsPrimaryBooth == true)
                                                {
                                                    foreach (var boothrecord in existingBooths)
                                                    {
                                                        boothrecord.IsPrimaryBooth = false;
                                                        _context.BoothMaster.Update(boothrecord);
                                                    }
                                                    existingbooth.IsPrimaryBooth = boothMaster.IsPrimaryBooth;

                                                    _context.BoothMaster.Update(existingbooth);
                                                    await _context.SaveChangesAsync();
                                                    return new Response { Status = RequestStatusEnum.OK, Message = "Booth Updated successfully except BoothName/Number/District/Assembly/Status/Aux. Kindly Release Booth in order to update all fields" };

                                                }
                                                else
                                                {
                                                    existingbooth.IsPrimaryBooth = boothMaster.IsPrimaryBooth;

                                                    _context.BoothMaster.Update(existingbooth);
                                                    await _context.SaveChangesAsync();
                                                    return new Response { Status = RequestStatusEnum.OK, Message = "Booth Updated successfully except BoothName/Number/District/Assembly/Status/Aux. Kindly Release Booth in order to update all fields" };


                                                }


                                            }
                                            else
                                            {
                                                existingbooth.IsPrimaryBooth = boothMaster.IsPrimaryBooth;
                                                _context.BoothMaster.Update(existingbooth);
                                                await _context.SaveChangesAsync();
                                                return new Response { Status = RequestStatusEnum.OK, Message = "Booth Updated successfully except BoothName/Number/District/Assembly/Status/Aux. Kindly Release Booth in order to update all fields" };

                                            }
                                        }
                                        else
                                        {
                                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Election Info Record found aganist this Booth, thus can't change status" };

                                        }


                                    }
                                    else
                                    {
                                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Problem while Updating Booth" };

                                    }
                                    //}

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
            foreach (var boothMaster in boothMasters)
            {
                var existingBooth = await _context.BoothMaster.FirstOrDefaultAsync(b =>
                    b.StateMasterId == boothMaster.StateMasterId &&
                    b.DistrictMasterId == boothMaster.DistrictMasterId &&
                    b.AssemblyMasterId == boothMaster.AssemblyMasterId &&
                    b.BoothMasterId == boothMaster.BoothMasterId &&
                    b.ElectionTypeMasterId == boothMaster.ElectionTypeMasterId && b.BoothMasterId == boothMaster.BoothMasterId);

                if (existingBooth == null)
                {
                    return new Response { Status = RequestStatusEnum.NotFound, Message = "Booth Not Found" };
                }

                if (!existingBooth.BoothStatus)
                {
                    return new Response { Status = RequestStatusEnum.NotFound, Message = "Booth is Not Active" };
                }

                // Check for Field Officer asynchronously
                var foExists = await _context.FieldOfficerMaster
                    .AnyAsync(p => p.FieldOfficerMasterId == Convert.ToInt32(boothMaster.AssignedTo));

                if (!foExists)
                {
                    return new Response { Status = RequestStatusEnum.NotFound, Message = "Field Officer Not Found" };
                }

                // Update booth assignment details
                existingBooth.AssignedBy = boothMaster.AssignedBy;
                existingBooth.AssignedTo = boothMaster.AssignedTo;
                existingBooth.AssignedOnTime = DateTime.UtcNow;
                existingBooth.IsAssigned = boothMaster.IsAssigned;

                _context.BoothMaster.Update(existingBooth);
            }

            await _context.SaveChangesAsync();

            return new Response { Status = RequestStatusEnum.OK, Message = "Booths successfully mapped" };
        }

        public async Task<Response> ReleaseBooth(BoothMaster boothMaster)
        {
            if (boothMaster.BoothMasterId != null || boothMaster.IsAssigned == false)
            {

                var electionInfoRecord = await _context.ElectionInfoMaster.FirstOrDefaultAsync(e => e.BoothMasterId == boothMaster.BoothMasterId);
                if (electionInfoRecord == null)

                {
                    var existingbooth = await _context.BoothMaster.FirstOrDefaultAsync(so => so.BoothMasterId == boothMaster.BoothMasterId && so.StateMasterId == boothMaster.StateMasterId
                                                && so.DistrictMasterId == so.DistrictMasterId && so.AssemblyMasterId == boothMaster.AssemblyMasterId);
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
                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Record not found!" };

            }
        }
        public async Task<BoothMaster> GetBoothById(string boothMasterId)
        {
            var boothRecord = await _context.BoothMaster.Include(d => d.StateMaster).Include(d => d.DistrictMaster).Include(d => d.AssemblyMaster).Include(d => d.FourthLevelH).Include(d => d.ElectionTypeMaster).Where(d => d.BoothMasterId == Convert.ToInt32(boothMasterId)).FirstOrDefaultAsync();

            return boothRecord;
        }
        public async Task<BoothDetailForVoterInQueue> GetBoothDetailForVoterInQueue(int boothMasterId)
        {
            // Fetch FinalVote from ElectionInfoMaster
            var electionInfoMaster = await _context.ElectionInfoMaster
                .Where(e => e.BoothMasterId == boothMasterId)
                .Select(e => new
                {
                    e.FinalVote,
                    e.VotingLastUpdate,
                    e.IsVoterTurnOut
                })
                .FirstOrDefaultAsync();

            // Fetch TotalVoters from BoothMaster
            var boothRecord = await _context.BoothMaster
                .Where(d => d.BoothMasterId == boothMasterId)
                .Select(b => new
                {
                    TotalVoters = b.TotalVoters
                })
                .FirstOrDefaultAsync();

            // Create and populate the result object
            BoothDetailForVoterInQueue boothDetailForVoterInQueue = new BoothDetailForVoterInQueue()
            {
                BoothMasterId = boothMasterId,
                TotalVoters = boothRecord.TotalVoters,
                RemainingVoters = (boothRecord.TotalVoters) - (electionInfoMaster.FinalVote),
                VotesPolled = electionInfoMaster.FinalVote,
                VotesPolledTime = electionInfoMaster.VotingLastUpdate,
                IsVoteEnabled = false,
                Message = "Voter Queue is not Available"

            };
            if (electionInfoMaster.IsVoterTurnOut == true)
            {
                boothDetailForVoterInQueue.IsVoteEnabled = true;
                boothDetailForVoterInQueue.Message = "Voter Queue is Available";
            }
            return boothDetailForVoterInQueue;
        }
        #endregion

        #region Event Master
        public async Task<ServiceResponse> AddEvent(EventMaster eventMaster)
        {
            // Check if the event already exists
            var isExist = await _context.EventMaster.AnyAsync(d =>
                d.StateMasterId == eventMaster.StateMasterId
                && d.ElectionTypeMasterId == eventMaster.ElectionTypeMasterId
                && d.EventSequence == eventMaster.EventSequence
                && d.EventABBR.Contains(eventMaster.EventABBR));

            // Return a failure response if the event already exists
            if (isExist)
            {
                return new ServiceResponse
                {
                    IsSucceed = false,
                    Message = "Event already exists."
                };
            }

            // Add the new event to the context and save changes
            _context.EventMaster.Add(eventMaster);
            await _context.SaveChangesAsync();

            // Return success response
            return new ServiceResponse
            {
                IsSucceed = true,
                Message = "Successfully added."
            };
        }


        public async Task<ServiceResponse> UpdateEvent(EventMaster eventMaster)
        {
            // Check if any other event has the same EventABBR and EventSequence
            var duplicateEvent = await _context.EventMaster
                .AnyAsync(d => d.StateMasterId == eventMaster.StateMasterId
                              && d.ElectionTypeMasterId == eventMaster.ElectionTypeMasterId
                              && d.EventABBR == eventMaster.EventABBR
                               && d.EventSequence == eventMaster.EventSequence);

            // If a duplicate is found, return a failure response
            if (duplicateEvent)
            {
                return new ServiceResponse
                {
                    IsSucceed = false,
                    Message = "An event with the same abbreviation and sequence already exists."
                };
            }

            // Check if the event to be updated exists
            var existingEvent = await _context.EventMaster.FirstOrDefaultAsync(d => d.EventMasterId == eventMaster.EventMasterId);

            // If the event does not exist, return a failure response
            if (existingEvent == null)
            {
                return new ServiceResponse
                {
                    IsSucceed = false,
                    Message = "Event not found."
                };
            }

            existingEvent.EventName = eventMaster.EventName;
            existingEvent.StateMasterId = eventMaster.StateMasterId;
            existingEvent.EventSequence = eventMaster.EventSequence;
            existingEvent.EventABBR = eventMaster.EventABBR;
            existingEvent.ElectionTypeMasterId = eventMaster.ElectionTypeMasterId;
            _context.EventMaster.Update(existingEvent);
            await _context.SaveChangesAsync();

            // Return a success response
            return new ServiceResponse
            {
                IsSucceed = true,
                Message = "Event updated successfully."
            };
        }

        public async Task<List<EventMaster>> GetEventListById(int stateMasterId, int electionTypeMasterId)
        {
            return await _context.EventMaster.Where(d => d.StateMasterId == stateMasterId
            && d.ElectionTypeMasterId == electionTypeMasterId).OrderBy(d => d.EventSequence)
            .ToListAsync();


        }
        private async Task<EventMaster> GetFirstSequenceEventById(int stateMasterId, int electionTypeMasterId)
        {
            return await _context.EventMaster.Where(d => d.StateMasterId == stateMasterId
                                                         && d.ElectionTypeMasterId == electionTypeMasterId).OrderBy(d => d.EventSequence)
                .FirstOrDefaultAsync();


        }
        public async Task<List<EventMaster>> GetEventListForBooth(int stateMasterId, int electionTypeMasterId)
        {
            return await _context.EventMaster.Where(d => d.StateMasterId == stateMasterId
            && d.ElectionTypeMasterId == electionTypeMasterId && d.Status == true).OrderBy(d => d.EventSequence)
            .ToListAsync();


        }
        public async Task<List<EventAbbr>> GetEventAbbrList()
        {
            return await _context.EventAbbr.ToListAsync();

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
        public async Task<EventMaster> GetEventById(int eventMasterId)
        {
            return await _context.EventMaster
                .Include(e => e.StateMaster)
                .Include(e => e.ElectionTypeMaster)
                .FirstOrDefaultAsync(d => d.EventMasterId == eventMasterId);
        }
        public async Task<ServiceResponse> DeleteEventById(int eventMasterId)
        {
            // Check if the event exists
            var eventToDelete = await _context.EventMaster.FirstOrDefaultAsync(d => d.EventMasterId == eventMasterId);

            // If the event does not exist, return a failure response
            if (eventToDelete == null)
            {
                return new ServiceResponse
                {
                    IsSucceed = false,
                    Message = $"Event with ID {eventMasterId} not found."
                };
            }

            _context.EventMaster.Remove(eventToDelete);
            await _context.SaveChangesAsync();

            // Return a success response
            return new ServiceResponse
            {
                IsSucceed = true,
                Message = "Event deleted successfully."
            };
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
                            UpdateStatus = electioInfoRecord.IsPartyDispatched
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
                            UpdateStatus = electioInfoRecord.IsPartyReached
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
                            UpdateStatus = electioInfoRecord.IsSetupOfPolling
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
                            UpdateStatus = electioInfoRecord.IsMockPollDone
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
                            UpdateStatus = electioInfoRecord.IsPollStarted
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
                            UpdateStatus = electioInfoRecord.IsFinalVote
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
                            UpdateStatus = electioInfoRecord.IsPollEnded
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
                            UpdateStatus = electioInfoRecord.IsMCESwitchOff
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
                            UpdateStatus = electioInfoRecord.IsPartyDeparted
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
                            UpdateStatus = electioInfoRecord.IsPartyReachedCollectionCenter
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
                            UpdateStatus = electioInfoRecord.IsEVMDeposited
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
                    return electioInfoRecord.IsPartyDispatched;
                case "2":
                    return electioInfoRecord.IsPartyReached;
                case "3":
                    return electioInfoRecord.IsSetupOfPolling;
                case "4":
                    return electioInfoRecord.IsMockPollDone;
                case "5":
                    return electioInfoRecord.IsPollStarted;
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
                    return electioInfoRecord.IsFinalVote;

                case "9":
                    return electioInfoRecord.IsPollEnded;
                case "10":
                    return electioInfoRecord.IsMCESwitchOff;
                case "11":
                    return electioInfoRecord.IsPartyDeparted;
                case "12":
                    return electioInfoRecord.IsPartyReachedCollectionCenter;
                case "13":
                    return electioInfoRecord.IsEVMDeposited;
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
                    if (electioInfoRecord.VoterInQueue == null && electioInfoRecord.IsVoterInQueue != true)
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
        public async Task<List<BoothEvents>> GetBoothEventListById(int stateMasterId, int electionTypeMasterId, int boothMasterId)
        {
            // Step 1: Get the list of events for the given state and election type
            var getBoothEvents = await GetEventListForBooth(stateMasterId, electionTypeMasterId);

            // Step 2: Get the corresponding election info for the given booth
            var getElectionInfoRecord = await _context.ElectionInfoMaster.FirstOrDefaultAsync(d => d.StateMasterId == stateMasterId
                && d.ElectionTypeMasterId == electionTypeMasterId
                && d.BoothMasterId == boothMasterId);

            // Step 3: Prepare the final list of BoothEvents with the appropriate statuses
            var boothEventsList = new List<BoothEvents>();
            if (getElectionInfoRecord == null)
            {
                foreach (var eventItem in getBoothEvents)
                {
                    boothEventsList.Add(new BoothEvents
                    {
                        BoothMasterId = boothMasterId,
                        EventMasterId = eventItem.EventMasterId,
                        EventName = eventItem.EventName,
                        EventSequence = eventItem.EventSequence,
                        EventABBR = eventItem.EventABBR,
                        EventStatus = false  // Set all statuses to false if getElectionInfoRecord is null
                    });
                }
            }
            else
            {
                foreach (var eventItem in getBoothEvents)
                {
                    bool eventStatus = false;  // Default status is false

                    // Map the event status based on getElectionInfoRecord only for events from getBoothEvents
                    switch (eventItem.EventABBR)
                    {
                        case "PD":
                            eventStatus = getElectionInfoRecord.IsPartyDispatched;
                            break;

                        case "PA": // Party Arrived
                            eventStatus = getElectionInfoRecord.IsPartyReached;
                            break;
                        case "SP": // Setup Polling Station
                            eventStatus = getElectionInfoRecord.IsSetupOfPolling;
                            break;
                        case "MP": // Mock Poll Done
                            eventStatus = getElectionInfoRecord.IsMockPollDone;
                            break;
                        case "PS": // Poll Started
                            eventStatus = getElectionInfoRecord.IsPollStarted;
                            break;
                        case "VT": // Voter Turn Out
                            eventStatus = getElectionInfoRecord.IsVoterTurnOut;
                            break;
                        case "VQ": // Voter In Queue
                            eventStatus = getElectionInfoRecord.IsVoterInQueue;
                            break;
                        case "FV": // Final Votes Polled
                            eventStatus = getElectionInfoRecord.IsFinalVote;
                            break;
                        case "PE": // Poll Ended
                            eventStatus = getElectionInfoRecord.IsPollEnded;
                            break;
                        case "EO": // EVMVVPATOff
                            eventStatus = getElectionInfoRecord.IsMCESwitchOff;
                            break;

                        case "PC": // PartyDeparted	
                            eventStatus = getElectionInfoRecord.IsPartyDeparted;
                            break;

                        case "PR": // PartyReachedAtCollection
                            eventStatus = getElectionInfoRecord.IsPartyReachedCollectionCenter;
                            break;

                        case "ED": // EVMDeposited
                            eventStatus = getElectionInfoRecord.IsEVMDeposited;
                            break;

                        default:
                            eventStatus = false;  // If no matching status is found, set it to false
                            break;
                    }

                    // Step 4: Add only those events to the list that are from getBoothEvents
                    boothEventsList.Add(new BoothEvents
                    {
                        BoothMasterId = boothMasterId,
                        EventMasterId = eventItem.EventMasterId,
                        EventName = eventItem.EventName,
                        EventSequence = eventItem.EventSequence,
                        EventABBR = eventItem.EventABBR,
                        EventStatus = eventStatus  // Set the status according to the ElectionInfoMaster record
                    });
                }
            }

            return boothEventsList;  // Return the filtered and mapped list of BoothEvents
        }

        public async Task<CheckEventActivity> GetNextEvent(UpdateEventActivity updateEventActivity)
        {
            // Try to retrieve the event list from cache
            var eventList = await _cacheService.GetDataAsync<List<EventMaster>>("GetNextEventList");

            // If cache is empty, retrieve from repository and set it in cache
            if (eventList == null || !eventList.Any())
            {
                eventList = await GetEventListById(updateEventActivity.StateMasterId, updateEventActivity.ElectionTypeMasterId);
                await _cacheService.SetDataAsync("GetNextEventList", eventList, DateTimeOffset.Now.AddMinutes(10)); // Cache for 5 minutes
            }

            // Sort the event list by sequence in ascending order
            var sortedEventList = eventList.OrderBy(e => e.EventSequence).ToList();

            // Find the current event based on EventABBR and EventSequence
            var currentEvent = sortedEventList.FirstOrDefault(e => e.EventABBR == updateEventActivity.EventABBR && e.EventSequence == updateEventActivity.EventSequence);

            if (currentEvent == null)
            {
                // If the current event is not found, handle the error (return null or throw exception)
                return null;
            }

            // Find the index of the current event
            int currentIndex = sortedEventList.IndexOf(currentEvent);

            // Check if there is a next event after the current one
            var nextEvent = currentIndex >= 0 && currentIndex < sortedEventList.Count - 1
                ? sortedEventList[currentIndex + 1]
                : null;

            CheckEventActivity checkEventActivity = new CheckEventActivity();

            if (nextEvent != null)
            {
                // Set up the CheckEventActivity with the next event details
                checkEventActivity.StateMasterId = updateEventActivity.StateMasterId;
                checkEventActivity.DistrictMasterId = updateEventActivity.DistrictMasterId;
                checkEventActivity.AssemblyMasterId = updateEventActivity.AssemblyMasterId;
                checkEventActivity.BoothMasterId = updateEventActivity.BoothMasterId;
                checkEventActivity.ElectionTypeMasterId = updateEventActivity.ElectionTypeMasterId;
                checkEventActivity.EventMasterId = nextEvent.EventMasterId;
                checkEventActivity.EventABBR = nextEvent.EventABBR;
                checkEventActivity.EventName = nextEvent.EventName;
                checkEventActivity.EventSequence = nextEvent.EventSequence;
                checkEventActivity.EventStatus = nextEvent.Status;
            }
            else
            {
                // No next event, handle accordingly
                checkEventActivity = null; // or set it up with some default values
            }

            // Return the next event (or null if none found)
            return checkEventActivity;
        }
        public async Task<CheckEventActivity> GetPreviousEvent(UpdateEventActivity updateEventActivity)
        {
            // Try to retrieve the event list from cache
            var eventList = await _cacheService.GetDataAsync<List<EventMaster>>("GetEventList");

            // If cache is empty, retrieve from repository and set it in cache
            if (eventList == null || !eventList.Any())
            {
                eventList = await GetEventListById(updateEventActivity.StateMasterId, updateEventActivity.ElectionTypeMasterId);
                await _cacheService.SetDataAsync("GetEventList", eventList, DateTimeOffset.Now.AddMinutes(5)); // Cache for 5 minutes
            }

            // Sort the event list by sequence in ascending order
            var sortedEventList = eventList.OrderBy(e => e.EventSequence).ToList();

            // Find the current event based on EventABBR and EventSequence
            var currentEvent = sortedEventList.FirstOrDefault(e => e.EventABBR == updateEventActivity.EventABBR && e.EventSequence == updateEventActivity.EventSequence);

            if (currentEvent == null)
            {
                // If the current event is not found, handle the error (return null or throw exception)
                return null;
            }

            // Check previous events for status
            var previousEvent = sortedEventList.Take(sortedEventList.IndexOf(currentEvent))
                                               .LastOrDefault(e => e.Status == true);
            if (previousEvent == null)
            {
                // If the current event is not found, handle the error (return null or throw exception)
                return null;
            }

            CheckEventActivity checkEventActivity = new CheckEventActivity();
            checkEventActivity.StateMasterId = updateEventActivity.StateMasterId;
            checkEventActivity.DistrictMasterId = updateEventActivity.DistrictMasterId;
            checkEventActivity.AssemblyMasterId = updateEventActivity.AssemblyMasterId;
            checkEventActivity.BoothMasterId = updateEventActivity.BoothMasterId;
            checkEventActivity.ElectionTypeMasterId = updateEventActivity.ElectionTypeMasterId;
            checkEventActivity.EventMasterId = previousEvent.EventMasterId;
            checkEventActivity.EventABBR = previousEvent.EventABBR;
            checkEventActivity.EventName = previousEvent.EventName;
            checkEventActivity.EventSequence = previousEvent.EventSequence;
            checkEventActivity.EventStatus = previousEvent.Status;
            // Return the first previous event with Status = true (or null if none found)
            return checkEventActivity;
        }

        public async Task<ElectionInfoMaster> EventUpdationStatus(ElectionInfoMaster electionInfoMaster)
        {
            // added electionTypeMasterId check
            var electionInfoRecord = _context.ElectionInfoMaster.Where(d => d.StateMasterId == electionInfoMaster.StateMasterId
            && d.DistrictMasterId == electionInfoMaster.DistrictMasterId &&
            d.AssemblyMasterId == electionInfoMaster.AssemblyMasterId
            && d.BoothMasterId == electionInfoMaster.BoothMasterId && d.ElectionTypeMasterId == electionInfoMaster.ElectionTypeMasterId
            ).FirstOrDefault();
            return electionInfoRecord;
        }
        public async Task<ServiceResponse> PartyDispatch(UpdateEventActivity updateEventActivity)
        {
            // Fetch the record from the ElectionInfoMaster table that matches the UpdateEventActivity fields
            var result = await _context.ElectionInfoMaster.FirstOrDefaultAsync(d =>
                d.StateMasterId == updateEventActivity.StateMasterId &&
                d.DistrictMasterId == updateEventActivity.DistrictMasterId &&
                d.AssemblyMasterId == updateEventActivity.AssemblyMasterId &&
                d.ElectionTypeMasterId == updateEventActivity.ElectionTypeMasterId &&
                d.BoothMasterId == updateEventActivity.BoothMasterId
            );

            // If the record exists, update it
            if (result is not null)
            {
                result.EventMasterId = updateEventActivity.EventMasterId;
                result.EventSequence = updateEventActivity.EventSequence;
                result.EventABBR = updateEventActivity.EventABBR;
                result.EventName = updateEventActivity.EventName;
                result.ElectionInfoStatus = updateEventActivity.EventStatus;
                result.IsPartyDispatched = updateEventActivity.EventStatus;
                result.PartyDispatchedLastUpdate = BharatDateTime();
                result.EventStatus = updateEventActivity.EventStatus;
                _context.ElectionInfoMaster.Update(result);
            }
            // If the record does not exist, create a new one
            else
            {
                var newElectionInfo = new ElectionInfoMaster
                {
                    StateMasterId = updateEventActivity.StateMasterId,
                    DistrictMasterId = updateEventActivity.DistrictMasterId,
                    AssemblyMasterId = updateEventActivity.AssemblyMasterId,
                    ElectionTypeMasterId = updateEventActivity.ElectionTypeMasterId,
                    BoothMasterId = updateEventActivity.BoothMasterId,
                    EventMasterId = updateEventActivity.EventMasterId,
                    EventSequence = updateEventActivity.EventSequence,
                    EventABBR = updateEventActivity.EventABBR,
                    EventName = updateEventActivity.EventName,
                    ElectionInfoStatus = updateEventActivity.EventStatus,
                    IsPartyDispatched = updateEventActivity.EventStatus,
                    EventStatus = updateEventActivity.EventStatus,
                    PartyDispatchedLastUpdate = BharatDateTime()
                };

                // Add the new record to the database
                await _context.ElectionInfoMaster.AddAsync(newElectionInfo);
            }

            await _context.SaveChangesAsync();


            return new ServiceResponse
            {
                IsSucceed = true
                ,
                Message = "Party Dispatch Updated SucessFully"
            };
        }
        public async Task<ServiceResponse> PartyArrived(UpdateEventActivity updateEventActivity)
        {
            // Fetch the record from the ElectionInfoMaster table that matches the UpdateEventActivity fields
            var result = await _context.ElectionInfoMaster.FirstOrDefaultAsync(d =>
                d.StateMasterId == updateEventActivity.StateMasterId &&
                d.DistrictMasterId == updateEventActivity.DistrictMasterId &&
                d.AssemblyMasterId == updateEventActivity.AssemblyMasterId &&
                d.ElectionTypeMasterId == updateEventActivity.ElectionTypeMasterId &&
                d.BoothMasterId == updateEventActivity.BoothMasterId
            );

            // If the record exists, update it
            if (result is not null)
            {
                result.EventMasterId = updateEventActivity.EventMasterId;
                result.EventSequence = updateEventActivity.EventSequence;
                result.EventABBR = updateEventActivity.EventABBR;
                result.EventName = updateEventActivity.EventName;
                result.ElectionInfoStatus = updateEventActivity.EventStatus;
                result.IsPartyReached = updateEventActivity.EventStatus;
                result.PartyReachedLastUpdate = BharatDateTime();
                result.EventStatus = updateEventActivity.EventStatus;
                _context.ElectionInfoMaster.Update(result);
            }

            await _context.SaveChangesAsync();


            return new ServiceResponse
            {
                IsSucceed = true
                ,
                Message = "Party Arrived Updated SucessFully"
            };
        }
        public async Task<ServiceResponse> SetupPollingStation(UpdateEventActivity updateEventActivity)
        {
            // Fetch the record from the ElectionInfoMaster table that matches the UpdateEventActivity fields
            var result = await _context.ElectionInfoMaster.FirstOrDefaultAsync(d =>
                d.StateMasterId == updateEventActivity.StateMasterId &&
                d.DistrictMasterId == updateEventActivity.DistrictMasterId &&
                d.AssemblyMasterId == updateEventActivity.AssemblyMasterId &&
                d.ElectionTypeMasterId == updateEventActivity.ElectionTypeMasterId &&
                d.BoothMasterId == updateEventActivity.BoothMasterId
            );

            // If the record exists, update it
            if (result is not null)
            {
                result.EventMasterId = updateEventActivity.EventMasterId;
                result.EventSequence = updateEventActivity.EventSequence;
                result.EventABBR = updateEventActivity.EventABBR;
                result.ElectionInfoStatus = updateEventActivity.EventStatus;
                result.IsSetupOfPolling = updateEventActivity.EventStatus;
                result.SetupOfPollingLastUpdate = BharatDateTime();
                result.EventName = updateEventActivity.EventName;
                result.EventStatus = updateEventActivity.EventStatus;
                _context.ElectionInfoMaster.Update(result);
            }


            await _context.SaveChangesAsync();


            return new ServiceResponse
            {
                IsSucceed = true
                ,
                Message = "Event Updated SucessFully"
            };
        }
        public async Task<ServiceResponse> MockPollDone(UpdateEventActivity updateEventActivity)
        {
            // Fetch the record from the ElectionInfoMaster table that matches the UpdateEventActivity fields
            var result = await _context.ElectionInfoMaster.FirstOrDefaultAsync(d =>
                d.StateMasterId == updateEventActivity.StateMasterId &&
                d.DistrictMasterId == updateEventActivity.DistrictMasterId &&
                d.AssemblyMasterId == updateEventActivity.AssemblyMasterId &&
                d.ElectionTypeMasterId == updateEventActivity.ElectionTypeMasterId &&
                d.BoothMasterId == updateEventActivity.BoothMasterId
            );

            // If the record exists, update it
            if (result is not null)
            {
                result.EventMasterId = updateEventActivity.EventMasterId;
                result.EventSequence = updateEventActivity.EventSequence;
                result.EventABBR = updateEventActivity.EventABBR;
                result.ElectionInfoStatus = updateEventActivity.EventStatus;
                result.NoOfPollingAgents = updateEventActivity.NoOfPollingAgents;
                result.IsMockPollDone = updateEventActivity.EventStatus;
                result.MockPollDoneLastUpdate = BharatDateTime();
                result.EventName = updateEventActivity.EventName;
                result.EventStatus = updateEventActivity.EventStatus;
                _context.ElectionInfoMaster.Update(result);
            }


            await _context.SaveChangesAsync();


            return new ServiceResponse
            {
                IsSucceed = true
                ,
                Message = "Event Updated SucessFully"
            };
        }
        public async Task<ServiceResponse> PollStarted(UpdateEventActivity updateEventActivity)
        {
            // Fetch the record from the ElectionInfoMaster table that matches the UpdateEventActivity fields
            var result = await _context.ElectionInfoMaster.FirstOrDefaultAsync(d =>
                d.StateMasterId == updateEventActivity.StateMasterId &&
                d.DistrictMasterId == updateEventActivity.DistrictMasterId &&
                d.AssemblyMasterId == updateEventActivity.AssemblyMasterId &&
                d.ElectionTypeMasterId == updateEventActivity.ElectionTypeMasterId &&
                d.BoothMasterId == updateEventActivity.BoothMasterId
            );

            // If the record exists, update it
            if (result is not null)
            {
                result.EventMasterId = updateEventActivity.EventMasterId;
                result.EventSequence = updateEventActivity.EventSequence;
                result.EventABBR = updateEventActivity.EventABBR;
                result.ElectionInfoStatus = updateEventActivity.EventStatus;
                result.IsPollStarted = updateEventActivity.EventStatus;
                result.PollStartedLastUpdate = BharatDateTime();
                result.EventName = updateEventActivity.EventName;
                result.EventStatus = updateEventActivity.EventStatus;
                _context.ElectionInfoMaster.Update(result);
            }


            await _context.SaveChangesAsync();


            return new ServiceResponse
            {
                IsSucceed = true
                ,
                Message = "Event Updated SucessFully"
            };
        }

        public async Task<ServiceResponse> VoterTurnOut(UpdateEventActivity updateEventActivity)
        {
            // Get the latest slot from the SlotManagementMaster table
            var getLatestSlot = await GetVoterSlotAvailable(updateEventActivity.StateMasterId, updateEventActivity.ElectionTypeMasterId);
            var getLastSlot = await GetLastSlot(updateEventActivity.StateMasterId, updateEventActivity.EventMasterId, updateEventActivity.ElectionTypeMasterId);
            //var currentTime = DateTimeOffset.Now;

            //// Check if current time falls between EndTime and LockTime, if both are available
            //bool isWithinTimeWindow = getLatestSlot.EndTime.HasValue && getLatestSlot.LockTime.HasValue &&
            //                          currentTime.TimeOfDay >= getLatestSlot.EndTime.Value.ToTimeSpan() &&
            //                          currentTime.TimeOfDay <= getLatestSlot.LockTime.Value.ToTimeSpan();
            if (getLatestSlot is null)
            {
                return new ServiceResponse
                {
                    IsSucceed = false,
                    Message = "Current time is not within the allowed voting window"
                };
            }
            var totalVoters = await _context.BoothMaster
      .Where(d => d.StateMasterId == updateEventActivity.StateMasterId &&
                  d.DistrictMasterId == updateEventActivity.DistrictMasterId &&
                  d.AssemblyMasterId == updateEventActivity.AssemblyMasterId &&
                  d.ElectionTypeMasterId == updateEventActivity.ElectionTypeMasterId &&
                  d.BoothMasterId == updateEventActivity.BoothMasterId)
      .Select(d => d.TotalVoters)
      .FirstOrDefaultAsync();

            if (updateEventActivity.VotesPolled > totalVoters)
            {
                return new ServiceResponse
                {
                    IsSucceed = false,
                    Message = "The number of votes polled exceeds the total registered voters. Please verify the input."
                };
            }

            // Fetch the record from the ElectionInfoMaster table that matches the UpdateEventActivity fields


            bool pollDetail = await _context.PollDetails.AnyAsync(d =>
                d.StateMasterId == updateEventActivity.StateMasterId &&
                d.DistrictMasterId == updateEventActivity.DistrictMasterId &&
                d.AssemblyMasterId == updateEventActivity.AssemblyMasterId &&
                d.ElectionTypeMasterId == updateEventActivity.ElectionTypeMasterId &&
                d.BoothMasterId == updateEventActivity.BoothMasterId
                && d.SlotManagementId == getLatestSlot.SlotManagementId
            );


            if (pollDetail)
            {

                return new ServiceResponse
                {
                    IsSucceed = false,
                    Message = "Already Entered for this Slot"
                };
            }

            var result = await _context.ElectionInfoMaster.FirstOrDefaultAsync(d =>
             d.StateMasterId == updateEventActivity.StateMasterId &&
             d.DistrictMasterId == updateEventActivity.DistrictMasterId &&
             d.AssemblyMasterId == updateEventActivity.AssemblyMasterId &&
             d.ElectionTypeMasterId == updateEventActivity.ElectionTypeMasterId &&
             d.BoothMasterId == updateEventActivity.BoothMasterId
         );
            if (result is not null)
            {
                // Update the existing ElectionInfoMaster record
                result.EventMasterId = updateEventActivity.EventMasterId;
                result.EventSequence = updateEventActivity.EventSequence;
                result.EventABBR = updateEventActivity.EventABBR;
                result.ElectionInfoStatus = updateEventActivity.EventStatus;
                result.EventStatus = false;
                if (getLatestSlot != null && getLatestSlot.IsLastSlot == true)
                {
                    result.IsVoterTurnOut = true;
                    result.EventStatus = true;

                }
                result.VotingTurnOutLastUpdate = BharatDateTime();
                result.VotingLastUpdate = BharatDateTime();
                result.FinalVote = updateEventActivity.VotesPolled;
                result.EventName = updateEventActivity.EventName;
                // Check if a PollDetail already exists within the current Slot's EndTime and LockTime





                PollDetail newPollDetail = new PollDetail()
                {
                    StateMasterId = updateEventActivity.StateMasterId,
                    DistrictMasterId = updateEventActivity.DistrictMasterId,
                    AssemblyMasterId = updateEventActivity.AssemblyMasterId,
                    BoothMasterId = updateEventActivity.BoothMasterId,
                    ElectionTypeMasterId = updateEventActivity.ElectionTypeMasterId,
                    EventMasterId = updateEventActivity.EventMasterId,
                    EventSequence = updateEventActivity.EventSequence,
                    EventABBR = updateEventActivity.EventABBR,
                    VotesPolledRecivedTime = BharatDateTime(),
                    VotesPolled = updateEventActivity.VotesPolled,
                    EventName = updateEventActivity.EventName,
                    SlotManagementId = getLatestSlot.SlotManagementId,
                };

                _context.PollDetails.Add(newPollDetail);


                // Update ElectionInfoMaster in the context
                _context.ElectionInfoMaster.Update(result);
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return success response
            return new ServiceResponse
            {
                IsSucceed = true,
                Message = "Event Updated Successfully"
            };
        }

        public async Task<ServiceResponse> VoterInQueue(UpdateEventActivity updateEventActivity)
        {
            // Fetch the record from the ElectionInfoMaster table that matches the UpdateEventActivity fields
            var result = await _context.ElectionInfoMaster.FirstOrDefaultAsync(d =>
                d.StateMasterId == updateEventActivity.StateMasterId &&
                d.DistrictMasterId == updateEventActivity.DistrictMasterId &&
                d.AssemblyMasterId == updateEventActivity.AssemblyMasterId &&
                d.ElectionTypeMasterId == updateEventActivity.ElectionTypeMasterId &&
                d.BoothMasterId == updateEventActivity.BoothMasterId
            );

            // If the record exists, update it
            if (result is not null)
            {
                result.EventMasterId = updateEventActivity.EventMasterId;
                result.EventSequence = updateEventActivity.EventSequence;
                result.EventABBR = updateEventActivity.EventABBR;
                result.ElectionInfoStatus = updateEventActivity.EventStatus;
                result.IsVoterInQueue = updateEventActivity.EventStatus;
                result.VoterInQueueLastUpdate = BharatDateTime();
                result.EventName = updateEventActivity.EventName;
                result.EventStatus = updateEventActivity.EventStatus;
                _context.ElectionInfoMaster.Update(result);
            }


            await _context.SaveChangesAsync();


            return new ServiceResponse
            {
                IsSucceed = true
                ,
                Message = "Event Updated SucessFully"
            };
        }
        public async Task<ServiceResponse> FinalVotesPolled(UpdateEventActivity updateEventActivity)
        {
            var boothMaster = await _context.BoothMaster
       .Where(d => d.StateMasterId == updateEventActivity.StateMasterId &&
                   d.DistrictMasterId == updateEventActivity.DistrictMasterId &&
                   d.AssemblyMasterId == updateEventActivity.AssemblyMasterId &&
                   d.ElectionTypeMasterId == updateEventActivity.ElectionTypeMasterId &&
                   d.BoothMasterId == updateEventActivity.BoothMasterId)
       .Select(d => new
       {
           d.Male,
           d.Female,
           d.Transgender
       })
       .FirstOrDefaultAsync();

            string warningMessage = string.Empty;

            if (updateEventActivity.FinalMaleVotes > boothMaster.Male)
            {
                warningMessage += "The number of male votes exceeds the total registered male voters. ";
            }
            if (updateEventActivity.FinalFeMaleVotes > boothMaster.Female)
            {
                warningMessage += "The number of female votes exceeds the total registered female voters. ";
            }
            if (updateEventActivity.FinalTransgenderVotes > boothMaster.Transgender)
            {
                warningMessage += "The number of transgender votes exceeds the total registered transgender voters. ";
            }

            if (!string.IsNullOrEmpty(warningMessage))
            {
                return new ServiceResponse
                {
                    IsSucceed = false,
                    Message = warningMessage.Trim()
                };
            }

            // Fetch the record from the ElectionInfoMaster table that matches the UpdateEventActivity fields
            var result = await _context.ElectionInfoMaster.FirstOrDefaultAsync(d =>
                d.StateMasterId == updateEventActivity.StateMasterId &&
                d.DistrictMasterId == updateEventActivity.DistrictMasterId &&
                d.AssemblyMasterId == updateEventActivity.AssemblyMasterId &&
                d.ElectionTypeMasterId == updateEventActivity.ElectionTypeMasterId &&
                d.BoothMasterId == updateEventActivity.BoothMasterId
            );

            // If the record exists, update it
            if (result is not null)
            {
                result.EventMasterId = updateEventActivity.EventMasterId;
                result.EventSequence = updateEventActivity.EventSequence;
                result.EventABBR = updateEventActivity.EventABBR;
                result.ElectionInfoStatus = updateEventActivity.EventStatus;
                result.IsFinalVote = updateEventActivity.EventStatus;
                result.FinalVoteLastUpdate = BharatDateTime();
                result.EventName = updateEventActivity.EventName;
                result.EventStatus = updateEventActivity.EventStatus;
                _context.ElectionInfoMaster.Update(result);
            }


            await _context.SaveChangesAsync();


            return new ServiceResponse
            {
                IsSucceed = true
                ,
                Message = "Event Updated SucessFully"
            };
        }
        public async Task<ServiceResponse> PollEnded(UpdateEventActivity updateEventActivity)
        {
            // Fetch the record from the ElectionInfoMaster table that matches the UpdateEventActivity fields
            var result = await _context.ElectionInfoMaster.FirstOrDefaultAsync(d =>
                d.StateMasterId == updateEventActivity.StateMasterId &&
                d.DistrictMasterId == updateEventActivity.DistrictMasterId &&
                d.AssemblyMasterId == updateEventActivity.AssemblyMasterId &&
                d.ElectionTypeMasterId == updateEventActivity.ElectionTypeMasterId &&
                d.BoothMasterId == updateEventActivity.BoothMasterId
            );

            // If the record exists, update it
            if (result is not null)
            {
                result.EventMasterId = updateEventActivity.EventMasterId;
                result.EventSequence = updateEventActivity.EventSequence;
                result.EventABBR = updateEventActivity.EventABBR;
                result.ElectionInfoStatus = updateEventActivity.EventStatus;
                result.IsPollEnded = updateEventActivity.EventStatus;
                result.IsPollEndedLastUpdate = BharatDateTime();
                result.EventName = updateEventActivity.EventName;

                result.EventStatus = updateEventActivity.EventStatus;
                _context.ElectionInfoMaster.Update(result);
            }


            await _context.SaveChangesAsync();


            return new ServiceResponse
            {
                IsSucceed = true
                ,
                Message = "Event Updated SucessFully"
            };
        }
        public async Task<ServiceResponse> EVMVVPATOff(UpdateEventActivity updateEventActivity)
        {
            // Fetch the record from the ElectionInfoMaster table that matches the UpdateEventActivity fields
            var result = await _context.ElectionInfoMaster.FirstOrDefaultAsync(d =>
                d.StateMasterId == updateEventActivity.StateMasterId &&
                d.DistrictMasterId == updateEventActivity.DistrictMasterId &&
                d.AssemblyMasterId == updateEventActivity.AssemblyMasterId &&
                d.ElectionTypeMasterId == updateEventActivity.ElectionTypeMasterId &&
                d.BoothMasterId == updateEventActivity.BoothMasterId
            );

            // If the record exists, update it
            if (result is not null)
            {
                result.EventMasterId = updateEventActivity.EventMasterId;
                result.EventSequence = updateEventActivity.EventSequence;
                result.EventABBR = updateEventActivity.EventABBR;
                result.ElectionInfoStatus = updateEventActivity.EventStatus;
                result.IsMCESwitchOff = updateEventActivity.EventStatus;
                result.MCESwitchOffLastUpdate = BharatDateTime();
                result.EventName = updateEventActivity.EventName;
                result.EventStatus = updateEventActivity.EventStatus;
                _context.ElectionInfoMaster.Update(result);
            }


            await _context.SaveChangesAsync();


            return new ServiceResponse
            {
                IsSucceed = true
                ,
                Message = "Event Updated SucessFully"
            };
        }
        public async Task<ServiceResponse> PartyDeparted(UpdateEventActivity updateEventActivity)
        {
            // Fetch the record from the ElectionInfoMaster table that matches the UpdateEventActivity fields
            var result = await _context.ElectionInfoMaster.FirstOrDefaultAsync(d =>
                d.StateMasterId == updateEventActivity.StateMasterId &&
                d.DistrictMasterId == updateEventActivity.DistrictMasterId &&
                d.AssemblyMasterId == updateEventActivity.AssemblyMasterId &&
                d.ElectionTypeMasterId == updateEventActivity.ElectionTypeMasterId &&
                d.BoothMasterId == updateEventActivity.BoothMasterId
            );

            // If the record exists, update it
            if (result is not null)
            {
                result.EventMasterId = updateEventActivity.EventMasterId;
                result.EventSequence = updateEventActivity.EventSequence;
                result.EventABBR = updateEventActivity.EventABBR;
                result.ElectionInfoStatus = updateEventActivity.EventStatus;
                result.IsPartyDeparted = updateEventActivity.EventStatus;
                result.PartyDepartedLastUpdate = BharatDateTime();
                result.EventName = updateEventActivity.EventName;
                result.EventStatus = updateEventActivity.EventStatus;
                _context.ElectionInfoMaster.Update(result);
            }


            await _context.SaveChangesAsync();


            return new ServiceResponse
            {
                IsSucceed = true
                ,
                Message = "Event Updated SucessFully"
            };
        }
        public async Task<ServiceResponse> PartyReachedAtCollection(UpdateEventActivity updateEventActivity)
        {
            // Fetch the record from the ElectionInfoMaster table that matches the UpdateEventActivity fields
            var result = await _context.ElectionInfoMaster.FirstOrDefaultAsync(d =>
                d.StateMasterId == updateEventActivity.StateMasterId &&
                d.DistrictMasterId == updateEventActivity.DistrictMasterId &&
                d.AssemblyMasterId == updateEventActivity.AssemblyMasterId &&
                d.ElectionTypeMasterId == updateEventActivity.ElectionTypeMasterId &&
                d.BoothMasterId == updateEventActivity.BoothMasterId
            );

            // If the record exists, update it
            if (result is not null)
            {
                result.EventMasterId = updateEventActivity.EventMasterId;
                result.EventSequence = updateEventActivity.EventSequence;
                result.EventABBR = updateEventActivity.EventABBR;
                result.ElectionInfoStatus = updateEventActivity.EventStatus;
                result.IsPartyReachedCollectionCenter = updateEventActivity.EventStatus;
                result.PartyReachedCollectionCenterLastUpdate = BharatDateTime();
                result.EventName = updateEventActivity.EventName;
                result.EventStatus = updateEventActivity.EventStatus;
                _context.ElectionInfoMaster.Update(result);
            }


            await _context.SaveChangesAsync();


            return new ServiceResponse
            {
                IsSucceed = true
                ,
                Message = "Event Updated SucessFully"
            };
        }
        public async Task<ServiceResponse> EVMDeposited(UpdateEventActivity updateEventActivity)
        {
            // Fetch the record from the ElectionInfoMaster table that matches the UpdateEventActivity fields
            var result = await _context.ElectionInfoMaster.FirstOrDefaultAsync(d =>
                d.StateMasterId == updateEventActivity.StateMasterId &&
                d.DistrictMasterId == updateEventActivity.DistrictMasterId &&
                d.AssemblyMasterId == updateEventActivity.AssemblyMasterId &&
                d.ElectionTypeMasterId == updateEventActivity.ElectionTypeMasterId &&
                d.BoothMasterId == updateEventActivity.BoothMasterId
            );

            // If the record exists, update it
            if (result is not null)
            {
                result.EventMasterId = updateEventActivity.EventMasterId;
                result.EventSequence = updateEventActivity.EventSequence;
                result.EventABBR = updateEventActivity.EventABBR;
                result.ElectionInfoStatus = updateEventActivity.EventStatus;
                result.IsEVMDeposited = updateEventActivity.EventStatus;
                result.EVMDepositedLastUpdate = BharatDateTime();
                result.EventName = updateEventActivity.EventName;
                result.EventStatus = updateEventActivity.EventStatus;
                _context.ElectionInfoMaster.Update(result);
            }


            await _context.SaveChangesAsync();


            return new ServiceResponse
            {
                IsSucceed = true
                ,
                Message = "Event Updated SucessFully"
            };
        }

        #region IsCheckEvent

        public async Task<ServiceResponse> IsPartyDispatch(CheckEventActivity checkEventActivity)
        {
            // Fetch the record from the ElectionInfoMaster table that matches the UpdateEventActivity fields
            var result = await _context.ElectionInfoMaster.FirstOrDefaultAsync(d =>
                d.StateMasterId == checkEventActivity.StateMasterId &&
                d.DistrictMasterId == checkEventActivity.DistrictMasterId &&
                d.AssemblyMasterId == checkEventActivity.AssemblyMasterId &&
                d.ElectionTypeMasterId == checkEventActivity.ElectionTypeMasterId &&
                d.BoothMasterId == checkEventActivity.BoothMasterId
            );

            if (result is not null && result.IsPartyDispatched == true)
            {
                return new ServiceResponse()
                {
                    IsSucceed = true,
                };

            }
            else
            {
                return new ServiceResponse()
                {
                    IsSucceed = false,
                    Message = "Party Not Dispatch Yet"
                };

            }


        }
        public async Task<ServiceResponse> IsPartyArrived(CheckEventActivity checkEventActivity)
        {
            // Fetch the record from the ElectionInfoMaster table that matches the UpdateEventActivity fields
            var result = await _context.ElectionInfoMaster.FirstOrDefaultAsync(d =>
                d.StateMasterId == checkEventActivity.StateMasterId &&
                d.DistrictMasterId == checkEventActivity.DistrictMasterId &&
                d.AssemblyMasterId == checkEventActivity.AssemblyMasterId &&
                d.ElectionTypeMasterId == checkEventActivity.ElectionTypeMasterId &&
                d.BoothMasterId == checkEventActivity.BoothMasterId
            );

            if (result is not null && result.IsPartyReached == true)
            {
                return new ServiceResponse()
                {
                    IsSucceed = true,
                };

            }
            else
            {
                return new ServiceResponse()
                {
                    IsSucceed = false,
                    Message = "Party Not Arrived Yet"
                };

            }
        }
        public async Task<ServiceResponse> IsSetupPollingStation(CheckEventActivity checkEventActivity)
        {
            // Fetch the record from the ElectionInfoMaster table that matches the UpdateEventActivity fields
            var result = await _context.ElectionInfoMaster.FirstOrDefaultAsync(d =>
                d.StateMasterId == checkEventActivity.StateMasterId &&
                d.DistrictMasterId == checkEventActivity.DistrictMasterId &&
                d.AssemblyMasterId == checkEventActivity.AssemblyMasterId &&
                d.ElectionTypeMasterId == checkEventActivity.ElectionTypeMasterId &&
                d.BoothMasterId == checkEventActivity.BoothMasterId
            );

            if (result is not null && result.IsSetupOfPolling == true)
            {
                return new ServiceResponse()
                {
                    IsSucceed = true,
                };

            }
            else
            {
                return new ServiceResponse()
                {
                    IsSucceed = false,
                    Message = "Setup of Polling not  Yet"
                };

            }
        }

        public async Task<ServiceResponse> IsMockPollDone(CheckEventActivity checkEventActivity)
        {
            // Fetch the record from the ElectionInfoMaster table that matches the UpdateEventActivity fields
            var result = await _context.ElectionInfoMaster.FirstOrDefaultAsync(d =>
                d.StateMasterId == checkEventActivity.StateMasterId &&
                d.DistrictMasterId == checkEventActivity.DistrictMasterId &&
                d.AssemblyMasterId == checkEventActivity.AssemblyMasterId &&
                d.ElectionTypeMasterId == checkEventActivity.ElectionTypeMasterId &&
                d.BoothMasterId == checkEventActivity.BoothMasterId
            );

            if (result is not null && result.IsMockPollDone == true)
            {
                return new ServiceResponse()
                {
                    IsSucceed = true,
                };

            }
            else
            {
                return new ServiceResponse()
                {
                    IsSucceed = false,
                    Message = "Mock Poll Not Done Yet"
                };

            }
        }

        public async Task<ServiceResponse> IsPollStarted(CheckEventActivity checkEventActivity)
        {
            // Fetch the record from the ElectionInfoMaster table that matches the UpdateEventActivity fields
            var result = await _context.ElectionInfoMaster.FirstOrDefaultAsync(d =>
                d.StateMasterId == checkEventActivity.StateMasterId &&
                d.DistrictMasterId == checkEventActivity.DistrictMasterId &&
                d.AssemblyMasterId == checkEventActivity.AssemblyMasterId &&
                d.ElectionTypeMasterId == checkEventActivity.ElectionTypeMasterId &&
                d.BoothMasterId == checkEventActivity.BoothMasterId
            );

            if (result is not null && result.IsPollStarted == true)
            {
                return new ServiceResponse()
                {
                    IsSucceed = true,
                };

            }
            else
            {
                return new ServiceResponse()
                {
                    IsSucceed = false,
                    Message = "Poll Not Started Yet"
                };

            }
        }

        public async Task<ServiceResponse> IsVoterTurnOut(CheckEventActivity checkEventActivity)
        {
            // Fetch the record from the ElectionInfoMaster table that matches the UpdateEventActivity fields
            var result = await _context.ElectionInfoMaster.FirstOrDefaultAsync(d =>
                d.StateMasterId == checkEventActivity.StateMasterId &&
                d.DistrictMasterId == checkEventActivity.DistrictMasterId &&
                d.AssemblyMasterId == checkEventActivity.AssemblyMasterId &&
                d.ElectionTypeMasterId == checkEventActivity.ElectionTypeMasterId &&
                d.BoothMasterId == checkEventActivity.BoothMasterId
            );

            if (result is not null && result.IsVoterTurnOut == true)
            {
                return new ServiceResponse()
                {
                    IsSucceed = true,
                };

            }
            else
            {
                return new ServiceResponse()
                {
                    IsSucceed = false,
                    Message = "Voter TurnOut not Started Yet"
                };

            }
        }
        private async Task<ServiceResponse> IsVoterSlotAvailable(int stateMasterId, int electionTypeMasterId)
        {
            // Fetch slot list from cache
            var getSlotList = await _cacheService.GetDataAsync<List<SlotManagementMaster>>($"GetNextEventList{electionTypeMasterId}");

            // If not in cache, retrieve from the database and update cache
            if (getSlotList == null)
            {
                getSlotList = await _context.SlotManagementMaster
                    .Where(d => d.StateMasterId == stateMasterId &&
                                d.ElectionTypeMasterId == electionTypeMasterId)
                    .ToListAsync();

                await _cacheService.SetDataAsync($"GetNextEventList{electionTypeMasterId}", getSlotList, BharatTimeDynamic(0, 0, 0, 10, 0));
            }

            // Get the current time
            var currentTime = DateTimeOffset.Now;

            // Find the latest slot where the current time is between the EndTime and LockTime
            var availableSlot = getSlotList.FirstOrDefault(slot =>
                slot.StartDate == DateOnly.FromDateTime(currentTime.DateTime) && // Match the date
                slot.EndTime.HasValue &&
                slot.LockTime.HasValue &&
                currentTime.TimeOfDay > slot.EndTime.Value.ToTimeSpan() &&      // Current time is after EndTime
                currentTime.TimeOfDay < slot.LockTime.Value.ToTimeSpan());      // Current time is before LockTime

            // If a valid slot is found, return success
            if (availableSlot != null)
            {
                return new ServiceResponse()
                {
                    IsSucceed = true,
                    Message = $"Slot available. Slot ID: {availableSlot.SlotManagementId}, Time: {availableSlot.StartTime} - {availableSlot.EndTime}"
                };
            }

            // Default response if no slot is available
            return new ServiceResponse()
            {
                IsSucceed = false,
                Message = "No slot available at this time."
            };
        }
        private async Task<SlotManagementMaster> GetVoterSlotAvailable(int stateMasterId, int electionTypeMasterId)
        {

            var getSlotList = await _context.SlotManagementMaster
                 .Where(d => d.StateMasterId == stateMasterId &&
                             d.ElectionTypeMasterId == electionTypeMasterId)
                 .ToListAsync();


            // Get the current time
            var currentTime = DateTimeOffset.Now;

            // Find the latest slot where the current time is between the EndTime and LockTime
            var availableSlot = getSlotList.FirstOrDefault(slot =>
                slot.StartDate == DateOnly.FromDateTime(currentTime.DateTime) && // Match the date
                slot.EndTime.HasValue &&
                slot.LockTime.HasValue &&
                currentTime.TimeOfDay > slot.EndTime.Value.ToTimeSpan() &&      // Current time is after EndTime
                currentTime.TimeOfDay < slot.LockTime.Value.ToTimeSpan());      // Current time is before LockTime

            // If a valid slot is found, return success
            if (availableSlot != null)
            {
                return availableSlot;
            }
            else
            {
                return null;
            }
            // Default response if no slot is available

        }

        public async Task<ServiceResponse> IsVoterInQueue(CheckEventActivity checkEventActivity)
        {
            // Fetch the record from the ElectionInfoMaster table that matches the UpdateEventActivity fields
            var result = await _context.ElectionInfoMaster.FirstOrDefaultAsync(d =>
                d.StateMasterId == checkEventActivity.StateMasterId &&
                d.DistrictMasterId == checkEventActivity.DistrictMasterId &&
                d.AssemblyMasterId == checkEventActivity.AssemblyMasterId &&
                d.ElectionTypeMasterId == checkEventActivity.ElectionTypeMasterId &&
                d.BoothMasterId == checkEventActivity.BoothMasterId
            );

            if (result is not null && result.IsVoterInQueue == true)
            {
                return new ServiceResponse()
                {
                    IsSucceed = true,
                };

            }
            else
            {
                return new ServiceResponse()
                {
                    IsSucceed = false,
                    Message = "Voter Queue is not available"
                };

            }
        }

        public async Task<ServiceResponse> IsFinalVotesPolled(CheckEventActivity checkEventActivity)
        {
            // Fetch the record from the ElectionInfoMaster table that matches the UpdateEventActivity fields
            var result = await _context.ElectionInfoMaster.FirstOrDefaultAsync(d =>
                d.StateMasterId == checkEventActivity.StateMasterId &&
                d.DistrictMasterId == checkEventActivity.DistrictMasterId &&
                d.AssemblyMasterId == checkEventActivity.AssemblyMasterId &&
                d.ElectionTypeMasterId == checkEventActivity.ElectionTypeMasterId &&
                d.BoothMasterId == checkEventActivity.BoothMasterId
            );

            if (result is not null && result.IsFinalVote == true)
            {
                return new ServiceResponse()
                {
                    IsSucceed = true,
                };

            }
            else
            {
                return new ServiceResponse()
                {
                    IsSucceed = false,
                    Message = "Final Vote Not Done Yet"
                };

            }
        }

        public async Task<ServiceResponse> IsPollEnded(CheckEventActivity checkEventActivity)
        {
            // Fetch the record from the ElectionInfoMaster table that matches the UpdateEventActivity fields
            var result = await _context.ElectionInfoMaster.FirstOrDefaultAsync(d =>
                d.StateMasterId == checkEventActivity.StateMasterId &&
                d.DistrictMasterId == checkEventActivity.DistrictMasterId &&
                d.AssemblyMasterId == checkEventActivity.AssemblyMasterId &&
                d.ElectionTypeMasterId == checkEventActivity.ElectionTypeMasterId &&
                d.BoothMasterId == checkEventActivity.BoothMasterId
            );

            if (result is not null && result.IsPollEnded == true)
            {
                return new ServiceResponse()
                {
                    IsSucceed = true,
                };

            }
            else
            {
                return new ServiceResponse()
                {
                    IsSucceed = false,
                    Message = "Poll Not Ended Yet"
                };

            }
        }

        public async Task<ServiceResponse> IsEVMVVPATOff(CheckEventActivity checkEventActivity)
        {
            // Fetch the record from the ElectionInfoMaster table that matches the UpdateEventActivity fields
            var result = await _context.ElectionInfoMaster.FirstOrDefaultAsync(d =>
                d.StateMasterId == checkEventActivity.StateMasterId &&
                d.DistrictMasterId == checkEventActivity.DistrictMasterId &&
                d.AssemblyMasterId == checkEventActivity.AssemblyMasterId &&
                d.ElectionTypeMasterId == checkEventActivity.ElectionTypeMasterId &&
                d.BoothMasterId == checkEventActivity.BoothMasterId
            );

            if (result is not null && result.IsMCESwitchOff == true)
            {
                return new ServiceResponse()
                {
                    IsSucceed = true,
                };

            }
            else
            {
                return new ServiceResponse()
                {
                    IsSucceed = false,
                    Message = "EVM Machine is not switched off Yet"
                };

            }
        }

        public async Task<ServiceResponse> IsPartyDeparted(CheckEventActivity checkEventActivity)
        {
            // Fetch the record from the ElectionInfoMaster table that matches the UpdateEventActivity fields
            var result = await _context.ElectionInfoMaster.FirstOrDefaultAsync(d =>
                d.StateMasterId == checkEventActivity.StateMasterId &&
                d.DistrictMasterId == checkEventActivity.DistrictMasterId &&
                d.AssemblyMasterId == checkEventActivity.AssemblyMasterId &&
                d.ElectionTypeMasterId == checkEventActivity.ElectionTypeMasterId &&
                d.BoothMasterId == checkEventActivity.BoothMasterId
            );

            if (result is not null && result.IsPartyDeparted == true)
            {
                return new ServiceResponse()
                {
                    IsSucceed = true,
                };

            }
            else
            {
                return new ServiceResponse()
                {
                    IsSucceed = false,
                    Message = "Party Not Departed Yet"
                };

            }
        }

        public async Task<ServiceResponse> IsPartyReachedAtCollection(CheckEventActivity checkEventActivity)
        {
            // Fetch the record from the ElectionInfoMaster table that matches the UpdateEventActivity fields
            var result = await _context.ElectionInfoMaster.FirstOrDefaultAsync(d =>
                d.StateMasterId == checkEventActivity.StateMasterId &&
                d.DistrictMasterId == checkEventActivity.DistrictMasterId &&
                d.AssemblyMasterId == checkEventActivity.AssemblyMasterId &&
                d.ElectionTypeMasterId == checkEventActivity.ElectionTypeMasterId &&
                d.BoothMasterId == checkEventActivity.BoothMasterId
            );

            if (result is not null && result.IsPartyReachedCollectionCenter == true)
            {
                return new ServiceResponse()
                {
                    IsSucceed = true,
                };

            }
            else
            {
                return new ServiceResponse()
                {
                    IsSucceed = false,
                    Message = "Party Not Reached At Collection Center Yet"
                };

            }
        }

        public async Task<ServiceResponse> IsEVMDeposited(CheckEventActivity checkEventActivity)
        {
            // Fetch the record from the ElectionInfoMaster table that matches the UpdateEventActivity fields
            var result = await _context.ElectionInfoMaster.FirstOrDefaultAsync(d =>
                d.StateMasterId == checkEventActivity.StateMasterId &&
                d.DistrictMasterId == checkEventActivity.DistrictMasterId &&
                d.AssemblyMasterId == checkEventActivity.AssemblyMasterId &&
                d.ElectionTypeMasterId == checkEventActivity.ElectionTypeMasterId &&
                d.BoothMasterId == checkEventActivity.BoothMasterId
            );

            if (result is not null && result.IsEVMDeposited == true)
            {
                return new ServiceResponse()
                {
                    IsSucceed = true,
                };

            }
            else
            {
                return new ServiceResponse()
                {
                    IsSucceed = false,
                    Message = "EVAM Not Deposited Yet"
                };

            }
        }
        #endregion

        public async Task<VoterTurnOutPolledDetailViewModel> GetLastUpdatedPollDetail(int boothMasterId)
        {
            // Step 1: Try to fetch BoothMaster details from cache
            var cacheKeyBooth = $"BoothMaster_{boothMasterId}";
            var getBooth = await _cacheService.GetDataAsync<BoothMaster>(cacheKeyBooth);

            if (getBooth == null)
            {
                // Fetch from database if not available in cache
                getBooth = await _context.BoothMaster
                                .Where(d => d.BoothMasterId == boothMasterId)
                                .Select(d => new BoothMaster
                                {
                                    BoothMasterId = d.BoothMasterId,
                                    StateMasterId = d.StateMasterId,
                                    ElectionTypeMasterId = d.ElectionTypeMasterId,
                                    TotalVoters = d.TotalVoters,

                                })
                                .FirstOrDefaultAsync();

                // Add to cache if found
                if (getBooth != null)
                {
                    await _cacheService.SetDataAsync(cacheKeyBooth, getBooth, BharatTimeDynamic(0, 0, 0, 10, 0)); // Cache for 30 minutes
                }
            }

            if (getBooth == null)
            {
                return null; // Handle case when no booth is found
            }


            var currentEvent = await _cacheService.GetDataAsync<EventMaster>("GetVTEvent");

            if (currentEvent == null)
            {
                // Fetch from database if not available in cache
                currentEvent = await _context.EventMaster
                                   .FirstOrDefaultAsync(d => d.StateMasterId == getBooth.StateMasterId
                                                         && d.ElectionTypeMasterId == getBooth.ElectionTypeMasterId
                                                         && d.EventABBR == "VT");

                // Add to cache if found
                if (currentEvent != null)
                {
                    await _cacheService.SetDataAsync("GetVTEvent", currentEvent, BharatTimeDynamic(0, 0, 0, 10, 0)); // Cache for 30 minutes
                }
            }

            if (currentEvent == null)
            {
                return null; // Handle case when no current event is found
            }

            // Step 3: Fetch election info (not cached as it may be frequently updated)
            var electionInfo = await _context.ElectionInfoMaster
                                   .FirstOrDefaultAsync(d => d.BoothMasterId == boothMasterId
                                                          && d.StateMasterId == getBooth.StateMasterId
                                                          && d.ElectionTypeMasterId == getBooth.ElectionTypeMasterId);

            if (electionInfo == null)
            {
                return null; // Handle case when no election info is found
            }


            // Step 4: Get voter slot availability
            var getVoterSlotAvailable = await GetVoterSlotAvailable(getBooth.StateMasterId, getBooth.ElectionTypeMasterId);
            // Step 5: Populate ViewModel and return
            VoterTurnOutPolledDetailViewModel voterTurnOutPolledDetailViewModel = new VoterTurnOutPolledDetailViewModel
            {
                BoothMasterId = boothMasterId,
                StateMasterId = getBooth.StateMasterId,
                ElectionTypeMasterId = getBooth.ElectionTypeMasterId,
                EventMasterId = currentEvent.EventMasterId,
                EventABBR = currentEvent.EventABBR,
                EventName = currentEvent.EventName,
                EventSequence = currentEvent.EventSequence,
                TotalVoters = getBooth.TotalVoters,
                VotesPolled = electionInfo.FinalVote,
                VotesPolledRecivedTime = electionInfo.VotingLastUpdate

            };
            var getLastSlot = await GetLastSlot(electionInfo.StateMasterId, electionInfo.EventMasterId, electionInfo.ElectionTypeMasterId);

            if (getLastSlot.IsLastSlot == true && getLastSlot.LockTime.HasValue)
            {
                // Get the current time in TimeOnly format
                var currentTime = TimeOnly.FromDateTime(DateTime.Now);

                // Compare LockTime with the current time
                bool checkTimeExceeded = getLastSlot.LockTime.Value < currentTime;

                if (checkTimeExceeded)
                {
                    voterTurnOutPolledDetailViewModel.IsSlotAvailable = false;
                    voterTurnOutPolledDetailViewModel.Message = "Kindly Proceed for Voter In Queue ";
                    electionInfo.IsVoterTurnOut = true;
                    _context.Update(electionInfo);
                    _context.SaveChanges();
                    return voterTurnOutPolledDetailViewModel;
                }
            }
            if (getVoterSlotAvailable == null)
            {

                voterTurnOutPolledDetailViewModel.IsSlotAvailable = false;
                voterTurnOutPolledDetailViewModel.Message = "Slot Not Available";

            }
            else
            {
                voterTurnOutPolledDetailViewModel.StartTime = getVoterSlotAvailable.StartTime;
                voterTurnOutPolledDetailViewModel.EndTime = getVoterSlotAvailable.EndTime;
                voterTurnOutPolledDetailViewModel.LockTime = getVoterSlotAvailable.LockTime;
                voterTurnOutPolledDetailViewModel.IsSlotAvailable = true;
                voterTurnOutPolledDetailViewModel.Message = "Slot  Available";
            }
            return voterTurnOutPolledDetailViewModel;
        }
        private async Task<SlotManagementMaster> GetLastSlot(int stateMasterId, int eventmasterid, int electionTypeMasterId)
        {

            var lastSlot = _context.SlotManagementMaster.Where(p => p.StateMasterId == stateMasterId && p.EventMasterId == eventmasterid && p.ElectionTypeMasterId == electionTypeMasterId && p.IsLastSlot == true).FirstOrDefault();

            if (lastSlot == null)
            {
                return null;

            }
            else
            {
                return lastSlot;



            }


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
                var sumOfFinalTVoteSum = _context.ElectionInfoMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId)).Sum(p => p.FinalVote);
                var finalIsTrueSum = _context.ElectionInfoMaster.Where(p => p.IsFinalVote == true && p.StateMasterId == Convert.ToInt32(stateMasterId)).Sum(p => p.FinalVote);


                totalVoters.Add(totalVotersSum);
                sumOfFinalTVote.Add(sumOfFinalTVoteSum);
                finalIsTrue.Add(finalIsTrueSum);
            }
            else if (roles == "DistrictAdmin")
            {

                var totalVotersSum = _context.BoothMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId))
                   .Sum(p => p.TotalVoters);
                var sumOfFinalTVoteSum = _context.ElectionInfoMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId))
                   .Sum(p => p.FinalVote);
                var finalIsTrueSum = _context.ElectionInfoMaster.Where(d => d.IsFinalVote == true && d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId))
                    .Sum(p => p.FinalVote);


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
                var sumOfFinalTVoteSum = _context.ElectionInfoMaster.Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId)).Sum(p => p.FinalVote);
                var finalIsTrueSum = _context.ElectionInfoMaster.Where(d => d.IsFinalVote == true && d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId))
                    .Sum(p => p.FinalVote);


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
                                            Message = "Queue is Available",
                                            ElectionTypeMasterId = electionInfoRecord.ElectionTypeMasterId


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
                                            Message = "Queue is Available, You have not entered any value in Voter Turn Out of this Booth.",
                                            ElectionTypeMasterId = electionInfoRecord.ElectionTypeMasterId


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
                                        ElectionTypeMasterId = electionInfoRecord.ElectionTypeMasterId,
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
                                    ElectionTypeMasterId = electionInfoRecord.ElectionTypeMasterId,
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
                                ElectionTypeMasterId = electionInfoRecord.ElectionTypeMasterId,
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
                            ElectionTypeMasterId = 0,
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
                        Message = "Booth record Not Found",
                        ElectionTypeMasterId = 0,



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
            return null;
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
            if (electionInfo != null && (electionInfo.IsFinalVote == null || electionInfo.IsFinalVote == false))
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
        public async Task<FinalViewModel> GetFinalVotes(int boothMasterId)
        {
            // Step 1: Try to fetch BoothMaster details from cache
            var cacheKeyBooth = $"GetFinalBoothMaster_{boothMasterId}";
            var getBooth = await _cacheService.GetDataAsync<BoothMaster>(cacheKeyBooth);

            if (getBooth == null)
            {
                // Fetch from database if not available in cache
                getBooth = await _context.BoothMaster
                                .Where(d => d.BoothMasterId == boothMasterId)
                                .Select(d => new BoothMaster
                                {
                                    BoothMasterId = d.BoothMasterId,
                                    StateMasterId = d.StateMasterId,
                                    ElectionTypeMasterId = d.ElectionTypeMasterId,
                                    TotalVoters = d.TotalVoters,
                                    Male = d.Male,
                                    Female = d.Female,
                                    Transgender = d.Transgender

                                })
                                .FirstOrDefaultAsync();

                // Add to cache if found
                if (getBooth != null)
                {
                    await _cacheService.SetDataAsync(cacheKeyBooth, getBooth, BharatTimeDynamic(0, 0, 0, 10, 0)); // Cache for 30 minutes
                }
            }

            if (getBooth == null)
            {
                return null; // Handle case when no booth is found
            }


            var currentEvent = await _cacheService.GetDataAsync<EventMaster>("GetVTEvent");

            if (currentEvent == null)
            {
                // Fetch from database if not available in cache
                currentEvent = await _context.EventMaster
                                   .FirstOrDefaultAsync(d => d.StateMasterId == getBooth.StateMasterId
                                                         && d.ElectionTypeMasterId == getBooth.ElectionTypeMasterId
                                                         && d.EventABBR == "VT");

                // Add to cache if found
                if (currentEvent != null)
                {
                    await _cacheService.SetDataAsync("GetVTEvent", currentEvent, BharatTimeDynamic(0, 0, 0, 10, 0)); // Cache for 30 minutes
                }
            }

            if (currentEvent == null)
            {
                return null; // Handle case when no current event is found
            }

            // Step 3: Fetch election info (not cached as it may be frequently updated)
            var electionInfo = await _context.ElectionInfoMaster
                                   .FirstOrDefaultAsync(d => d.BoothMasterId == boothMasterId
                                                          && d.StateMasterId == getBooth.StateMasterId
                                                          && d.ElectionTypeMasterId == getBooth.ElectionTypeMasterId);

            if (electionInfo == null)
            {
                return null; // Handle case when no election info is found
            }
            var votesPolled = await _context.PollDetails
                .Where(d => d.StateMasterId == electionInfo.StateMasterId
                            && d.DistrictMasterId == electionInfo.DistrictMasterId
                            && d.AssemblyMasterId == electionInfo.AssemblyMasterId
                            && d.BoothMasterId == electionInfo.BoothMasterId
                            && d.ElectionTypeMasterId == electionInfo.ElectionTypeMasterId
                            && d.EventABBR == currentEvent.EventABBR
                            && d.EventSequence == currentEvent.EventSequence
                            && d.EventMasterId == currentEvent.EventMasterId)
                .OrderByDescending(d => d.PollDetailMasterId) // Ensure ordering before using LastOrDefault
                .LastOrDefaultAsync();

            if (votesPolled is null || electionInfo is null && electionInfo.IsVoterInQueue == false)
            {
                return null;
            }
            // Step 5: Populate ViewModel and return
            FinalViewModel finalViewModel = new FinalViewModel
            {
                BoothMasterId = boothMasterId,
                ElectionTypeMasterId = getBooth.ElectionTypeMasterId,
                TotalVoters = getBooth.TotalVoters,
                LastVotesPolled = votesPolled.VotesPolled,
                TotalAvailableMale = getBooth.Male,
                TotalAvailableFemale = getBooth.Female,
                TotalAvailableTransgender = getBooth.Transgender,
                Male = electionInfo.Male,
                Female = electionInfo.Female,
                Transgender = electionInfo.Transgender,
                LastFinalVotesPolled = electionInfo.FinalVote,
                VotesFinalPolledTime = electionInfo.FinalVoteLastUpdate
            };

            return finalViewModel;
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
        /// 
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





            return lastpolldetail;
        }

        public async Task<string> GetPollDetailforPCWise(int stateId, int? pcMasterId)
        {
            int lastpolldetail = 0;





            return lastpolldetail.ToString();
        }

        public async Task<int> GetPollDetailById(int assemblyId, string type)
        {
            int lastpolldetail = 0;



            return lastpolldetail;
        }
        public async Task<int> GetFinalVotesById(int keyMasterId, string type)
        {
            int finalVotes = 0;
            if (keyMasterId != 0 && type == "Assembly")
            {
                var finalVotesValue = await _context.ElectionInfoMaster
                    .Where(p => p.AssemblyMasterId == keyMasterId)
                    .Select(p => p.FinalVote)
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
            var finalVotesValue = _context.ElectionInfoMaster.FirstOrDefault(p => p.BoothMasterId == boothMasterId).FinalVote;
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
            var masterIds = slotManagement.Select(d => new { d.StateMasterId, d.EventMasterId, d.ElectionTypeMasterId }).FirstOrDefault();
            var deleteRecord = _context.SlotManagementMaster
                .Where(d => d.StateMasterId == masterIds.StateMasterId && d.ElectionTypeMasterId == masterIds.ElectionTypeMasterId && d.EventMasterId == masterIds.EventMasterId)
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

        public async Task<List<SlotManagementMaster>> GetEventSlotList(int stateMasterId, int electionTypeMasterId, int eventId)
        {
            var slotList = await _context.SlotManagementMaster.Where(d => d.StateMasterId == stateMasterId
                                                                    && d.ElectionTypeMasterId == electionTypeMasterId
                                                                    && d.EventMasterId == eventId).ToListAsync();
            return slotList;
        }
        #endregion

        #region UserList
        public async Task<List<UserList>> GetUserList(string soName, string type)
        {
            var users = await _context.FieldOfficerMaster
            .Where(u => EF.Functions.Like(u.FieldOfficerOfficeName.ToUpper(), "%" + soName.ToUpper() + "%"))
            .OrderBy(u => u.FieldOfficerMasterId)
            .Select(d => new UserList
            {
                Name = d.FieldOfficerName,
                MobileNumber = d.FieldOfficerMobile,
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
        //public async Task<PollInterruption> GetPollInterruptionData(string boothMasterId)
        //{
        //    var pollInterruptionRecord = await _context.PollInterruptions.Where(d => d.BoothMasterId == Convert.ToInt32(boothMasterId)).OrderByDescending(p => p.PollInterruptionId).FirstOrDefaultAsync();
        //    return pollInterruptionRecord;
        //}
        public async Task<PollInterruption> GetPollInterruptionData(string boothMasterId)
        {
            var pollInterruptionRecord = await _context.PollInterruptions
                .Where(d => d.BoothMasterId == Convert.ToInt32(boothMasterId))
                .OrderByDescending(p => p.PollInterruptionId)
                .FirstOrDefaultAsync();

            // Handle case where no record is found
            if (pollInterruptionRecord == null)
            {
                return null;
            }

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
                            //pollInterruption.PCMasterId,

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
                //PCMasterId = p.PCMasterId,
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
                                 //PCMasterId = pi.PCMasterId,
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
                                 //PCMasterId = pi.PCMasterId,
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
                                 //PCMasterId = pi.PCMasterId,
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
                                 //PCMasterId = pi.PCMasterId,
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
                                 //PCMasterId = pi.PCMasterId,
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
                                     //PCMasterId = pi.PCMasterId,
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
                           .Where(pi => pi.StateMasterId == Convert.ToInt32(sid))
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
                                 //PCMasterId = pi.PCMasterId,
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
            //    if (model.StateMasterId is not 0 && model.DistrictMasterId is not 0 && model.AssemblyMasterId is not 0 && model.PCMasterId is 0)
            //    {
            //        var isDistrictActive = _context.DistrictMaster.Where(d => d.StateMasterId == model.StateMasterId && d.DistrictMasterId == model.DistrictMasterId).FirstOrDefault();
            //        if (isStateActive.StateStatus && isDistrictActive.DistrictStatus && isAssemblyActive.AssemblyStatus)
            //        {
            //            List<LocationModelList> locationModelLists = new List<LocationModelList>();
            //            var locations = from lc in _context.PollingLocationMaster.Where(d => d.AssemblyMasterId == model.AssemblyMasterId && d.StateMasterId == d.StateMasterId && d.DistrictMasterId == d.DistrictMasterId)
            //                            join asem in _context.AssemblyMaster
            //                             on lc.AssemblyMasterId equals asem.AssemblyMasterId
            //                            join dist in _context.DistrictMaster
            //                            on asem.DistrictMasterId equals dist.DistrictMasterId
            //                            join state in _context.StateMaster
            //                             on dist.StateMasterId equals state.StateMasterId
            //                            join pc in _context.ParliamentConstituencyMaster
            //                            //on lc.PCMasterId equals pc.PCMasterId
            //                            on asem.PCMasterId equals pc.PCMasterId

            //                            select new LocationModelList
            //                            {
            //                                StateMasterId = state.StateMasterId,
            //                                PCMasterId = pc.PCMasterId,
            //                                PCName = pc.PcName,
            //                                DistrictMasterId = dist.DistrictMasterId,
            //                                AssemblyMasterId = asem.AssemblyMasterId,
            //                                StateName = state.StateName,
            //                                DistrictName = dist.DistrictName,
            //                                AssemblyName = asem.AssemblyName,
            //                                LocationMasterId = lc.LocationMasterId,
            //                                LocationName = lc.LocationName,
            //                                LocationCode = lc.LocationCode,
            //                                LocationLatitude = lc.LocationLatitude,
            //                                LocationLongitude = lc.LocationLongitude,
            //                                IsStatus = lc.Status,
            //                                CreatedOn = lc.CreatedOn,

            //                            };

            //            foreach (var location in locations)
            //            {
            //                var boothNumbers = _context.BoothMaster
            //.Where(d => d.LocationMasterId == location.LocationMasterId)
            //.Select(d => int.Parse(d.BoothCode_No))
            //.ToList();


            //                location.BoothNumbers = string.Join(", ", boothNumbers);
            //                locationModelLists.Add(location);
            //            }
            //            //var sortedlocationList = await locationModelLists.ToListAsync();
            //            return locationModelLists;
            //        }
            //        else
            //        {
            //            return null;

            //        }
            //    }
            //    //state,pc,ase.m
            //    else if (model.StateMasterId is not 0 && model.DistrictMasterId is 0 && model.AssemblyMasterId is not 0 && model.PCMasterId is not 0)
            //    {
            //        var isPCRecord = _context.ParliamentConstituencyMaster.Where(d => d.StateMasterId == model.StateMasterId && d.PCMasterId == model.PCMasterId).FirstOrDefault();
            //        if (isPCRecord != null)
            //        {
            //            List<LocationModelList> locationModelLists = new List<LocationModelList>();
            //            var locations = from lc in _context.PollingLocationMaster.Where(d => d.AssemblyMasterId == model.AssemblyMasterId && d.StateMasterId == d.StateMasterId && d.DistrictMasterId == d.DistrictMasterId)
            //                            join asem in _context.AssemblyMaster
            //                             on lc.AssemblyMasterId equals asem.AssemblyMasterId
            //                            join dist in _context.DistrictMaster
            //                            on asem.DistrictMasterId equals dist.DistrictMasterId
            //                            join state in _context.StateMaster
            //                             on dist.StateMasterId equals state.StateMasterId
            //                            join pc in _context.ParliamentConstituencyMaster
            //                            on asem.PCMasterId equals pc.PCMasterId
            //                            // on lc.PCMasterId equals pc.PCMasterId

            //                            select new LocationModelList
            //                            {
            //                                StateMasterId = state.StateMasterId,
            //                                PCMasterId = pc.PCMasterId,
            //                                PCName = pc.PcName,
            //                                DistrictMasterId = dist.DistrictMasterId,
            //                                AssemblyMasterId = asem.AssemblyMasterId,
            //                                StateName = state.StateName,
            //                                DistrictName = dist.DistrictName,
            //                                AssemblyName = asem.AssemblyName,
            //                                LocationMasterId = lc.LocationMasterId,
            //                                LocationName = lc.LocationName,
            //                                LocationCode = lc.LocationCode,
            //                                LocationLatitude = lc.LocationLatitude,
            //                                LocationLongitude = lc.LocationLongitude,
            //                                IsStatus = lc.Status,
            //                                CreatedOn = lc.CreatedOn,

            //                            };

            //            foreach (var location in locations)
            //            {
            //                var boothNumbers = _context.BoothMaster
            //.Where(d => d.LocationMasterId == location.LocationMasterId)
            //.Select(d => int.Parse(d.BoothCode_No))
            //.ToList();


            //                location.BoothNumbers = string.Join(", ", boothNumbers);
            //                locationModelLists.Add(location);
            //            }
            //            //var sortedlocationList = await locationModelLists.ToListAsync();
            //            return locationModelLists;
            //        }
            //        else
            //        {
            //            return null;
            //        }
            //    }
            //    else
            //    {
            //        return null;
            //    }
            return null;

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
            var query = _context.BoothMaster
                .AsQueryable();
            string reportType = "";
            ////State
            //if (boothReportModel.StateMasterId != 0 && boothReportModel.DistrictMasterId == 0 && boothReportModel.AssemblyMasterId == 0 && boothReportModel.FourthLevelHMasterId == 0 && boothReportModel.PSZonePanchayatMasterId == 0)
            //{
            //  query = query.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.ElectionTypeMasterId == boothReportModel.ElectionTypeMasterId);
            // query = query.Include(d => d.StateMaster);
            //  reportType = "State";
            //}
            //District
            if (boothReportModel.StateMasterId != 0 && boothReportModel.DistrictMasterId != 0 && boothReportModel.AssemblyMasterId == 0 && boothReportModel.FourthLevelHMasterId == 0 && boothReportModel.PSZonePanchayatMasterId == 0)
            {
                query = query.Where(d => d.ElectionTypeMasterId == boothReportModel.ElectionTypeMasterId
                && d.StateMasterId == boothReportModel.StateMasterId
                && d.DistrictMasterId == boothReportModel.DistrictMasterId);
                query = query.Include(d => d.DistrictMaster);
                reportType = "District";
            }
            //Assembly
            if (boothReportModel.StateMasterId != 0 && boothReportModel.DistrictMasterId != 0 && boothReportModel.AssemblyMasterId != 0 && boothReportModel.FourthLevelHMasterId == 0 && boothReportModel.PSZonePanchayatMasterId == 0)
            {
                query = query.Where(d => d.ElectionTypeMasterId == boothReportModel.ElectionTypeMasterId
                && d.StateMasterId == boothReportModel.StateMasterId
                && d.DistrictMasterId == boothReportModel.DistrictMasterId &&
                  d.AssemblyMasterId == boothReportModel.AssemblyMasterId);
                query = query.Include(d => d.AssemblyMaster);
                reportType = "Local Bodies";
            }
            //FourthLevel
            if (boothReportModel.StateMasterId != 0 && boothReportModel.DistrictMasterId != 0 && boothReportModel.AssemblyMasterId != 0 && boothReportModel.FourthLevelHMasterId != 0 && boothReportModel.PSZonePanchayatMasterId == 0)
            {
                query = query.Where(d => d.ElectionTypeMasterId == boothReportModel.ElectionTypeMasterId
                && d.StateMasterId == boothReportModel.StateMasterId
                && d.DistrictMasterId == boothReportModel.DistrictMasterId &&
                  d.AssemblyMasterId == boothReportModel.AssemblyMasterId
                && d.FourthLevelHMasterId == boothReportModel.FourthLevelHMasterId);
                query = query.Include(d => d.FourthLevelH);
                reportType = "Sub Local Bodies";
            }
            //PSZonePanchayat
            if (boothReportModel.StateMasterId != 0 && boothReportModel.DistrictMasterId != 0 && boothReportModel.AssemblyMasterId != 0 && boothReportModel.FourthLevelHMasterId != 0 && boothReportModel.PSZonePanchayatMasterId != 0)
            {
                query = query.Where(d => d.ElectionTypeMasterId == boothReportModel.ElectionTypeMasterId
                && d.StateMasterId == boothReportModel.StateMasterId
                && d.DistrictMasterId == boothReportModel.DistrictMasterId &&
                  d.AssemblyMasterId == boothReportModel.AssemblyMasterId
                && d.FourthLevelHMasterId == boothReportModel.FourthLevelHMasterId &&
                d.PSZonePanchayatMasterId == boothReportModel.PSZonePanchayatMasterId);
                query = query.Include(d => d.PsZonePanchayat);
                reportType = "PSZonePanchayat";
            }

            return await query.Select(d => new ConsolidateBoothReport
            {
                Header = boothReportModel.PSZonePanchayatMasterId != 0
            ? $"{d.StateMaster.StateName} ({d.StateMaster.StateCode}) {d.DistrictMaster.DistrictName} ({d.DistrictMaster.DistrictCode}) {d.AssemblyMaster.AssemblyName} ({d.AssemblyMaster.AssemblyCode}) {d.FourthLevelH.HierarchyName} ({d.FourthLevelH.HierarchyCode}) {d.PsZonePanchayat.PSZonePanchayatName} ({d.PsZonePanchayat.PSZonePanchayatCode})"
            : boothReportModel.FourthLevelHMasterId != 0
            ? $"{d.StateMaster.StateName} ({d.StateMaster.StateCode}) {d.DistrictMaster.DistrictName} ({d.DistrictMaster.DistrictCode}) {d.AssemblyMaster.AssemblyName} ({d.AssemblyMaster.AssemblyCode}) {d.FourthLevelH.HierarchyName} ({d.FourthLevelH.HierarchyCode})"
            : boothReportModel.AssemblyMasterId != 0
            ? $"{d.StateMaster.StateName} ({d.StateMaster.StateCode}) {d.DistrictMaster.DistrictName} ({d.DistrictMaster.DistrictCode}) {d.AssemblyMaster.AssemblyName} ({d.AssemblyMaster.AssemblyCode})"
            : boothReportModel.DistrictMasterId != 0
            ? $"{d.StateMaster.StateName} ({d.StateMaster.StateCode}) {d.DistrictMaster.DistrictName} ({d.DistrictMaster.DistrictCode})"
            : $"{d.StateMaster.StateName} ({d.StateMaster.StateCode})",

                Title = boothReportModel.StateMasterId != 0 && boothReportModel.DistrictMasterId == 0 && boothReportModel.AssemblyMasterId == 0 && boothReportModel.FourthLevelHMasterId == 0 && boothReportModel.PSZonePanchayatMasterId == 0
    ? d.DistrictMaster.DistrictName // Show DistrictName when only StateMasterId is provided
    : boothReportModel.StateMasterId != 0 && boothReportModel.DistrictMasterId != 0 && boothReportModel.AssemblyMasterId == 0 && boothReportModel.FourthLevelHMasterId == 0 && boothReportModel.PSZonePanchayatMasterId == 0
    ? d.DistrictMaster.DistrictName // Show DistrictName when State and District are provided
    : boothReportModel.StateMasterId != 0 && boothReportModel.DistrictMasterId != 0 && boothReportModel.AssemblyMasterId != 0 && boothReportModel.FourthLevelHMasterId == 0 && boothReportModel.PSZonePanchayatMasterId == 0
    ? d.AssemblyMaster.AssemblyName // Show AssemblyName when State, District, and Assembly are provided
    : boothReportModel.StateMasterId != 0 && boothReportModel.DistrictMasterId != 0 && boothReportModel.AssemblyMasterId != 0 && boothReportModel.FourthLevelHMasterId != 0 && boothReportModel.PSZonePanchayatMasterId == 0
    ? d.FourthLevelH.HierarchyName // Show FourthLevelH when State, District, Assembly, and FourthLevelH are provided
    : boothReportModel.StateMasterId != 0 && boothReportModel.DistrictMasterId != 0 && boothReportModel.AssemblyMasterId != 0 && boothReportModel.FourthLevelHMasterId != 0 && boothReportModel.PSZonePanchayatMasterId != 0
    ? d.PsZonePanchayat.PSZonePanchayatName // Show PSZonePanchayat when all IDs are provided
    : d.StateMaster.StateName, // Default to StateName

                Type = reportType,
                Code = d.BoothCode_No.ToString(),
                Name = d.BoothName,
                DistrictName = d.DistrictMaster.DistrictName,
                AssemblyName = d.AssemblyMaster.AssemblyName,
                HierarchyName = d.FourthLevelH.HierarchyName,
                PSZonePanchayatName = d.PsZonePanchayat.PSZonePanchayatName,
                //TotalNumberOfBooths = d.FourthLevelH.BoothMaster.Count,
                //TotalNumberOfBoothsEntered = d.AssemblyMaster.TotalBooths,
                Male = d.Male,
                Female = d.Female,
                Trans = d.Transgender,
                Total = d.TotalVoters,
                IsStatus = d.BoothStatus
            }).ToListAsync();
        }

        public async Task<List<ConsolidateBoothReport>> GetConsolidateGPWardReports(BoothReportModel boothReportModel)
        {
            var query = _context.GPPanchayatWards
                .AsQueryable();

            string reportType = "";

            //// State

            //if (boothReportModel.StateMasterId != 0 && boothReportModel.DistrictMasterId == 0 && boothReportModel.AssemblyMasterId == 0 && boothReportModel.FourthLevelHMasterId == 0 && boothReportModel.PSZonePanchayatMasterId == 0)
            //{
            //    query = query.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.ElectionTypeMasterId == boothReportModel.ElectionTypeMasterId);
            //    reportType = "State";
            //}
            // District
            //if (boothReportModel.StateMasterId != 0 && boothReportModel.DistrictMasterId != 0 && boothReportModel.AssemblyMasterId == 0 && boothReportModel.FourthLevelHMasterId == 0)
            //{
            //    query = query.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.ElectionTypeMasterId == boothReportModel.ElectionTypeMasterId);
            //    query = query.Include(d => d.StateMaster);
            //    reportType = "State";
            //}
            // District
            if (boothReportModel.StateMasterId != 0 && boothReportModel.DistrictMasterId != 0 && boothReportModel.AssemblyMasterId == 0 && boothReportModel.FourthLevelHMasterId == 0)
            {
                query = query.Where(d => d.StateMasterId == boothReportModel.StateMasterId
                && d.ElectionTypeMasterId == boothReportModel.ElectionTypeMasterId
                && d.DistrictMasterId == boothReportModel.DistrictMasterId);
                query = query.Include(d => d.DistrictMaster);
                reportType = "District";
            }
            // Assembly
            else if (boothReportModel.StateMasterId != 0 && boothReportModel.DistrictMasterId != 0 && boothReportModel.AssemblyMasterId != 0 && boothReportModel.FourthLevelHMasterId == 0)
            {
                query = query.Where(d => d.StateMasterId == boothReportModel.StateMasterId
                && d.ElectionTypeMasterId == boothReportModel.ElectionTypeMasterId
                && d.DistrictMasterId == boothReportModel.DistrictMasterId
                && d.AssemblyMasterId == boothReportModel.AssemblyMasterId);
                query = query.Include(d => d.AssemblyMaster);
                reportType = "Local Bodies";
            }
            // FourthLevel
            else if (boothReportModel.StateMasterId != 0 && boothReportModel.DistrictMasterId != 0 && boothReportModel.AssemblyMasterId != 0 && boothReportModel.FourthLevelHMasterId != 0)
            {
                query = query.Where(d => d.StateMasterId == boothReportModel.StateMasterId
                && d.ElectionTypeMasterId == boothReportModel.ElectionTypeMasterId
                && d.DistrictMasterId == boothReportModel.DistrictMasterId
                && d.AssemblyMasterId == boothReportModel.AssemblyMasterId
                && d.FourthLevelHMasterId == boothReportModel.FourthLevelHMasterId);
                query = query.Include(d => d.FourthLevelH);
                reportType = "Sub Local Bodies";
            }


            return await query.Select(d => new ConsolidateBoothReport
            {
                Header = boothReportModel.FourthLevelHMasterId != 0
    ? $"{d.StateMaster.StateName} ({d.StateMaster.StateCode}) {d.DistrictMaster.DistrictName} ({d.DistrictMaster.DistrictCode}) {d.AssemblyMaster.AssemblyName} ({d.AssemblyMaster.AssemblyCode}) {d.FourthLevelH.HierarchyName} ({d.FourthLevelH.HierarchyCode})"
    : boothReportModel.AssemblyMasterId != 0
    ? $"{d.StateMaster.StateName} ({d.StateMaster.StateCode}) {d.DistrictMaster.DistrictName} ({d.DistrictMaster.DistrictCode}) {d.AssemblyMaster.AssemblyName} ({d.AssemblyMaster.AssemblyCode})"
    : boothReportModel.DistrictMasterId != 0
    ? $"{d.StateMaster.StateName} ({d.StateMaster.StateCode}) {d.DistrictMaster.DistrictName} ({d.DistrictMaster.DistrictCode})"
    : $"{d.StateMaster.StateName} ({d.StateMaster.StateCode})",

                Title = boothReportModel.StateMasterId != 0 && boothReportModel.DistrictMasterId == 0 && boothReportModel.AssemblyMasterId == 0 && boothReportModel.FourthLevelHMasterId == 0
    ? d.DistrictMaster.DistrictName // Show DistrictName when only StateMasterId is provided
    : boothReportModel.StateMasterId != 0 && boothReportModel.DistrictMasterId != 0 && boothReportModel.AssemblyMasterId == 0 && boothReportModel.FourthLevelHMasterId == 0
    ? d.DistrictMaster.DistrictName // Show DistrictName when State and District are provided
    : boothReportModel.StateMasterId != 0 && boothReportModel.DistrictMasterId != 0 && boothReportModel.AssemblyMasterId != 0 && boothReportModel.FourthLevelHMasterId == 0
    ? d.AssemblyMaster.AssemblyName // Show AssemblyName when State, District, and Assembly are provided
    : boothReportModel.StateMasterId != 0 && boothReportModel.DistrictMasterId != 0 && boothReportModel.AssemblyMasterId != 0 && boothReportModel.FourthLevelHMasterId != 0
    ? d.FourthLevelH.HierarchyName // Show FourthLevelH when State, District, Assembly, and FourthLevelH are provided
    : d.StateMaster.StateName,


                Type = reportType,
                Code = d.GPPanchayatWardsCode.ToString(),
                Name = d.GPPanchayatWardsName,
                DistrictName = d.DistrictMaster.DistrictName,
                AssemblyName = d.AssemblyMaster.AssemblyName,
                HierarchyName = d.FourthLevelH.HierarchyName,
                //TotalNumberOfBooths = d.FourthLevelH.BoothMaster.Count,
                //TotalNumberOfBoothsEntered = d.AssemblyMaster.TotalBooths,
                // Male = d.Male,
                //Female = d.Female,
                //Trans = d.Transgender,
                //Total = d.TotalVoters,
                IsStatus = d.GPPanchayatWardsStatus
            }).ToListAsync();
        }


        public async Task<List<SoReport>> GetSOReport(BoothReportModel boothReportModel)
        {

            //if (boothReportModel.AssemblyMasterId is not 0)
            //{
            //    List<SoReport> soReports = new List<SoReport>(); List<int> boothNumbers = new List<int>();
            //    if (boothReportModel.DistrictMasterId is not 0)
            //    {
            //        var assembly = _context.AssemblyMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId && d.AssemblyMasterId == boothReportModel.AssemblyMasterId && d.AssemblyStatus == true).Include(d => d.BoothMaster).FirstOrDefault();
            //        var sectorOfficerList = _context.SectorOfficerMaster.Where(d => d.StateMasterId == assembly.StateMasterId && d.SoAssemblyCode == assembly.AssemblyCode && d.SoStatus == true).ToList();
            //        var state = _context.StateMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.StateStatus == true).FirstOrDefault();
            //        var district = _context.DistrictMaster.Where(d => d.DistrictMasterId == boothReportModel.DistrictMasterId && d.DistrictStatus == true).FirstOrDefault();
            //        var pollingLocation = _context.PollingLocationMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId && d.AssemblyMasterId == boothReportModel.AssemblyMasterId && d.Status == true).Count();
            //        foreach (var so in sectorOfficerList)
            //        {
            //            var locationMasterId = _context.BoothMaster.Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true).Select(d => d.LocationMasterId).FirstOrDefault();
            //            SoReport report = new SoReport
            //            {
            //                SoMasterId = so.SOMasterId,
            //                Header = $"{state.StateName}({state.StateCode}),{district.DistrictName}({district.DistrictCode}),{assembly.AssemblyName}({assembly.AssemblyCode})",
            //                Title = $"{district.DistrictName}",
            //                Type = "District",
            //                TotalBoothCount = assembly.BoothMaster.Count(),
            //                TotalPollingLocationCount = pollingLocation,
            //                TotalSOAppointedCount = sectorOfficerList.Count(),
            //                SOName = so.SoName,
            //                SOMobileNo = so.SoMobile,
            //                Office = so.SoOfficeName,
            //                SODesignation = so.SoDesignation,
            //                BoothAllocatedCount = assembly.BoothMaster.Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true).Count(),
            //                BoothAllocatedName = assembly.BoothMaster
            //                                    .Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true)
            //                                    .Select(d => $"{d.BoothName}({d.BoothCode_No})")
            //                                    .ToList(),

            //                BoothLocationName = _context.PollingLocationMaster.Where(d => d.LocationMasterId == locationMasterId && d.Status == true).Select(d => $"{d.LocationName}-{d.LocationCode}").ToList(),



            //            };
            //            var AllocatedBoothNumbers = assembly.BoothMaster
            //                                .Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true)
            //                                .Select(d => $"{d.BoothCode_No}").ToList();
            //            if (AllocatedBoothNumbers.Count > 0)
            //            {
            //                report.AllocatedBoothNumbers = string.Join(",", AllocatedBoothNumbers);
            //                soReports.Add(report);
            //            }


            //        }
            //        return soReports;
            //    }
            //    else
            //    {

            //        // When Pass PC
            //        var assembly = _context.AssemblyMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.PCMasterId == boothReportModel.PCMasterId && d.AssemblyMasterId == boothReportModel.AssemblyMasterId && d.AssemblyStatus == true).Include(d => d.BoothMaster).FirstOrDefault();
            //        var sectorOfficerList = _context.SectorOfficerMaster.Where(d => d.StateMasterId == assembly.StateMasterId && d.SoAssemblyCode == assembly.AssemblyCode && d.SoStatus == true).ToList();
            //        var state = _context.StateMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.StateStatus == true).FirstOrDefault();
            //        var pcMaster = _context.ParliamentConstituencyMaster.Where(d => d.PCMasterId == boothReportModel.PCMasterId && d.PcStatus == true).FirstOrDefault();
            //        var pollingLocation = _context.PollingLocationMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId && d.AssemblyMasterId == boothReportModel.AssemblyMasterId && d.Status == true).Count();
            //        foreach (var so in sectorOfficerList)
            //        {
            //            var locationMasterId = _context.BoothMaster.Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true).Select(d => d.LocationMasterId).FirstOrDefault();
            //            SoReport report = new SoReport
            //            {
            //                SoMasterId = so.SOMasterId,
            //                Header = $"{state.StateName}({state.StateCode}),{pcMaster.PcName}({pcMaster.PcCodeNo}),{assembly.AssemblyName}({assembly.AssemblyCode})",
            //                Title = $"{pcMaster.PcName}",
            //                Type = "PC",
            //                TotalBoothCount = assembly.BoothMaster.Count(),
            //                TotalPollingLocationCount = pollingLocation,
            //                TotalSOAppointedCount = sectorOfficerList.Count(),
            //                SOName = so.SoName,
            //                SOMobileNo = so.SoMobile,
            //                Office = so.SoOfficeName,
            //                SODesignation = so.SoDesignation,
            //                BoothAllocatedCount = assembly.BoothMaster.Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true).Count(),
            //                BoothAllocatedName = assembly.BoothMaster
            //                                    .Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true)
            //                                    .Select(d => $"{d.BoothName}, {d.BoothCode_No}")
            //                                    .ToList(),

            //                BoothLocationName = _context.PollingLocationMaster.Where(d => d.LocationMasterId == locationMasterId).Select(d => $"{d.LocationName}-{d.LocationCode}").ToList(),



            //            };

            //            var AllocatedBoothNumbers = assembly.BoothMaster
            //                               .Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true)
            //                               .Select(d => $"{d.BoothCode_No}").ToList();
            //            if (AllocatedBoothNumbers.Count > 0)
            //            {
            //                report.AllocatedBoothNumbers = string.Join(",", AllocatedBoothNumbers);
            //                soReports.Add(report);
            //            }

            //        }
            //        return soReports;
            //    }


            //}
            //else
            //{
            //    return null;

            //}
            return null;
        }
        public async Task<List<SoReport>> GetPendingSOReport(BoothReportModel boothReportModel)
        {
            //List<SoReport> soReports = new List<SoReport>();
            //if (boothReportModel.StateMasterId != 0 && boothReportModel.DistrictMasterId == 0 && boothReportModel.PCMasterId == 0)
            //{
            //    var assemblyList = _context.AssemblyMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId).ToList();
            //    var districtList = _context.DistrictMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId).ToList();
            //    var state = _context.StateMaster.FirstOrDefault(d => d.StateMasterId == boothReportModel.StateMasterId);
            //    var assignedSectorOfficerIds = _context.BoothMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId).Select(b => b.AssignedTo).ToList();
            //    var getUnassignedSO = _context.SectorOfficerMaster
            //        .Where(som => som.StateMasterId == boothReportModel.StateMasterId && !assignedSectorOfficerIds.Contains(som.SOMasterId.ToString())).ToList();

            //    foreach (var so in getUnassignedSO)
            //    {
            //        // Assuming you have pollingLocation and sectorOfficerList defined somewhere
            //        var locationMasterId = _context.BoothMaster.Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true).Select(d => d.LocationMasterId).FirstOrDefault();
            //        var assembly = assemblyList.FirstOrDefault(d => d.AssemblyCode == so.SoAssemblyCode);
            //        var assemblyName = assembly?.AssemblyName ?? "Default Assembly Name";
            //        var assemblyCode = assembly?.AssemblyCode ?? 0;
            //        var districtName = districtList.FirstOrDefault(d => d.DistrictMasterId == (assembly?.DistrictMasterId))?.DistrictName ?? "Default District Name";
            //        var districtCode = districtList.FirstOrDefault(d => d.DistrictMasterId == (assembly?.DistrictMasterId))?.DistrictCode ?? "0";

            //        SoReport report = new SoReport
            //        {
            //            SoMasterId = so.SOMasterId,
            //            Header = $"{state.StateName}({state.StateCode}))",
            //            Title = $"{state.StateName}",
            //            Type = "State",
            //            //TotalBoothCount = assembly.Count(), // Assuming you want to count assemblies
            //            //TotalPollingLocationCount = pollingLocation, // Assuming pollingLocation is defined
            //            //                                             // TotalSOAppointedCount = sectorOfficerList.Count(), // Assuming sectorOfficerList is defined
            //            SOName = so.SoName,
            //            SOMobileNo = so.SoMobile,
            //            Office = so.SoOfficeName,
            //            SODesignation = so.SoDesignation,
            //            BoothAllocatedCount = _context.BoothMaster.Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true).Count(),
            //            BoothAllocatedName = _context.BoothMaster
            //                .Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true)
            //                .Select(d => $"{d.BoothName}({d.BoothCode_No})")
            //                .ToList(),

            //            BoothLocationName = _context.PollingLocationMaster.Where(d => d.LocationMasterId == locationMasterId && d.Status == true).Select(d => $"{d.LocationName}-{d.LocationCode}").ToList()
            //        };

            //        var AllocatedBoothNumbers = _context.BoothMaster
            //            .Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true)
            //            .Select(d => $"{d.BoothCode_No}").ToList();
            //        if (AllocatedBoothNumbers.Count > 0)
            //        {
            //            report.AllocatedBoothNumbers = string.Join(",", AllocatedBoothNumbers);
            //        }
            //        soReports.Add(report);
            //    }
            //    return soReports;
            //}

            //if (boothReportModel.AssemblyMasterId is not 0)
            //{
            //    List<int> boothNumbers = new List<int>();
            //    if (boothReportModel.DistrictMasterId is not 0)
            //    {
            //        var assembly = _context.AssemblyMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId && d.AssemblyMasterId == boothReportModel.AssemblyMasterId && d.AssemblyStatus == true).Include(d => d.BoothMaster).FirstOrDefault();
            //        var sectorOfficerList = _context.SectorOfficerMaster.Where(d => d.StateMasterId == assembly.StateMasterId && d.SoAssemblyCode == assembly.AssemblyCode && d.SoStatus == true).ToList();
            //        var state = _context.StateMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.StateStatus == true).FirstOrDefault();
            //        var district = _context.DistrictMaster.Where(d => d.DistrictMasterId == boothReportModel.DistrictMasterId && d.DistrictStatus == true).FirstOrDefault();
            //        var pollingLocation = _context.PollingLocationMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId && d.AssemblyMasterId == boothReportModel.AssemblyMasterId && d.Status == true).Count();
            //        foreach (var so in sectorOfficerList)
            //        {
            //            var locationMasterId = _context.BoothMaster.Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true).Select(d => d.LocationMasterId).FirstOrDefault();
            //            SoReport report = new SoReport
            //            {
            //                SoMasterId = so.SOMasterId,
            //                Header = $"{state.StateName}({state.StateCode}),{district.DistrictName}({district.DistrictCode}),{assembly.AssemblyName}({assembly.AssemblyCode})",
            //                Title = $"{district.DistrictName}",
            //                Type = "District",
            //                TotalBoothCount = assembly.BoothMaster.Count(),
            //                TotalPollingLocationCount = pollingLocation,
            //                TotalSOAppointedCount = sectorOfficerList.Count(),
            //                SOName = so.SoName,
            //                SOMobileNo = so.SoMobile,
            //                Office = so.SoOfficeName,
            //                SODesignation = so.SoDesignation,
            //                BoothAllocatedCount = assembly.BoothMaster.Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true).Count(),
            //                BoothAllocatedName = assembly.BoothMaster
            //                                    .Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true)
            //                                    .Select(d => $"{d.BoothName}({d.BoothCode_No})")
            //                                    .ToList(),

            //                BoothLocationName = _context.PollingLocationMaster.Where(d => d.LocationMasterId == locationMasterId && d.Status == true).Select(d => $"{d.LocationName}-{d.LocationCode}").ToList(),



            //            };
            //            var AllocatedBoothNumbers = assembly.BoothMaster
            //                                .Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true)
            //                                .Select(d => $"{d.BoothCode_No}").ToList();
            //            if (AllocatedBoothNumbers.Count < 0)
            //            {
            //                report.AllocatedBoothNumbers = string.Join(",", AllocatedBoothNumbers);
            //                soReports.Add(report);
            //            }


            //        }
            //        return soReports;
            //    }
            //    else
            //    {

            //        // When Pass PC
            //        var assembly = _context.AssemblyMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.PCMasterId == boothReportModel.PCMasterId && d.AssemblyMasterId == boothReportModel.AssemblyMasterId && d.AssemblyStatus == true).Include(d => d.BoothMaster).FirstOrDefault();
            //        var sectorOfficerList = _context.SectorOfficerMaster.Where(d => d.StateMasterId == assembly.StateMasterId && d.SoAssemblyCode == assembly.AssemblyCode && d.SoStatus == true).ToList();
            //        var state = _context.StateMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.StateStatus == true).FirstOrDefault();
            //        var pcMaster = _context.ParliamentConstituencyMaster.Where(d => d.PCMasterId == boothReportModel.PCMasterId && d.PcStatus == true).FirstOrDefault();
            //        var pollingLocation = _context.PollingLocationMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId && d.AssemblyMasterId == boothReportModel.AssemblyMasterId && d.Status == true).Count();
            //        foreach (var so in sectorOfficerList)
            //        {
            //            var locationMasterId = _context.BoothMaster.Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true).Select(d => d.LocationMasterId).FirstOrDefault();
            //            SoReport report = new SoReport
            //            {
            //                SoMasterId = so.SOMasterId,
            //                Header = $"{state.StateName}({state.StateCode}),{pcMaster.PcName}({pcMaster.PcCodeNo}),{assembly.AssemblyName}({assembly.AssemblyCode})",
            //                Title = $"{pcMaster.PcName}",
            //                Type = "PC",
            //                TotalBoothCount = assembly.BoothMaster.Count(),
            //                TotalPollingLocationCount = pollingLocation,
            //                TotalSOAppointedCount = sectorOfficerList.Count(),
            //                SOName = so.SoName,
            //                SOMobileNo = so.SoMobile,
            //                Office = so.SoOfficeName,
            //                SODesignation = so.SoDesignation,
            //                BoothAllocatedCount = assembly.BoothMaster.Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true).Count(),
            //                BoothAllocatedName = assembly.BoothMaster
            //                                    .Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true)
            //                                    .Select(d => $"{d.BoothName}, {d.BoothCode_No}")
            //                                    .ToList(),

            //                BoothLocationName = _context.PollingLocationMaster.Where(d => d.LocationMasterId == locationMasterId).Select(d => $"{d.LocationName}-{d.LocationCode}").ToList(),



            //            };

            //            var AllocatedBoothNumbers = assembly.BoothMaster
            //                               .Where(d => d.AssignedTo == so.SOMasterId.ToString() && d.BoothStatus == true)
            //                               .Select(d => $"{d.BoothCode_No}").ToList();
            //            if (AllocatedBoothNumbers.Count < 0)
            //            {
            //                report.AllocatedBoothNumbers = string.Join(",", AllocatedBoothNumbers);
            //                soReports.Add(report);
            //            }

            //        }
            //        return soReports;
            //    }


            //}
            //else
            //{
            //    return null;

            //}
            return null;
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

            //if (boothReportModel.DistrictMasterId is not 0)
            //{
            //    district = _context.DistrictMaster.Where(d => d.DistrictMasterId == boothReportModel.DistrictMasterId && d.StateMasterId == boothReportModel.StateMasterId && d.DistrictStatus == true).Select(d => new { d.DistrictName, d.DistrictCode }).FirstOrDefault();
            //}

            //if (boothReportModel.PCMasterId is not 0)
            //{
            //    pcMaster = _context.ParliamentConstituencyMaster.Where(d => d.PCMasterId == boothReportModel.PCMasterId && d.PcStatus == true).Select(d => new { d.PcName, d.PcCodeNo }).FirstOrDefault();
            //}

            ////State 
            //if (boothReportModel.StateMasterId is not 0 && boothReportModel.DistrictMasterId is 0 && boothReportModel.AssemblyMasterId is 0 && boothReportModel.PCMasterId is 0)
            //{

            //    var assemblyList = _context.AssemblyMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.AssemblyStatus == true).ToList();

            //    foreach (var assembly in assemblyList)
            //    {
            //        var pollingStationData = _context.PollingStationMaster.Include(psm => psm.PollingStationGender).Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId).ToList();
            //        VTPSReportReportModel report = new VTPSReportReportModel
            //        {
            //            // Populate your ConsolidateBoothReport properties here based on assembly data
            //            Header = $"{state.StateName}({state.StateCode})",
            //            Title = $"{state.StateName}",
            //            Type = "State",
            //            DistrictName = district.DistrictName,
            //            DistrictCode = district.DistrictCode,
            //            AssemblyName = assemblyList.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId).Select(p => p.AssemblyName).FirstOrDefault(),
            //            AssemblyCode = assemblyList.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId).Select(p => p.AssemblyCode.ToString()).FirstOrDefault(),

            //            //AssemblyCode= _context.PollingStationMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId && d.AssemblyMasterId == boothReportModel.AssemblyMasterId).Select(p=>p.AssemblySegmentNo).FirstOrDefault(),
            //            EPIC = pollingStationData.Sum(psm => psm.VotePolledEPIC),
            //            TotalEVMS = _context.PollingStationMaster.Where(d => d.AssemblyMasterId == boothReportModel.AssemblyMasterId && d.EVMReplaced).Count(),
            //            Male = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2).Sum(gender => gender.Male)),
            //            Female = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2).Sum(gender => gender.Female)),
            //            ThirdGender = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2).Sum(gender => gender.ThirdGender)),
            //            Total = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2).Sum(gender => gender.Total)),
            //            OverseasElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 4).Sum(gender => gender.Total)), //overseas
            //            YoungElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 3).Sum(gender => gender.Total)), // young electrl
            //            PWdEelectoral = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1).Sum(gender => gender.Total)), // no of electrl pwd
            //                                                                                                                                                                                        //VotePolledOtherDocument= pollingStationData.Sum(psm =>Convert.ToInt16(psm.VotePolledOtherDocument)),
            //            VotePolledOtherDocument = pollingStationData.Sum(psm =>
            //            {
            //                if (int.TryParse(psm.VotePolledOtherDocument, out int result))
            //                {
            //                    return result;
            //                }
            //                else
            //                {
            //                    return 0;
            //                }
            //            }),
            //            TenderedVotes = pollingStationData.Sum(psm => psm.TenderedVote),
            //        };
            //        consolidateBoothReports.Add(report);
            //    }
            //    return consolidateBoothReports;
            //}

            ////District
            //else if (boothReportModel.StateMasterId is not 0 && boothReportModel.DistrictMasterId is not 0 && boothReportModel.AssemblyMasterId is 0 && boothReportModel.PCMasterId is 0)
            //{
            //    var assemblyList = _context.AssemblyMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId && d.AssemblyStatus == true).ToList();
            //    foreach (var assembly in assemblyList)
            //    {
            //        var pollingStationData = _context.PollingStationMaster.Include(psm => psm.PollingStationGender).Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId
            //         && d.AssemblyMasterId == assembly.AssemblyMasterId)
            //.ToList();
            //        VTPSReportReportModel report = new VTPSReportReportModel
            //        {
            //            // Populate your ConsolidateBoothReport properties here based on assembly data
            //            Header = $"{state.StateName}({state.StateCode})",
            //            Title = $"{state.StateName}",
            //            Type = "District",
            //            DistrictName = district.DistrictName,
            //            DistrictCode = district.DistrictCode,
            //            AssemblyName = assemblyList.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId).Select(p => p.AssemblyName).FirstOrDefault(),
            //            AssemblyCode = assemblyList.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId).Select(p => p.AssemblyCode.ToString()).FirstOrDefault(),

            //            //AssemblyCode= _context.PollingStationMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId && d.AssemblyMasterId == boothReportModel.AssemblyMasterId).Select(p=>p.AssemblySegmentNo).FirstOrDefault(),
            //            EPIC = pollingStationData.Sum(psm => psm.VotePolledEPIC),
            //            TotalEVMS = _context.PollingStationMaster.Where(d => d.AssemblyMasterId == boothReportModel.AssemblyMasterId && d.EVMReplaced).Count(),
            //            Male = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2).Sum(gender => gender.Male)),
            //            Female = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2).Sum(gender => gender.Female)),
            //            ThirdGender = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2).Sum(gender => gender.ThirdGender)),
            //            Total = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2).Sum(gender => gender.Total)),
            //            OverseasElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 4).Sum(gender => gender.Total)), //overseas
            //            YoungElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 3).Sum(gender => gender.Total)), // young electrl
            //            PWdEelectoral = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1).Sum(gender => gender.Total)), // no of electrl pwd
            //                                                                                                                                                                                        //VotePolledOtherDocument= pollingStationData.Sum(psm =>Convert.ToInt16(psm.VotePolledOtherDocument)),
            //            VotePolledOtherDocument = pollingStationData.Sum(psm =>
            //            {
            //                if (int.TryParse(psm.VotePolledOtherDocument, out int result))
            //                {
            //                    return result;
            //                }
            //                else
            //                {
            //                    // Handle non-numeric values, for example, you can log or set a default value
            //                    // In this case, I'll set it to 0, but you can adjust based on your requirements
            //                    return 0;
            //                }
            //            }),
            //            TenderedVotes = pollingStationData.Sum(psm => psm.TenderedVote),

            //        };
            //        consolidateBoothReports.Add(report);
            //    }
            //    return consolidateBoothReports;
            //}

            ////PC
            //else if (boothReportModel.StateMasterId is not 0 && boothReportModel.DistrictMasterId is 0 && boothReportModel.PCMasterId is not 0 && boothReportModel.AssemblyMasterId is 0)
            //{
            //    var assemblyList = _context.AssemblyMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.PCMasterId == boothReportModel.PCMasterId && d.AssemblyStatus == true).ToList();
            //    foreach (var assembly in assemblyList)
            //    {
            //        var pollingStationData = await _context.PollingStationMaster
            //.Include(psm => psm.PollingStationGender)
            //.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId).ToListAsync();
            //        VTPSReportReportModel report = new VTPSReportReportModel
            //        {
            //            // Populate your ConsolidateBoothReport properties here based on assembly data
            //            Header = $"{state.StateName}({state.StateCode}),{pcMaster.PcName}({pcMaster.PcCodeNo})",
            //            Title = $"{pcMaster.PcName}",
            //            Type = "PC",
            //            Code = assembly.AssemblyCode.ToString(),
            //            Name = assembly.AssemblyName,
            //            PCCode = pcMaster.PcCodeNo,
            //            PCName = pcMaster.PcName,
            //            AssemblyName = assemblyList.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.PCMasterId == boothReportModel.PCMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId).Select(p => p.AssemblyName).FirstOrDefault(),
            //            AssemblyCode = assemblyList.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.PCMasterId == boothReportModel.PCMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId).Select(p => p.AssemblyCode.ToString()).FirstOrDefault(),

            //            EPIC = pollingStationData.Sum(psm => psm.VotePolledEPIC),
            //            TotalEVMS = _context.PollingStationMaster.Where(d => d.AssemblyMasterId == boothReportModel.AssemblyMasterId && d.EVMReplaced).Count(),
            //            Male = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2).Sum(gender => gender.Male)),
            //            Female = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2).Sum(gender => gender.Female)),
            //            ThirdGender = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2).Sum(gender => gender.ThirdGender)),
            //            Total = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2).Sum(gender => gender.Total)),
            //            OverseasElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 4).Sum(gender => gender.Total)), //overseas
            //            YoungElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 3).Sum(gender => gender.Total)), // young electrl
            //            PWdEelectoral = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1).Sum(gender => gender.Total)), // no of electrl pwd
            //            VotePolledOtherDocument = pollingStationData.Sum(psm =>
            //            {
            //                if (int.TryParse(psm.VotePolledOtherDocument, out int result))
            //                {
            //                    return result;
            //                }
            //                else
            //                {
            //                    // Handle non-numeric values, for example, you can log or set a default value
            //                    // In this case, I'll set it to 0, but you can adjust based on your requirements
            //                    return 0;
            //                }
            //            }),
            //            TenderedVotes = pollingStationData.Sum(psm => psm.TenderedVote),

            //        };
            //        consolidateBoothReports.Add(report);
            //    }
            //    return consolidateBoothReports;
            //}

            ////Assembly
            //else if (boothReportModel.AssemblyMasterId is not 0)
            //{

            //    if (boothReportModel.DistrictMasterId is not 0)
            //    {
            //        var assemblyList = _context.AssemblyMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId && d.AssemblyStatus == true).ToList();
            //        foreach (var assembly in assemblyList)
            //        {
            //            var pollingStationData = _context.PollingStationMaster.Include(psm => psm.PollingStationGender).Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId
            //             && d.AssemblyMasterId == assembly.AssemblyMasterId)
            //    .ToList();
            //            VTPSReportReportModel report = new VTPSReportReportModel
            //            {
            //                Header = $"{state.StateName}({state.StateCode}),{district.DistrictName}({district.DistrictCode}),{assembly.AssemblyName}({assembly.AssemblyCode})",
            //                Title = $"{assembly.AssemblyName}",
            //                Type = "Assembly",
            //                DistrictName = district.DistrictName,
            //                DistrictCode = district.DistrictCode,
            //                AssemblyName = assemblyList.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId).Select(p => p.AssemblyName).FirstOrDefault(),
            //                AssemblyCode = assemblyList.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId).Select(p => p.AssemblyCode.ToString()).FirstOrDefault(),

            //                //AssemblyCode= _context.PollingStationMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictMasterId == boothReportModel.DistrictMasterId && d.AssemblyMasterId == boothReportModel.AssemblyMasterId).Select(p=>p.AssemblySegmentNo).FirstOrDefault(),
            //                EPIC = pollingStationData.Sum(psm => psm.VotePolledEPIC),
            //                TotalEVMS = _context.PollingStationMaster.Where(d => d.AssemblyMasterId == boothReportModel.AssemblyMasterId && d.EVMReplaced).Count(),
            //                Male = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2).Sum(gender => gender.Male)),
            //                Female = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2).Sum(gender => gender.Female)),
            //                ThirdGender = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2).Sum(gender => gender.ThirdGender)),
            //                Total = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2).Sum(gender => gender.Total)),
            //                OverseasElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 4).Sum(gender => gender.Total)), //overseas
            //                YoungElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 3).Sum(gender => gender.Total)), // young electrl
            //                PWdEelectoral = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1).Sum(gender => gender.Total)), // no of electrl pwd
            //                                                                                                                                                                                            //VotePolledOtherDocument= pollingStationData.Sum(psm =>Convert.ToInt16(psm.VotePolledOtherDocument)),
            //                VotePolledOtherDocument = pollingStationData.Sum(psm =>
            //                {
            //                    if (int.TryParse(psm.VotePolledOtherDocument, out int result))
            //                    {
            //                        return result;
            //                    }
            //                    else
            //                    {

            //                        return 0;
            //                    }
            //                }),
            //                TenderedVotes = pollingStationData.Sum(psm => psm.TenderedVote),

            //            };
            //            consolidateBoothReports.Add(report);
            //        }
            //        return consolidateBoothReports;
            //    }
            //    else
            //    {
            //        var assemblyList = _context.AssemblyMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.PCMasterId == boothReportModel.PCMasterId && d.AssemblyStatus == true).ToList();
            //        foreach (var assembly in assemblyList)
            //        {
            //            var pollingStationData = await _context.PollingStationMaster
            //    .Include(psm => psm.PollingStationGender)
            //    .Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId)
            //    .ToListAsync();
            //            VTPSReportReportModel report = new VTPSReportReportModel
            //            {
            //                // Populate your ConsolidateBoothReport properties here based on assembly data

            //                Header = $"{state.StateName}({state.StateCode}),{pcMaster.PcName}({pcMaster.PcCodeNo}),{assembly.AssemblyName}({assembly.AssemblyCode})",
            //                Title = $"{assembly.AssemblyName}",
            //                Type = "Assembly",
            //                Code = assembly.AssemblyCode.ToString(),
            //                Name = assembly.AssemblyName,
            //                PCCode = pcMaster.PcCodeNo,
            //                PCName = pcMaster.PcName,
            //                AssemblyName = assemblyList.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.PCMasterId == boothReportModel.PCMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId).Select(p => p.AssemblyName).FirstOrDefault(),
            //                AssemblyCode = assemblyList.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.PCMasterId == boothReportModel.PCMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId).Select(p => p.AssemblyCode.ToString()).FirstOrDefault(),

            //                EPIC = pollingStationData.Sum(psm => psm.VotePolledEPIC),
            //                TotalEVMS = _context.PollingStationMaster.Where(d => d.AssemblyMasterId == boothReportModel.AssemblyMasterId && d.EVMReplaced).Count(),
            //                Male = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2).Sum(gender => gender.Male)),
            //                Female = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2).Sum(gender => gender.Female)),
            //                ThirdGender = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2).Sum(gender => gender.ThirdGender)),
            //                Total = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2).Sum(gender => gender.Total)),
            //                OverseasElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 4).Sum(gender => gender.Total)), //overseas
            //                YoungElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 3).Sum(gender => gender.Total)), // young electrl
            //                PWdEelectoral = pollingStationData.Where(psm => psm.PollingStationGender != null).Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1).Sum(gender => gender.Total)), // no of electrl pwd
            //                VotePolledOtherDocument = pollingStationData.Sum(psm =>
            //                {
            //                    if (int.TryParse(psm.VotePolledOtherDocument, out int result))
            //                    {
            //                        return result;
            //                    }
            //                    else
            //                    {

            //                        return 0;
            //                    }
            //                }),
            //                TenderedVotes = pollingStationData.Sum(psm => psm.TenderedVote),

            //            };
            //            consolidateBoothReports.Add(report);
            //        }
            //        return consolidateBoothReports;
            //    }


            //}



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
                 .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId) && d.IsFinalVote == true)
                .SumAsync(d => d.FinalVote);

            // Calculate total EDC votes
            var eDCVoteCount = await _context.ElectionInfoMaster
               .Where(d => d.IsFinalVote == true && d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId) && d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId))
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

            //if (boothReportModel.PCMasterId is not 0)
            //{
            //    pcMaster = _context.ParliamentConstituencyMaster.Where(d => d.PCMasterId == boothReportModel.PCMasterId && d.PcStatus == true).Select(d => new { d.PcName, d.PcCodeNo }).FirstOrDefault();
            //}

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
            //          else if (boothReportModel.StateMasterId is not 0 && boothReportModel.Type == "PCACWise")

            //          {
            //              if (boothReportModel.PCMasterId is not 0 && boothReportModel.AssemblyMasterId is 0)
            //              {
            //                  var pcList = _context.ParliamentConstituencyMaster
            //   .Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.PcStatus == true && d.PCMasterId == boothReportModel.PCMasterId)
            //   .AsEnumerable().FirstOrDefault();

            //                  int assemblyCount = 0; List<VTReportModel> assemblylistReport = new List<VTReportModel>();
            //                  var assemblyList = _context.AssemblyMaster
            //                        .Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.PCMasterId == boothReportModel.PCMasterId && d.AssemblyStatus == true)
            //                        .OrderBy(d => d.AssemblyCode)
            //                        .ToList();
            //                  if (assemblyList.Count > 0)
            //                  {
            //                      foreach (var assembly in assemblyList)
            //                      {
            //                          VTReportModel report = new VTReportModel();
            //                          report.Header = $"{state.StateName}({state.StateCode})";
            //                          report.Title = $"{state.StateName}";
            //                          report.Type = "PCACWise";
            //                          report.PCCode = pcList.PcCodeNo;
            //                          report.PCName = pcList.PcName;
            //                          report.AssemblyName = assemblyList.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId)
            //                                                             .Select(p => p.AssemblyName).FirstOrDefault();
            //                          report.AssemblyCode = assemblyList.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId)
            //                                                             .Select(p => p.AssemblyCode.ToString()).FirstOrDefault();
            //                          var pollingStationData = _context.PollingStationMaster.Include(psm => psm.PollingStationGender).Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId).ToList();
            //                          if (pollingStationData != null && pollingStationData.Count > 0)
            //                          {
            //                              //type 1: Electorals
            //                              report.MaleElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
            //                                                                        .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
            //                                                                        .Sum(gender => gender.Male));
            //                              report.FemaleElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
            //                                                                          .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
            //                                                                          .Sum(gender => gender.Female));
            //                              report.ThirdGenderElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
            //                                                                              .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
            //                                                                              .Sum(gender => gender.ThirdGender));
            //                              report.TotalElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
            //                                                                         .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
            //                                                                         .Sum(gender => gender.Total));

            //                              //type 2: Votes polled
            //                              report.MaleVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
            //                                                                     .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
            //                                                                     .Sum(gender => gender.Male));
            //                              report.FemaleVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
            //                                                                       .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
            //                                                                       .Sum(gender => gender.Female));
            //                              report.ThirdGenderVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
            //                                                                           .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
            //                                                                           .Sum(gender => gender.ThirdGender));
            //                              report.TotalVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
            //                                                                      .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
            //                                                                      .Sum(gender => gender.Total));

            //                              report.EPIC = pollingStationData.Sum(psm => psm.VotePolledEPIC);
            //                              report.VotePolledOtherDocument = pollingStationData.Sum(psm =>
            //                              {
            //                                  if (int.TryParse(psm.VotePolledOtherDocument, out int result))
            //                                  {
            //                                      return result;
            //                                  }
            //                                  else
            //                                  {
            //                                      return 0;
            //                                  }
            //                              });
            //                              report.MalePercentage = report.MaleVoters == 0 ? 0 : (double)report.MaleVoters / (double)report.MaleElectoral * 100;
            //                              if (double.IsInfinity(report.MalePercentage))
            //                              {
            //                                  report.MalePercentage = 0;
            //                              }

            //                              report.FemalePercentage = report.FemaleVoters == 0 ? 0 : (double)report.FemaleVoters / (double)report.FemaleElectoral * 100;
            //                              if (double.IsInfinity(report.FemalePercentage))
            //                              {
            //                                  report.FemalePercentage = 0;
            //                              }
            //                              report.ThirdGenderPercentage = report.ThirdGenderVoters == 0 ? 0 : (double)report.ThirdGenderVoters / (double)report.ThirdGenderElectoral * 100;
            //                              if (double.IsInfinity(report.ThirdGenderPercentage))
            //                              {
            //                                  report.ThirdGenderPercentage = 0;
            //                              }
            //                              report.TotalPercentage = report.TotalVoters == 0 ? 0 : (double)report.TotalVoters / (double)report.TotalElectoral * 100;
            //                              if (double.IsInfinity(report.TotalPercentage))
            //                              {
            //                                  report.TotalPercentage = 0;
            //                              }
            //                              consolidateBoothReports.Add(report); assemblylistReport.Add(report);
            //                              assemblyCount++;


            //                          }
            //                          else
            //                          {  //type 1: Electorals
            //                              report.MaleElectoral = 0;
            //                              report.FemaleElectoral = 0;
            //                              report.ThirdGenderElectoral = 0;
            //                              report.TotalElectoral = 0;

            //                              //type 2: Votes polled
            //                              report.MaleVoters = 0;
            //                              report.FemaleVoters = 0;
            //                              report.ThirdGenderVoters = 0;
            //                              report.TotalVoters = 0;
            //                              report.EPIC = 0;
            //                              report.VotePolledOtherDocument = 0;
            //                              report.MalePercentage = 0;
            //                              report.FemalePercentage = 0;
            //                              report.ThirdGenderPercentage = 0;
            //                              report.TotalPercentage = 0;

            //                              consolidateBoothReports.Add(report); assemblylistReport.Add(report);
            //                              assemblyCount++;


            //                          }


            //                      }

            //                  }
            //                  else
            //                  {
            //                      VTReportModel report = new VTReportModel();
            //                      report.Header = $"{state.StateName}({state.StateCode})";
            //                      report.Title = $"{state.StateName}";
            //                      report.Type = "PCACWise";
            //                      report.PCCode = pcList.PcCodeNo;
            //                      report.PCName = pcList.PcName;
            //                      report.AssemblyName = "N/A";
            //                      report.AssemblyCode = "N/A";
            //                      report.MaleElectoral = 0;
            //                      report.FemaleElectoral = 0;
            //                      report.ThirdGenderElectoral = 0;
            //                      report.TotalElectoral = 0;

            //                      //type 2: Votes polled
            //                      report.MaleVoters = 0;
            //                      report.FemaleVoters = 0;
            //                      report.ThirdGenderVoters = 0;
            //                      report.TotalVoters = 0;
            //                      report.EPIC = 0;
            //                      report.VotePolledOtherDocument = 0;
            //                      report.MalePercentage = 0;
            //                      report.FemalePercentage = 0;
            //                      report.ThirdGenderPercentage = 0;
            //                      report.TotalPercentage = 0;
            //                      consolidateBoothReports.Add(report);

            //                  }
            //                  if (assemblyList.Count == assemblyCount)
            //                  {
            //                      // add Grand Total Row

            //                      VTReportModel reportTotal = new VTReportModel();
            //                      reportTotal.Header = "";
            //                      reportTotal.Title = "";
            //                      reportTotal.Type = "";
            //                      reportTotal.DistrictName = "";
            //                      reportTotal.DistrictCode = "";
            //                      reportTotal.AssemblyName = "Total";
            //                      reportTotal.AssemblyCode = "";
            //                      reportTotal.MaleElectoral = assemblylistReport.Sum(report => report.MaleElectoral);
            //                      reportTotal.FemaleElectoral = assemblylistReport.Sum(report => report.FemaleElectoral);
            //                      reportTotal.ThirdGenderElectoral = assemblylistReport.Sum(report => report.ThirdGenderElectoral);
            //                      reportTotal.TotalElectoral = assemblylistReport.Sum(report => report.TotalElectoral);
            //                      reportTotal.MaleVoters = assemblylistReport.Sum(report => report.MaleVoters);
            //                      reportTotal.FemaleVoters = assemblylistReport.Sum(report => report.FemaleVoters);
            //                      reportTotal.ThirdGenderVoters = assemblylistReport.Sum(report => report.ThirdGenderVoters);
            //                      reportTotal.TotalVoters = assemblylistReport.Sum(report => report.TotalVoters);
            //                      reportTotal.EPIC = assemblylistReport.Sum(report => report.EPIC);
            //                      reportTotal.VotePolledOtherDocument = assemblylistReport.Sum(report => report.VotePolledOtherDocument);
            //                      //   reportGrandTotal.MalePercentage = (double)reportGrandTotal.MaleVoters / (double)reportGrandTotal.MaleElectoral * 100;
            //                      reportTotal.MalePercentage = (reportTotal.MaleVoters != 0 && reportTotal.MaleElectoral != 0)
            //  ? ((double)reportTotal.MaleVoters / (double)reportTotal.MaleElectoral) * 100
            //  : 0;

            //                      //reportGrandTotal.FemalePercentage = (double)reportGrandTotal.FemaleVoters / (double)reportGrandTotal.FemaleElectoral * 100;
            //                      reportTotal.FemalePercentage = (reportTotal.FemaleVoters != 0 && reportTotal.FemaleElectoral != 0)
            //       ? ((double)reportTotal.FemaleVoters / (double)reportTotal.FemaleElectoral) * 100 : 0;

            //                      reportTotal.ThirdGenderPercentage = (reportTotal.ThirdGenderVoters != 0 && reportTotal.ThirdGenderElectoral != 0)
            //      ? ((double)reportTotal.ThirdGenderVoters / (double)reportTotal.ThirdGenderElectoral) * 100
            //      : 0;

            //                      //reportGrandTotal.TotalPercentage = (double)reportGrandTotal.TotalVoters / (double)reportGrandTotal.TotalElectoral * 100;


            //                      reportTotal.TotalPercentage = (reportTotal.TotalVoters != 0 && reportTotal.TotalElectoral != 0)
            //     ? ((double)reportTotal.TotalVoters / (double)reportTotal.TotalElectoral) * 100
            //     : 0;
            //                      assemblylistTotal.Add(reportTotal);
            //                      consolidateBoothReports.Add(reportTotal);

            //                  }


            //                  return consolidateBoothReports;
            //              }
            //              else if (boothReportModel.PCMasterId is not 0 && boothReportModel.AssemblyMasterId is not 0)
            //              {
            //                  var pcList = _context.ParliamentConstituencyMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.PcStatus == true && d.PCMasterId == boothReportModel.PCMasterId).AsEnumerable().FirstOrDefault();

            //                  var assemblyList = _context.AssemblyMaster
            //                      .Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.PCMasterId == boothReportModel.PCMasterId && d.AssemblyStatus == true && d.AssemblyMasterId == boothReportModel.AssemblyMasterId)
            //                      .OrderBy(d => d.AssemblyCode)
            //                      .FirstOrDefault();

            //                  var boothList = _context.BoothMaster.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.BoothStatus && d.AssemblyMasterId == boothReportModel.AssemblyMasterId && d.AssemblyMasterId == assemblyList.AssemblyMasterId).ToList();

            //                  if (boothList.Count > 0)
            //                  {
            //                      foreach (var booth in boothList)
            //                      {
            //                          VTReportModel report = new VTReportModel();
            //                          report.Header = $"{state.StateName}({state.StateCode})";
            //                          report.Title = $"{state.StateName},({pcList.PcName})";
            //                          report.Type = "PCACWise";
            //                          report.PCCode = pcList.PcCodeNo;
            //                          report.PCName = pcList.PcName;
            //                          report.AssemblyName = assemblyList.AssemblyName;
            //                          report.AssemblyCode = assemblyList.AssemblyCode.ToString();
            //                          var BoothRecord = _context.BoothMaster.Where(d => d.BoothMasterId == booth.BoothMasterId).FirstOrDefault();
            //                          report.BoothName = BoothRecord.BoothName + " " + BoothRecord.BoothCode_No.ToString();

            //                          var pollingStationData = _context.PollingStationMaster.Include(psm => psm.PollingStationGender).Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.PCasterId == boothReportModel.PCMasterId && d.BoothMasterId == booth.BoothMasterId).ToList();
            //                          if (pollingStationData != null && pollingStationData.Count > 0)
            //                          {
            //                              //type 1: Electorals

            //                              report.MaleElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
            //                                                                        .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
            //                                                                        .Sum(gender => gender.Male));
            //                              report.FemaleElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
            //                                                                          .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
            //                                                                          .Sum(gender => gender.Female));
            //                              report.ThirdGenderElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
            //                                                                              .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
            //                                                                              .Sum(gender => gender.ThirdGender));
            //                              report.TotalElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
            //                                                                         .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
            //                                                                         .Sum(gender => gender.Total));

            //                              //type 2: Votes polled
            //                              report.MaleVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
            //                                                                     .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
            //                                                                     .Sum(gender => gender.Male));
            //                              report.FemaleVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
            //                                                                       .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
            //                                                                       .Sum(gender => gender.Female));
            //                              report.ThirdGenderVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
            //                                                                           .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
            //                                                                           .Sum(gender => gender.ThirdGender));
            //                              report.TotalVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
            //                                                                      .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
            //                                                                      .Sum(gender => gender.Total));

            //                              report.EPIC = pollingStationData.Sum(psm => psm.VotePolledEPIC);
            //                              report.VotePolledOtherDocument = pollingStationData.Sum(psm =>
            //                              {
            //                                  if (int.TryParse(psm.VotePolledOtherDocument, out int result))
            //                                  {
            //                                      return result;
            //                                  }
            //                                  else
            //                                  {
            //                                      return 0;
            //                                  }
            //                              });
            //                              report.MalePercentage = report.MaleVoters == 0 ? 0 : (double)report.MaleVoters / (double)report.MaleElectoral * 100;
            //                              if (double.IsInfinity(report.MalePercentage))
            //                              {
            //                                  report.MalePercentage = 0;
            //                              }

            //                              report.FemalePercentage = report.FemaleVoters == 0 ? 0 : (double)report.FemaleVoters / (double)report.FemaleElectoral * 100;
            //                              if (double.IsInfinity(report.FemalePercentage))
            //                              {
            //                                  report.FemalePercentage = 0;
            //                              }
            //                              report.ThirdGenderPercentage = report.ThirdGenderVoters == 0 ? 0 : (double)report.ThirdGenderVoters / (double)report.ThirdGenderElectoral * 100;
            //                              if (double.IsInfinity(report.ThirdGenderPercentage))
            //                              {
            //                                  report.ThirdGenderPercentage = 0;
            //                              }
            //                              report.TotalPercentage = report.TotalVoters == 0 ? 0 : (double)report.TotalVoters / (double)report.TotalElectoral * 100;
            //                              if (double.IsInfinity(report.TotalPercentage))
            //                              {
            //                                  report.TotalPercentage = 0;
            //                              }
            //                              consolidateBoothReports.Add(report);



            //                          }
            //                          else
            //                          {  //type 1: Electorals
            //                              report.MaleElectoral = 0;
            //                              report.FemaleElectoral = 0;
            //                              report.ThirdGenderElectoral = 0;
            //                              report.TotalElectoral = 0;

            //                              //type 2: Votes polled
            //                              report.MaleVoters = 0;
            //                              report.FemaleVoters = 0;
            //                              report.ThirdGenderVoters = 0;
            //                              report.TotalVoters = 0;
            //                              report.EPIC = 0;
            //                              report.VotePolledOtherDocument = 0;
            //                              report.MalePercentage = 0;
            //                              report.FemalePercentage = 0;
            //                              report.ThirdGenderPercentage = 0;
            //                              report.TotalPercentage = 0;
            //                              consolidateBoothReports.Add(report);



            //                          }

            //                      }

            //                  }
            //                  else

            //                  {
            //                      VTReportModel report = new VTReportModel();
            //                      report.Header = $"{state.StateName}({state.StateCode})";
            //                      report.Title = $"{state.StateName},({pcList.PcName})";
            //                      report.Type = "PCACWise";
            //                      report.AssemblyName = "N/A";
            //                      report.AssemblyCode = "N/A";
            //                      report.MaleElectoral = 0;
            //                      report.FemaleElectoral = 0;
            //                      report.ThirdGenderElectoral = 0;
            //                      report.TotalElectoral = 0;

            //                      //type 2: Votes polled
            //                      report.MaleVoters = 0;
            //                      report.FemaleVoters = 0;
            //                      report.ThirdGenderVoters = 0;
            //                      report.TotalVoters = 0;
            //                      report.EPIC = 0;
            //                      report.VotePolledOtherDocument = 0;
            //                      report.MalePercentage = 0;
            //                      report.FemalePercentage = 0;
            //                      report.ThirdGenderPercentage = 0;
            //                      report.TotalPercentage = 0;
            //                      consolidateBoothReports.Add(report);
            //                  }
            //                  // add Grand Total Row

            //                  VTReportModel reportTotal = new VTReportModel();
            //                  reportTotal.Header = "";
            //                  reportTotal.Title = "";
            //                  reportTotal.Type = "";
            //                  reportTotal.DistrictName = "";
            //                  reportTotal.DistrictCode = "";
            //                  reportTotal.AssemblyName = "Total";
            //                  reportTotal.AssemblyCode = "";
            //                  reportTotal.MaleElectoral = consolidateBoothReports.Sum(report => report.MaleElectoral);
            //                  reportTotal.FemaleElectoral = consolidateBoothReports.Sum(report => report.FemaleElectoral);
            //                  reportTotal.ThirdGenderElectoral = consolidateBoothReports.Sum(report => report.ThirdGenderElectoral);
            //                  reportTotal.TotalElectoral = consolidateBoothReports.Sum(report => report.TotalElectoral);
            //                  reportTotal.MaleVoters = consolidateBoothReports.Sum(report => report.MaleVoters);
            //                  reportTotal.FemaleVoters = consolidateBoothReports.Sum(report => report.FemaleVoters);
            //                  reportTotal.ThirdGenderVoters = consolidateBoothReports.Sum(report => report.ThirdGenderVoters);
            //                  reportTotal.TotalVoters = consolidateBoothReports.Sum(report => report.TotalVoters);
            //                  reportTotal.EPIC = consolidateBoothReports.Sum(report => report.EPIC);
            //                  reportTotal.VotePolledOtherDocument = consolidateBoothReports.Sum(report => report.VotePolledOtherDocument);


            //                  reportTotal.MalePercentage = (reportTotal.MaleVoters != 0 && reportTotal.MaleElectoral != 0)
            //? ((double)reportTotal.MaleVoters / (double)reportTotal.MaleElectoral) * 100
            //: 0;

            //                  //reportGrandTotal.FemalePercentage = (double)reportGrandTotal.FemaleVoters / (double)reportGrandTotal.FemaleElectoral * 100;
            //                  reportTotal.FemalePercentage = (reportTotal.FemaleVoters != 0 && reportTotal.FemaleElectoral != 0)
            //   ? ((double)reportTotal.FemaleVoters / (double)reportTotal.FemaleElectoral) * 100 : 0;

            //                  reportTotal.ThirdGenderPercentage = (reportTotal.ThirdGenderVoters != 0 && reportTotal.ThirdGenderElectoral != 0)
            //  ? ((double)reportTotal.ThirdGenderVoters / (double)reportTotal.ThirdGenderElectoral) * 100
            //  : 0;

            //                  //reportGrandTotal.TotalPercentage = (double)reportGrandTotal.TotalVoters / (double)reportGrandTotal.TotalElectoral * 100;


            //                  reportTotal.TotalPercentage = (reportTotal.TotalVoters != 0 && reportTotal.TotalElectoral != 0)
            // ? ((double)reportTotal.TotalVoters / (double)reportTotal.TotalElectoral) * 100
            // : 0;



            //                  consolidateBoothReports.Add(reportTotal);



            //                  return consolidateBoothReports;



            //              }
            //              else

            //              {

            //                  var pcList = _context.ParliamentConstituencyMaster
            //  .Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.PcStatus == true)
            //  .AsEnumerable() // Switch to client-side evaluation
            //  .OrderBy(p => int.Parse(p.PcCodeNo))
            //  .ToList();


            //                  foreach (var pc in pcList)
            //                  {
            //                      int assemblyCount = 0; List<VTReportModel> assemblylistReport = new List<VTReportModel>();
            //                      var assemblyList = _context.AssemblyMaster
            //                         .Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.PCMasterId == pc.PCMasterId && d.AssemblyStatus == true)
            //                         .OrderBy(d => d.AssemblyCode)
            //                         .ToList();

            //                      if (assemblyList.Count > 0)
            //                      {
            //                          foreach (var assembly in assemblyList)
            //                          {
            //                              VTReportModel report = new VTReportModel();
            //                              report.Header = $"{state.StateName}({state.StateCode}),{pc.PcName}({pc.PcCodeNo})";
            //                              report.Title = $"{pc.PcName}";
            //                              report.Type = "PCACWise";
            //                              report.PCCode = pc.PcCodeNo;
            //                              report.PCName = pc.PcName;
            //                              report.AssemblyName = assemblyList.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId)
            //                                                                 .Select(p => p.AssemblyName).FirstOrDefault();
            //                              report.AssemblyCode = assemblyList.Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId)
            //                                                                 .Select(p => p.AssemblyCode.ToString()).FirstOrDefault();
            //                              var pollingStationData = _context.PollingStationMaster.Include(psm => psm.PollingStationGender).Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.AssemblyMasterId == assembly.AssemblyMasterId).ToList();
            //                              if (pollingStationData != null && pollingStationData.Count > 0)
            //                              {
            //                                  //type 1: Electorals
            //                                  report.MaleElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
            //                                                                            .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
            //                                                                            .Sum(gender => gender.Male));
            //                                  report.FemaleElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
            //                                                                              .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
            //                                                                              .Sum(gender => gender.Female));
            //                                  report.ThirdGenderElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
            //                                                                                  .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
            //                                                                                  .Sum(gender => gender.ThirdGender));
            //                                  report.TotalElectoral = pollingStationData.Where(psm => psm.PollingStationGender != null)
            //                                                                             .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 1)
            //                                                                             .Sum(gender => gender.Total));

            //                                  //type 2: Votes polled
            //                                  report.MaleVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
            //                                                                         .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
            //                                                                         .Sum(gender => gender.Male));
            //                                  report.FemaleVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
            //                                                                           .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
            //                                                                           .Sum(gender => gender.Female));
            //                                  report.ThirdGenderVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
            //                                                                               .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
            //                                                                               .Sum(gender => gender.ThirdGender));
            //                                  report.TotalVoters = pollingStationData.Where(psm => psm.PollingStationGender != null)
            //                                                                          .Sum(psm => psm.PollingStationGender.Where(p => p.Type == 2)
            //                                                                          .Sum(gender => gender.Total));

            //                                  report.EPIC = pollingStationData.Sum(psm => psm.VotePolledEPIC);
            //                                  report.VotePolledOtherDocument = pollingStationData.Sum(psm =>
            //                                  {
            //                                      if (int.TryParse(psm.VotePolledOtherDocument, out int result))
            //                                      {
            //                                          return result;
            //                                      }
            //                                      else
            //                                      {
            //                                          return 0;
            //                                      }
            //                                  });
            //                                  report.MalePercentage = report.MaleVoters == 0 ? 0 : (double)report.MaleVoters / (double)report.MaleElectoral * 100;
            //                                  if (double.IsInfinity(report.MalePercentage))
            //                                  {
            //                                      report.MalePercentage = 0;
            //                                  }

            //                                  report.FemalePercentage = report.FemaleVoters == 0 ? 0 : (double)report.FemaleVoters / (double)report.FemaleElectoral * 100;
            //                                  if (double.IsInfinity(report.FemalePercentage))
            //                                  {
            //                                      report.FemalePercentage = 0;
            //                                  }
            //                                  report.ThirdGenderPercentage = report.ThirdGenderVoters == 0 ? 0 : (double)report.ThirdGenderVoters / (double)report.ThirdGenderElectoral * 100;
            //                                  if (double.IsInfinity(report.ThirdGenderPercentage))
            //                                  {
            //                                      report.ThirdGenderPercentage = 0;
            //                                  }
            //                                  report.TotalPercentage = report.TotalVoters == 0 ? 0 : (double)report.TotalVoters / (double)report.TotalElectoral * 100;
            //                                  if (double.IsInfinity(report.TotalPercentage))
            //                                  {
            //                                      report.TotalPercentage = 0;
            //                                  }


            //                                  consolidateBoothReports.Add(report); assemblylistReport.Add(report);
            //                                  assemblyCount++;


            //                              }
            //                              else
            //                              {  //type 1: Electorals
            //                                  report.MaleElectoral = 0;
            //                                  report.FemaleElectoral = 0;
            //                                  report.ThirdGenderElectoral = 0;
            //                                  report.TotalElectoral = 0;

            //                                  //type 2: Votes polled
            //                                  report.MaleVoters = 0;
            //                                  report.FemaleVoters = 0;
            //                                  report.ThirdGenderVoters = 0;
            //                                  report.TotalVoters = 0;
            //                                  report.EPIC = 0;
            //                                  report.VotePolledOtherDocument = 0;
            //                                  report.MalePercentage = 0;
            //                                  report.FemalePercentage = 0;
            //                                  report.ThirdGenderPercentage = 0;
            //                                  report.TotalPercentage = 0;
            //                                  consolidateBoothReports.Add(report); assemblylistReport.Add(report);
            //                                  assemblyCount++;


            //                              }


            //                          }

            //                      }
            //                      else
            //                      {
            //                          VTReportModel report = new VTReportModel();
            //                          report.Header = $"{state.StateName}({state.StateCode})";
            //                          report.Title = $"{state.StateName}";
            //                          report.Type = "PCACWise";
            //                          report.PCCode = pc.PcCodeNo;
            //                          report.PCName = pc.PcName;
            //                          report.AssemblyName = "N/A";
            //                          report.AssemblyCode = "N/A";
            //                          report.MaleElectoral = 0;
            //                          report.FemaleElectoral = 0;
            //                          report.ThirdGenderElectoral = 0;
            //                          report.TotalElectoral = 0;

            //                          //type 2: Votes polled
            //                          report.MaleVoters = 0;
            //                          report.FemaleVoters = 0;
            //                          report.ThirdGenderVoters = 0;
            //                          report.TotalVoters = 0;
            //                          report.EPIC = 0;
            //                          report.MalePercentage = 0;
            //                          report.FemalePercentage = 0;
            //                          report.ThirdGenderPercentage = 0;
            //                          report.TotalPercentage = 0;
            //                          report.VotePolledOtherDocument = 0;
            //                          consolidateBoothReports.Add(report);

            //                      }
            //                      if (assemblyList.Count == assemblyCount)
            //                      {
            //                          // add Total Row
            //                          VTReportModel reportTotal = new VTReportModel();
            //                          reportTotal.Header = "";
            //                          reportTotal.Title = "";
            //                          reportTotal.Type = "";
            //                          reportTotal.DistrictName = "";
            //                          reportTotal.DistrictCode = "";

            //                          reportTotal.AssemblyName = "Total";
            //                          reportTotal.AssemblyCode = "";
            //                          reportTotal.MaleElectoral = assemblylistReport.Sum(report => report.MaleElectoral);
            //                          reportTotal.FemaleElectoral = assemblylistReport.Sum(report => report.FemaleElectoral);
            //                          reportTotal.ThirdGenderElectoral = assemblylistReport.Sum(report => report.ThirdGenderElectoral);
            //                          reportTotal.TotalElectoral = assemblylistReport.Sum(report => report.TotalElectoral);
            //                          reportTotal.MaleVoters = assemblylistReport.Sum(report => report.MaleVoters);
            //                          reportTotal.FemaleVoters = assemblylistReport.Sum(report => report.FemaleVoters);
            //                          reportTotal.ThirdGenderVoters = assemblylistReport.Sum(report => report.ThirdGenderVoters);
            //                          reportTotal.TotalVoters = assemblylistReport.Sum(report => report.TotalVoters);
            //                          reportTotal.EPIC = assemblylistReport.Sum(report => report.EPIC);
            //                          reportTotal.VotePolledOtherDocument = assemblylistReport.Sum(report => report.VotePolledOtherDocument);

            //                          if (assemblylistReport.Any())
            //                          {
            //                              reportTotal.MalePercentage = reportTotal.MaleVoters == 0 ? 0 : (double)reportTotal.MaleVoters / (double)reportTotal.MaleElectoral * 100;
            //                              if (double.IsInfinity(reportTotal.MalePercentage))
            //                              {
            //                                  reportTotal.MalePercentage = 0;
            //                              }
            //                              reportTotal.FemalePercentage = reportTotal.FemaleVoters == 0 ? 0 : (double)reportTotal.FemaleVoters / (double)reportTotal.FemaleElectoral * 100;
            //                              if (double.IsInfinity(reportTotal.FemalePercentage))
            //                              {
            //                                  reportTotal.FemalePercentage = 0;
            //                              }
            //                              reportTotal.ThirdGenderPercentage = reportTotal.ThirdGenderVoters == 0 ? 0 : (double)reportTotal.ThirdGenderVoters / (double)reportTotal.ThirdGenderElectoral * 100;
            //                              if (double.IsInfinity(reportTotal.ThirdGenderPercentage))
            //                              {
            //                                  reportTotal.ThirdGenderPercentage = 0;
            //                              }
            //                              reportTotal.TotalPercentage = reportTotal.TotalVoters == 0 ? 0 : (double)reportTotal.TotalVoters / (double)reportTotal.TotalElectoral * 100;
            //                              if (double.IsInfinity(reportTotal.TotalPercentage))
            //                              {
            //                                  reportTotal.TotalPercentage = 0;
            //                              }
            //                          }
            //                          else
            //                          {
            //                              // Handle the case when assemblylistReport is empty, for example, set averages to 0
            //                              reportTotal.MalePercentage = 0;
            //                              reportTotal.FemalePercentage = 0;
            //                              reportTotal.ThirdGenderPercentage = 0;
            //                              reportTotal.TotalPercentage = 0;
            //                          }




            //                          assemblylistTotal.Add(reportTotal);
            //                          consolidateBoothReports.Add(reportTotal);

            //                      }

            //                  }
            //                  if (consolidateBoothReports.Count > 0)
            //                  {
            //                      // add grand Total after iterations
            //                      VTReportModel reportGrandTotal = new VTReportModel();
            //                      reportGrandTotal.Header = "";
            //                      reportGrandTotal.Title = "";
            //                      reportGrandTotal.Type = "";
            //                      reportGrandTotal.DistrictName = "";
            //                      reportGrandTotal.DistrictCode = "";
            //                      reportGrandTotal.AssemblyName = "Grand Total";
            //                      reportGrandTotal.AssemblyCode = "";
            //                      reportGrandTotal.MaleElectoral = assemblylistTotal.Sum(report => report.MaleElectoral);
            //                      reportGrandTotal.FemaleElectoral = assemblylistTotal.Sum(report => report.FemaleElectoral);
            //                      reportGrandTotal.ThirdGenderElectoral = assemblylistTotal.Sum(report => report.ThirdGenderElectoral);
            //                      reportGrandTotal.TotalElectoral = assemblylistTotal.Sum(report => report.TotalElectoral);
            //                      reportGrandTotal.MaleVoters = assemblylistTotal.Sum(report => report.MaleVoters);
            //                      reportGrandTotal.FemaleVoters = assemblylistTotal.Sum(report => report.FemaleVoters);
            //                      reportGrandTotal.ThirdGenderVoters = assemblylistTotal.Sum(report => report.ThirdGenderVoters);
            //                      reportGrandTotal.TotalVoters = assemblylistTotal.Sum(report => report.TotalVoters);
            //                      reportGrandTotal.EPIC = assemblylistTotal.Sum(report => report.EPIC);
            //                      reportGrandTotal.VotePolledOtherDocument = assemblylistTotal.Sum(report => report.VotePolledOtherDocument);


            //                      //   reportGrandTotal.MalePercentage = (double)reportGrandTotal.MaleVoters / (double)reportGrandTotal.MaleElectoral * 100;
            //                      reportGrandTotal.MalePercentage = (reportGrandTotal.MaleVoters != 0 && reportGrandTotal.MaleElectoral != 0)
            //  ? ((double)reportGrandTotal.MaleVoters / (double)reportGrandTotal.MaleElectoral) * 100
            //  : 0;

            //                      //reportGrandTotal.FemalePercentage = (double)reportGrandTotal.FemaleVoters / (double)reportGrandTotal.FemaleElectoral * 100;
            //                      reportGrandTotal.FemalePercentage = (reportGrandTotal.FemaleVoters != 0 && reportGrandTotal.FemaleElectoral != 0)
            //       ? ((double)reportGrandTotal.FemaleVoters / (double)reportGrandTotal.FemaleElectoral) * 100 : 0;

            //                      reportGrandTotal.ThirdGenderPercentage = (reportGrandTotal.ThirdGenderVoters != 0 && reportGrandTotal.ThirdGenderElectoral != 0)
            //      ? ((double)reportGrandTotal.ThirdGenderVoters / (double)reportGrandTotal.ThirdGenderElectoral) * 100
            //      : 0;

            //                      //reportGrandTotal.TotalPercentage = (double)reportGrandTotal.TotalVoters / (double)reportGrandTotal.TotalElectoral * 100;


            //                      reportGrandTotal.TotalPercentage = (reportGrandTotal.TotalVoters != 0 && reportGrandTotal.TotalElectoral != 0)
            //     ? ((double)reportGrandTotal.TotalVoters / (double)reportGrandTotal.TotalElectoral) * 100
            //     : 0;

            //                      consolidateBoothReports.Add(reportGrandTotal);
            //                  }

            //                  return consolidateBoothReports;

            //                  return consolidateBoothReports;
            //              }
            //          }

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

            return consolidateBoothReports;
        }




        #endregion


        #region GetChartConsolidatedReport

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





        public async Task<List<CombinedMaster>> AppNotDownload(string StateMasterId)
        {

            IQueryable<CombinedMaster> solist = Enumerable.Empty<CombinedMaster>().AsQueryable();

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
            //List<SoCountEventWiseReportModel> modelso = new List<SoCountEventWiseReportModel>();
            //if (boothReportModel.StateMasterId != 0)
            //{
            //    var districtList = await _context.DistrictMaster
            //        .Where(d => d.StateMasterId == boothReportModel.StateMasterId && d.DistrictStatus)
            //        .ToListAsync();



            //    foreach (var district in districtList)
            //    {
            //        if (district.DistrictMasterId == 43 || district.DistrictMasterId == 4)
            //        {

            //            //find total so
            //            var asembCodeList = await _context.AssemblyMaster
            //                .Where(p => p.StateMasterId == boothReportModel.StateMasterId && p.AssemblyStatus && p.DistrictMasterId == district.DistrictMasterId)
            //                .Select(p => p.AssemblyCode)
            //                .ToListAsync();

            //            int totalSO = await _context.SectorOfficerMaster
            //                            .Where(p => p.StateMasterId == boothReportModel.StateMasterId &&
            //                                        p.SoStatus &&
            //                                        asembCodeList.Contains(p.SoAssemblyCode)).CountAsync();

            //            var totalSO_list = await _context.SectorOfficerMaster
            //                   .Where(p => p.StateMasterId == boothReportModel.StateMasterId &&
            //                               p.SoStatus &&
            //                               asembCodeList.Contains(p.SoAssemblyCode)).ToListAsync();

            //            SoCountEventWiseReportModel report = new SoCountEventWiseReportModel
            //            {
            //                District = district.DistrictName,
            //                NoOfSo = totalSO,


            //            };
            //            int pendingPartyDispatchSO = 0; int pendingPartyReachedSO = 0;
            //            int SOBoothNotAlloted = 0;
            //            foreach (var m in totalSO_list)
            //            {

            //                var boothlistMasterid = await _context.BoothMaster.Where(p => p.StateMasterId == boothReportModel.StateMasterId && p.BoothStatus && p.AssignedTo == m.SOMasterId.ToString()).Select(p => p.BoothMasterId).ToListAsync();

            //                if (boothlistMasterid.Count > 0)
            //                {
            //                    //check for all booths of so that ispartydispatch event done
            //                    var allDispatched = await _context.ElectionInfoMaster.Where(ei => boothlistMasterid.Contains(ei.BoothMasterId) && ei.IsPartyDispatched == true)
            //    .Select(ei => ei.BoothMasterId)
            //    .Distinct()
            //    .ToListAsync();
            //                    //check for all booths of so that ispartyreach event done
            //                    var allReached = await _context.ElectionInfoMaster.Where(ei => boothlistMasterid.Contains(ei.BoothMasterId) && ei.IsPartyReached == true)
            //    .Select(ei => ei.BoothMasterId)
            //    .Distinct()
            //    .ToListAsync();
            //                    //compare the total booth of so is equal to alldispatched booths, if not then assign in pendingSo varaiable
            //                    if (boothlistMasterid.Count != allDispatched.Count)
            //                    {
            //                        // report.PartyDispatch = boothlistMasterid.Count - allDispatched.Count;

            //                        pendingPartyDispatchSO = pendingPartyDispatchSO + 1;

            //                    }
            //                    //similar to reach event
            //                    if (boothlistMasterid.Count != allReached.Count)
            //                    {
            //                        //report.PartyReach = boothlistMasterid.Count - allReached.Count;
            //                        pendingPartyReachedSO = pendingPartyReachedSO + 1;


            //                    }
            //                }
            //                else
            //                {
            //                    SOBoothNotAlloted = SOBoothNotAlloted + 1;

            //                }



            //            }

            //            // now here main assignment of returning variable dispatchCount
            //            if (pendingPartyDispatchSO > 0)
            //            {
            //                report.PartyDispatch = pendingPartyDispatchSO;
            //            }
            //            // now here main assignment of returning variable reachCount
            //            if (pendingPartyReachedSO > 0)
            //            {
            //                report.PartyReach = pendingPartyReachedSO;
            //            }

            //            if (SOBoothNotAlloted > 0)
            //            {
            //                report.SoBoothNotAllocated = SOBoothNotAlloted;
            //            }



            //            modelso.Add(report);

            //        }




            //    }
            //}
            //return modelso;
            return null;
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

            //var getsoRecord = _context.SectorOfficerMaster.FirstOrDefault(d => d.SOMasterId == Convert.ToInt32(soMasterId));
            //var getAssemblyRecord = _context.AssemblyMaster.FirstOrDefault(d => d.AssemblyCode == getsoRecord.SoAssemblyCode && d.StateMasterId == getsoRecord.StateMasterId);
            //var eventActivityList = new List<SectorOfficerPendencyBooth>();

            //// Establish a connection to the PostgreSQL database
            //await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("Postgres"));
            //await connection.OpenAsync();

            ////var command = new NpgsqlCommand("SELECT * FROM getboothwiseeventlistbyid_sopendency(@state_master_id, @district_master_id, @assembly_master_id)", connection);
            //var command = new NpgsqlCommand("SELECT * FROM getboothwiseeventlistbyid_sopendency_NEW(@so_master_id,@assembly_master_id)", connection);
            //command.Parameters.AddWithValue("@so_master_id", Convert.ToInt32(soMasterId));
            //command.Parameters.AddWithValue("@assembly_master_id", getAssemblyRecord.AssemblyMasterId);

            //// Execute the command and read the results
            //await using var reader = await command.ExecuteReaderAsync();

            //while (await reader.ReadAsync())
            //{
            //    // Create a new EventActivityBoothWise object and populate its properties from the reader
            //    var eventActivityBoothWise = new SectorOfficerPendencyBooth
            //    {
            //        Key = GenerateRandomAlphanumericString(6), // You need to define this method to generate a random alphanumeric string
            //        MasterId = reader.IsDBNull(0) ? (int?)null : reader.GetInt32(0),
            //        Name = reader.IsDBNull(1) ? null : reader.GetString(1),
            //        Type = "Booth", // Assuming this is the type for booth
            //        AssignedSOMobile = reader.IsDBNull(3) ? null : reader.GetString(3),
            //        AssignedSOName = reader.IsDBNull(2) ? null : reader.GetString(2),
            //        PartyDispatch = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4),
            //        PartyArrived = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5),
            //        SetupPollingStation = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6),
            //        MockPollDone = reader.IsDBNull(7) ? (int?)null : reader.GetInt32(7),
            //        PollStarted = reader.IsDBNull(8) ? (int?)null : reader.GetInt32(8),
            //        VoterTurnOutValue = reader.IsDBNull(16) ? (int?)null : reader.GetInt32(16),
            //        QueueValue = reader.IsDBNull(14) ? (int?)null : reader.GetInt32(14),
            //        FinalVotesValue = reader.IsDBNull(15) ? (int?)null : reader.GetInt32(15),
            //        PollEnded = reader.IsDBNull(9) ? (int?)null : reader.GetInt32(9),
            //        MCEVMOff = reader.IsDBNull(10) ? (int?)null : reader.GetInt32(10),
            //        PartyDeparted = reader.IsDBNull(11) ? (int?)null : reader.GetInt32(11),
            //        EVMDeposited = reader.IsDBNull(12) ? (int?)null : reader.GetInt32(12),
            //        PartyReachedAtCollection = reader.IsDBNull(13) ? (int?)null : reader.GetInt32(13)
            //    };

            //    // Add the object to the list
            //    eventActivityList.Add(eventActivityBoothWise);
            //}

            //return eventActivityList;
            return null;
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
        public async Task<FieldOfficerProfile> GetBLOOfficerProfile(string bloMasterId)
        {
            int bloIdInt = Convert.ToInt32(bloMasterId);

            var result = await (from blo in _context.BLOMaster
                                where blo.BLOMasterId == bloIdInt && blo.BLOStatus == true
                                join state in _context.StateMaster on blo.StateMasterId equals state.StateMasterId
                                join district in _context.DistrictMaster on blo.DistrictMasterId equals district.DistrictMasterId
                                join assembly in _context.AssemblyMaster on blo.AssemblyMasterId equals assembly.AssemblyMasterId
                                select new FieldOfficerProfile
                                {
                                    StateName = state.StateName,
                                    DistrictName = district.DistrictName,
                                    AssemblyName = assembly.AssemblyName,
                                    //AssemblyCode = assembly.AssemblyCode.ToString(),
                                    FoName = blo.BLOName,
                                    Role = "BLO",
                                    //ElectionType = "LS",
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
                                 FieldOfficerMasterId = so.BLOMasterId,
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
                                 FieldOfficerMasterId = so.BLOMasterId,
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

            var isSoExistMobileNumber = _context.FieldOfficerMaster.Where(d => d.FieldOfficerMobile == updatedBLOMaster.BLOMobile).FirstOrDefault();
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

            var isSoExistMobileNumber = _context.FieldOfficerMaster.Where(d => d.FieldOfficerMobile == bLOMaster.BLOMobile).FirstOrDefault();
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
                                           FieldOfficerMasterId = Convert.ToInt32(bloId)


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
        public async Task<ServiceResponse> UpdateKycDetails(Kyc kyc)
        {
            var existingKyc = await _context.Kyc.FirstOrDefaultAsync(k => k.KycMasterId == kyc.KycMasterId);

            if (existingKyc == null)
            {
                return new ServiceResponse { IsSucceed = false, Message = "KYC not found" };
            }

            // Update properties of the existing Kyc entity
            existingKyc.StateMasterId = kyc.StateMasterId;
            existingKyc.DistrictMasterId = kyc.DistrictMasterId;
            existingKyc.ElectionTypeMasterId = kyc.ElectionTypeMasterId;
            existingKyc.AssemblyMasterId = kyc.AssemblyMasterId;
            existingKyc.FourthLevelHMasterId = kyc.FourthLevelHMasterId;
            existingKyc.PSZonePanchayatMasterId = kyc.PSZonePanchayatMasterId;
            existingKyc.GPPanchayatWardsMasterId = kyc.GPPanchayatWardsMasterId;
            existingKyc.CandidateName = kyc.CandidateName;
            existingKyc.FatherName = kyc.FatherName;
            if (!string.IsNullOrEmpty(kyc.NominationPdfPath))
            {
                existingKyc.NominationPdfPath = kyc.NominationPdfPath;
            }
            else
            {
                existingKyc.NominationPdfPath = existingKyc.NominationPdfPath;
            }
            existingKyc.Option1 = kyc.Option1;
            existingKyc.Option2 = kyc.Option2;
            _context.Kyc.Update(existingKyc);
            await _context.SaveChangesAsync();

            return new ServiceResponse { IsSucceed = true, Message = "KYC updated successfully" };
        }
        public async Task<List<Kyc>> GetKYCDetails()
        {
            return await _context.Kyc.ToListAsync();
        }
        public async Task<List<KycList>> GetKYCDetailByAssemblyId(int electionType, int stateMasterId, int districtMasterId, int assemblyMasterId)
        {
            var baseUrl = "https://lbpams.punjab.gov.in/LBPAMSDOC/";

            // Execute the query with the necessary where conditions
            var kycList = from k in _context.Kyc
                          join state in _context.StateMaster on k.StateMasterId equals state.StateMasterId
                          join district in _context.DistrictMaster on k.DistrictMasterId equals district.DistrictMasterId into districts
                          from d in districts.DefaultIfEmpty()
                          join assembly in _context.AssemblyMaster on k.AssemblyMasterId equals assembly.AssemblyMasterId into assemblies
                          from a in assemblies.DefaultIfEmpty()
                          join fourthLevel in _context.FourthLevelH on k.FourthLevelHMasterId equals fourthLevel.FourthLevelHMasterId into fourthLevels
                          from fl in fourthLevels.DefaultIfEmpty()
                          join psZone in _context.PSZonePanchayat on k.PSZonePanchayatMasterId equals psZone.PSZonePanchayatMasterId into psZones
                          from pz in psZones.DefaultIfEmpty()
                          join gpWard in _context.GPPanchayatWards on k.GPPanchayatWardsMasterId equals gpWard.GPPanchayatWardsMasterId into gpWards
                          from gw in gpWards.DefaultIfEmpty()
                          where
                                k.StateMasterId == stateMasterId &&
                                k.DistrictMasterId == districtMasterId &&
                                k.AssemblyMasterId == assemblyMasterId
                          select new KycList
                          {
                              KycMasterId = k.KycMasterId,
                              StateName = state.StateName,
                              StateMasterId = k.StateMasterId,
                              DistrictName = d.DistrictName,
                              DistrictMasterId = k.DistrictMasterId,
                              AssemblyName = a.AssemblyName,
                              AssemblyMasterId = k.AssemblyMasterId,
                              FourthLevelHName = fl.HierarchyName,
                              FourthLevelHMasterId = k.FourthLevelHMasterId,
                              GPPanchayatWardsName = gw.GPPanchayatWardsName,
                              GPPanchayatWardsMasterId = k.GPPanchayatWardsMasterId,
                              CandidateType = k.GPPanchayatWardsMasterId == 0 ? "Sarpanch" : "Panch",
                              CandidateName = k.CandidateName,
                              FatherName = k.FatherName,
                              NominationPdfPath = $"{baseUrl}{k.NominationPdfPath}",
                          };

            return await kycList.ToListAsync();
        }

        public async Task<KycList> GetKycById(int kycMasterId)
        {
            var kyc = await _context.Kyc.FirstOrDefaultAsync(d => d.KycMasterId == kycMasterId);
            var electionType = await _context.ElectionTypeMaster.FirstOrDefaultAsync(d => d.ElectionTypeMasterId == kyc.ElectionTypeMasterId);
            if (kyc == null)
            {
                return null; // or throw an exception, or handle the case appropriately
            }
            var baseUrl = "https://lbpams.punjab.gov.in/LBPAMSDOC/";
            //PS Zone Panchayat
            if (kyc.PSZonePanchayatMasterId != 0)
            {
                var panchayat = await _context.PSZonePanchayat
                    .Where(d => d.PSZonePanchayatMasterId == kyc.PSZonePanchayatMasterId)
                    .Include(d => d.StateMaster)
                    .Include(d => d.DistrictMaster)
                    .Include(d => d.AssemblyMaster)
                    .Include(d => d.FourthLevelH)
                    .FirstOrDefaultAsync();

                if (panchayat == null)
                {
                    return null; // or handle the case appropriately
                }

                var result = new KycList
                {
                    KycMasterId = kyc.KycMasterId,
                    ElectionTypeMasterId = kyc.ElectionTypeMasterId,
                    ElectionTypeName = electionType.ElectionType,
                    StateMasterId = panchayat.StateMasterId,
                    StateName = panchayat.StateMaster.StateName,
                    DistrictMasterId = panchayat.DistrictMasterId,
                    DistrictName = panchayat.DistrictMaster.DistrictName,
                    AssemblyMasterId = panchayat.AssemblyMasterId,
                    AssemblyName = panchayat.AssemblyMaster.AssemblyName,
                    FourthLevelHMasterId = panchayat.FourthLevelHMasterId,
                    FourthLevelHName = panchayat.FourthLevelH.HierarchyName,
                    PSZonePanchayatMasterId = panchayat.PSZonePanchayatMasterId,
                    PSZonePanchayatName = panchayat.PSZonePanchayatName,
                    CandidateName = kyc.CandidateName,
                    FatherName = kyc.FatherName,
                    NominationPdfPath = $"{baseUrl}{kyc.NominationPdfPath}"
                };

                return result;
            }
            //Gp Panchayat ward
            else if (kyc.GPPanchayatWardsMasterId != 0)
            {
                var gpWard = await _context.GPPanchayatWards
                   .Where(d => d.GPPanchayatWardsMasterId == kyc.GPPanchayatWardsMasterId)
                   .Include(d => d.StateMaster)
                   .Include(d => d.DistrictMaster)
                   .Include(d => d.AssemblyMaster)
                   .Include(d => d.FourthLevelH)
                   .FirstOrDefaultAsync();

                if (gpWard == null)
                {
                    return null; // or handle the case appropriately
                }

                var result = new KycList
                {
                    KycMasterId = kyc.KycMasterId,
                    ElectionTypeMasterId = kyc.ElectionTypeMasterId,
                    ElectionTypeName = electionType.ElectionType,
                    StateMasterId = gpWard.StateMasterId,
                    StateName = gpWard.StateMaster.StateName,
                    DistrictMasterId = gpWard.DistrictMasterId,
                    DistrictName = gpWard.DistrictMaster.DistrictName,
                    AssemblyMasterId = gpWard.AssemblyMasterId,
                    AssemblyName = gpWard.AssemblyMaster.AssemblyName,
                    FourthLevelHMasterId = gpWard.FourthLevelHMasterId,
                    FourthLevelHName = gpWard.FourthLevelH.HierarchyName,
                    GPPanchayatWardsMasterId = gpWard.GPPanchayatWardsMasterId,
                    GPPanchayatWardsName = gpWard.GPPanchayatWardsName,
                    CandidateName = kyc.CandidateName,
                    FatherName = kyc.FatherName,
                    NominationPdfPath = $"{baseUrl}{kyc.NominationPdfPath}"
                };

                return result;
            }
            //fourth level
            else if (kyc.FourthLevelHMasterId != 0)
            {
                var fourthLevel = await _context.FourthLevelH
                  .Where(d => d.FourthLevelHMasterId == kyc.FourthLevelHMasterId)
                  .Include(d => d.StateMaster)
                  .Include(d => d.DistrictMaster)
                  .Include(d => d.AssemblyMaster)

                  .FirstOrDefaultAsync();

                if (fourthLevel == null)
                {
                    return null; // or handle the case appropriately
                }

                var result = new KycList
                {
                    KycMasterId = kyc.KycMasterId,
                    ElectionTypeMasterId = kyc.ElectionTypeMasterId,
                    ElectionTypeName = electionType.ElectionType,
                    StateMasterId = fourthLevel.StateMasterId,
                    StateName = fourthLevel.StateMaster.StateName,
                    DistrictMasterId = fourthLevel.DistrictMasterId,
                    DistrictName = fourthLevel.DistrictMaster.DistrictName,
                    AssemblyMasterId = fourthLevel.AssemblyMasterId,
                    AssemblyName = fourthLevel.AssemblyMaster.AssemblyName,
                    FourthLevelHMasterId = fourthLevel.FourthLevelHMasterId,
                    FourthLevelHName = fourthLevel.HierarchyName,
                    CandidateName = kyc.CandidateName,
                    FatherName = kyc.FatherName,
                    NominationPdfPath = $"{baseUrl}{kyc.NominationPdfPath}"
                };

                return result;
            }
            //assembly level
            else
            {
                var assemblyMaster = await _context.AssemblyMaster
                  .Where(d => d.AssemblyMasterId == kyc.AssemblyMasterId)
                  .Include(d => d.StateMaster)
                  .Include(d => d.DistrictMaster)

                  .FirstOrDefaultAsync();

                if (assemblyMaster == null)
                {
                    return null; // or handle the case appropriately
                }

                var result = new KycList
                {
                    KycMasterId = kyc.KycMasterId,
                    ElectionTypeMasterId = kyc.ElectionTypeMasterId,
                    ElectionTypeName = electionType.ElectionType,
                    StateMasterId = assemblyMaster.StateMasterId,
                    StateName = assemblyMaster.StateMaster.StateName,
                    DistrictMasterId = assemblyMaster.DistrictMasterId,
                    DistrictName = assemblyMaster.DistrictMaster.DistrictName,
                    AssemblyMasterId = assemblyMaster.AssemblyMasterId,
                    AssemblyName = assemblyMaster.AssemblyName,
                    CandidateName = kyc.CandidateName,
                    FatherName = kyc.FatherName,
                    NominationPdfPath = $"{baseUrl}{kyc.NominationPdfPath}"
                };

                return result;
            }
        }

        public async Task<ServiceResponse> DeleteKycById(int kycMasterId)
        {
            var isExist = await _context.Kyc.Where(d => d.KycMasterId == kycMasterId).FirstOrDefaultAsync();
            if (isExist == null)
            {
                return new ServiceResponse { IsSucceed = false, Message = "Record not Found" };
            }
            else
            {
                _context.Kyc.Remove(isExist);
                _context.SaveChanges();
                return new ServiceResponse { IsSucceed = true, Message = "Record Deleted successfully" };
            }
        }

        #endregion 

        #region UnOpposed Public Details
        public async Task<ServiceResponse> AddUnOpposedDetails(UnOpposed unOpposed)
        {
            bool isExist;

            // Check if ElectionTypeMasterId is not equal to 2
            if (unOpposed.ElectionTypeMasterId != 2)
            {
                // panch
                if (unOpposed.GPPanchayatWardsMasterId != 0)
                {
                    isExist = await _context.UnOpposed.AnyAsync(d =>
                        d.StateMasterId == unOpposed.StateMasterId &&
                        d.DistrictMasterId == unOpposed.DistrictMasterId &&
                        d.AssemblyMasterId == unOpposed.AssemblyMasterId &&
                        d.FourthLevelHMasterId == unOpposed.FourthLevelHMasterId &&
                        d.GPPanchayatWardsMasterId == unOpposed.GPPanchayatWardsMasterId && d.ElectionTypeMasterId == unOpposed.ElectionTypeMasterId);
                }
                else //sarpanch and other
                {
                    isExist = await _context.UnOpposed.AnyAsync(d =>
                        d.StateMasterId == unOpposed.StateMasterId &&
                        d.DistrictMasterId == unOpposed.DistrictMasterId &&
                        d.AssemblyMasterId == unOpposed.AssemblyMasterId &&
                        d.FourthLevelHMasterId == unOpposed.FourthLevelHMasterId && d.GPPanchayatWardsMasterId == 0 && d.ElectionTypeMasterId == unOpposed.ElectionTypeMasterId);
                }
            }
            else // When ElectionTypeMasterId equals 2
            {
                isExist = await _context.UnOpposed.AnyAsync(d =>
                    d.StateMasterId == unOpposed.StateMasterId &&
                    d.DistrictMasterId == unOpposed.DistrictMasterId &&
                    d.AssemblyMasterId == unOpposed.AssemblyMasterId && d.ElectionTypeMasterId == unOpposed.ElectionTypeMasterId);
            }

            // Return if the candidate already exists
            if (isExist)
            {
                return new ServiceResponse() { IsSucceed = false, Message = "UnOpposed Candidate Already Exist" };
            }

            // Add the new UnOpposed record
            _context.UnOpposed.Add(unOpposed);
            await _context.SaveChangesAsync();

            return new ServiceResponse { IsSucceed = true, Message = "Successfully added" };
        }

        public async Task<List<UnOpposed>> GetUnOpposedDetails()
        {
            return await _context.UnOpposed.ToListAsync();

        }
        public async Task<List<UnOpposedList>> GetUnOpposedDetailsByAssemblyId(int electionType, int stateMasterId, int districtMasterId, int assemblyMasterId)
        {
            var baseUrl = "https://lbpams.punjab.gov.in/LBPAMSDOC/";

            // Execute the initial query that can be translated to SQL
            var unOpposedList = from un in _context.UnOpposed
                                join state in _context.StateMaster on un.StateMasterId equals state.StateMasterId
                                join district in _context.DistrictMaster on un.DistrictMasterId equals district.DistrictMasterId into districts
                                from d in districts.DefaultIfEmpty()
                                join assembly in _context.AssemblyMaster on un.AssemblyMasterId equals assembly.AssemblyMasterId into assemblies
                                from a in assemblies.DefaultIfEmpty()
                                join fourthLevel in _context.FourthLevelH on un.FourthLevelHMasterId equals fourthLevel.FourthLevelHMasterId into fourthLevels
                                from fl in fourthLevels.DefaultIfEmpty()
                                join psZone in _context.PSZonePanchayat on un.PSZonePanchayatMasterId equals psZone.PSZonePanchayatMasterId into psZones
                                from pz in psZones.DefaultIfEmpty()
                                join gpWard in _context.GPPanchayatWards on un.GPPanchayatWardsMasterId equals gpWard.GPPanchayatWardsMasterId into gpWards
                                from gw in gpWards.DefaultIfEmpty()
                                where
                                         un.StateMasterId == stateMasterId &&
                                        un.DistrictMasterId == districtMasterId &&
                                        un.AssemblyMasterId == assemblyMasterId
                                select new UnOpposedList
                                {
                                    UnOpposedMasterId = un.UnOpposedMasterId,
                                    StateName = state.StateName,
                                    StateMasterId = un.StateMasterId,
                                    DistrictName = d.DistrictName,
                                    DistrictMasterId = un.DistrictMasterId,
                                    AssemblyName = a.AssemblyName,
                                    AssemblyMasterId = un.AssemblyMasterId,
                                    FourthLevelHName = fl.HierarchyName,
                                    FourthLevelHMasterId = un.FourthLevelHMasterId,
                                    GPPanchayatWardsName = gw.GPPanchayatWardsName,
                                    GPPanchayatWardsMasterId = un.GPPanchayatWardsMasterId,
                                    CandidateType = un.GPPanchayatWardsMasterId == 0 ? "Sarpanch" : "Panch",
                                    CandidateName = un.CandidateName,
                                    FatherName = un.FatherName,
                                    NominationPdfPath = $"{baseUrl}{un.NominationPdfPath}",

                                };



            return unOpposedList.ToList();


        }
        public async Task<ServiceResponse> UpdateUnOpposedDetails(UnOpposed unOpposed)
        {
            var existing = await _context.UnOpposed.FirstOrDefaultAsync(k => k.UnOpposedMasterId == unOpposed.UnOpposedMasterId);

            if (existing == null)
            {
                return new ServiceResponse { IsSucceed = false, Message = "KYC not found" };
            }

            // Update properties of the existing Kyc entity
            existing.StateMasterId = unOpposed.StateMasterId;
            existing.DistrictMasterId = unOpposed.DistrictMasterId;
            existing.ElectionTypeMasterId = unOpposed.ElectionTypeMasterId;
            existing.AssemblyMasterId = unOpposed.AssemblyMasterId;
            existing.FourthLevelHMasterId = unOpposed.FourthLevelHMasterId;
            existing.PSZonePanchayatMasterId = unOpposed.PSZonePanchayatMasterId;
            existing.GPPanchayatWardsMasterId = unOpposed.GPPanchayatWardsMasterId;
            existing.CandidateName = unOpposed.CandidateName;
            existing.FatherName = unOpposed.FatherName;
            if (!string.IsNullOrEmpty(unOpposed.NominationPdfPath))
            {
                existing.NominationPdfPath = unOpposed.NominationPdfPath;
            }
            else
            {
                existing.NominationPdfPath = existing.NominationPdfPath;
            }
            existing.Option1 = unOpposed.Option1;
            existing.Option2 = unOpposed.Option2;
            _context.UnOpposed.Update(existing);
            await _context.SaveChangesAsync();

            return new ServiceResponse { IsSucceed = true, Message = "KYC updated successfully" };
        }

        public async Task<UnOpposedList> GetUnOpposedById(int unOpposedMasterId)
        {
            var unOpposed = await _context.UnOpposed.FirstOrDefaultAsync(d => d.UnOpposedMasterId == unOpposedMasterId);
            var electionType = await _context.ElectionTypeMaster.FirstOrDefaultAsync(d => d.ElectionTypeMasterId == unOpposed.ElectionTypeMasterId);
            if (unOpposed == null)
            {
                return null; // or throw an exception, or handle the case appropriately
            }
            var baseUrl = "https://lbpams.punjab.gov.in/LBPAMSDOC/";
            if (unOpposed.PSZonePanchayatMasterId != 0)
            {
                var panchayat = await _context.PSZonePanchayat
                    .Where(d => d.PSZonePanchayatMasterId == unOpposed.PSZonePanchayatMasterId)
                    .Include(d => d.StateMaster)
                    .Include(d => d.DistrictMaster)
                    .Include(d => d.AssemblyMaster)
                    .Include(d => d.FourthLevelH)
                    .FirstOrDefaultAsync();

                if (panchayat == null)
                {
                    return null; // or handle the case appropriately
                }

                var result = new UnOpposedList
                {
                    UnOpposedMasterId = unOpposed.UnOpposedMasterId,
                    ElectionTypeMasterId = unOpposed.ElectionTypeMasterId,
                    ElectionTypeName = electionType.ElectionType,
                    StateMasterId = panchayat.StateMasterId,
                    StateName = panchayat.StateMaster.StateName,
                    DistrictMasterId = panchayat.DistrictMasterId,
                    DistrictName = panchayat.DistrictMaster.DistrictName,
                    AssemblyMasterId = panchayat.AssemblyMasterId,
                    AssemblyName = panchayat.AssemblyMaster.AssemblyName,
                    FourthLevelHMasterId = panchayat.FourthLevelHMasterId,
                    FourthLevelHName = panchayat.FourthLevelH.HierarchyName,
                    PSZonePanchayatMasterId = panchayat.PSZonePanchayatMasterId,
                    PSZonePanchayatName = panchayat.PSZonePanchayatName,
                    CandidateName = unOpposed.CandidateName,
                    FatherName = unOpposed.FatherName,
                    NominationPdfPath = $"{baseUrl}{unOpposed.NominationPdfPath}",
                };

                return result;
            }
            else if (unOpposed.GPPanchayatWardsMasterId != 0)
            {
                var gpWards = await _context.GPPanchayatWards
                    .Where(d => d.GPPanchayatWardsMasterId == unOpposed.GPPanchayatWardsMasterId)
                    .Include(d => d.StateMaster)
                    .Include(d => d.DistrictMaster)
                    .Include(d => d.AssemblyMaster)
                    .Include(d => d.FourthLevelH)
                    .FirstOrDefaultAsync();

                if (gpWards == null)
                {
                    return null; // or handle the case appropriately
                }

                var result = new UnOpposedList
                {
                    UnOpposedMasterId = unOpposed.UnOpposedMasterId,
                    ElectionTypeMasterId = unOpposed.ElectionTypeMasterId,
                    ElectionTypeName = electionType.ElectionType,
                    StateMasterId = gpWards.StateMasterId,
                    StateName = gpWards.StateMaster.StateName,
                    DistrictMasterId = gpWards.DistrictMasterId,
                    DistrictName = gpWards.DistrictMaster.DistrictName,
                    AssemblyMasterId = gpWards.AssemblyMasterId,
                    AssemblyName = gpWards.AssemblyMaster.AssemblyName,
                    FourthLevelHMasterId = gpWards.FourthLevelHMasterId,
                    FourthLevelHName = gpWards.FourthLevelH.HierarchyName,
                    GPPanchayatWardsMasterId = gpWards.GPPanchayatWardsMasterId,
                    GPPanchayatWardsName = gpWards.GPPanchayatWardsName,
                    CandidateName = unOpposed.CandidateName,
                    FatherName = unOpposed.FatherName,
                    NominationPdfPath = $"{baseUrl}{unOpposed.NominationPdfPath}",
                };

                return result;

            }
            else if (unOpposed.FourthLevelHMasterId != 0)
            {
                var fourthLevel = await _context.FourthLevelH
                   .Where(d => d.FourthLevelHMasterId == unOpposed.FourthLevelHMasterId)
                   .Include(d => d.StateMaster)
                   .Include(d => d.DistrictMaster)
                   .Include(d => d.AssemblyMaster)

                   .FirstOrDefaultAsync();

                if (fourthLevel == null)
                {
                    return null; // or handle the case appropriately
                }

                var result = new UnOpposedList
                {
                    UnOpposedMasterId = unOpposed.UnOpposedMasterId,
                    ElectionTypeMasterId = unOpposed.ElectionTypeMasterId,
                    ElectionTypeName = electionType.ElectionType,
                    StateMasterId = fourthLevel.StateMasterId,
                    StateName = fourthLevel.StateMaster.StateName,
                    DistrictMasterId = fourthLevel.DistrictMasterId,
                    DistrictName = fourthLevel.DistrictMaster.DistrictName,
                    AssemblyMasterId = fourthLevel.AssemblyMasterId,
                    AssemblyName = fourthLevel.AssemblyMaster.AssemblyName,
                    FourthLevelHMasterId = fourthLevel.FourthLevelHMasterId,
                    FourthLevelHName = fourthLevel.HierarchyName,
                    CandidateName = unOpposed.CandidateName,
                    FatherName = unOpposed.FatherName,
                    NominationPdfPath = $"{baseUrl}{unOpposed.NominationPdfPath}",
                };

                return result;
            }
            else
            {
                var assembly = await _context.AssemblyMaster
                   .Where(d => d.AssemblyMasterId == unOpposed.AssemblyMasterId)
                   .Include(d => d.StateMaster)
                   .Include(d => d.DistrictMaster)
                   .FirstOrDefaultAsync();

                if (assembly == null)
                {
                    return null; // or handle the case appropriately
                }

                var result = new UnOpposedList
                {
                    UnOpposedMasterId = unOpposed.UnOpposedMasterId,
                    ElectionTypeMasterId = unOpposed.ElectionTypeMasterId,
                    ElectionTypeName = electionType.ElectionType,
                    StateMasterId = assembly.StateMasterId,
                    StateName = assembly.StateMaster.StateName,
                    DistrictMasterId = assembly.DistrictMasterId,
                    DistrictName = assembly.DistrictMaster.DistrictName,
                    AssemblyMasterId = assembly.AssemblyMasterId,
                    AssemblyName = assembly.AssemblyName,

                    CandidateName = unOpposed.CandidateName,
                    FatherName = unOpposed.FatherName,
                    NominationPdfPath = $"{baseUrl}{unOpposed.NominationPdfPath}",
                };

                return result;
            }
        }
        public async Task<ServiceResponse> DeleteUnOpposedById(int unOpposedMasterId)
        {
            var isExist = await _context.UnOpposed.Where(d => d.UnOpposedMasterId == unOpposedMasterId).FirstOrDefaultAsync();
            if (isExist == null)
            {
                return new ServiceResponse { IsSucceed = false, Message = "Record not Found" };
            }
            else
            {
                _context.UnOpposed.Remove(isExist);
                _context.SaveChanges();
                return new ServiceResponse { IsSucceed = true, Message = "Record Deleted successfully" };
            }
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

        #region Fourth Level
        public async Task<Response> AddFourthLevelH(FourthLevelH fourthLevelH)
        {
            try
            {
                var isFourthLevelHExist = await _context.FourthLevelH.Where(p => p.StateMasterId == fourthLevelH.StateMasterId && p.DistrictMasterId == fourthLevelH.DistrictMasterId && p.AssemblyMasterId == fourthLevelH.AssemblyMasterId && p.HierarchyCode == fourthLevelH.HierarchyCode && p.ElectionTypeMasterId == fourthLevelH.ElectionTypeMasterId).FirstOrDefaultAsync();

                if (isFourthLevelHExist == null)
                {

                    fourthLevelH.HierarchyCreatedAt = BharatDateTime();
                    _context.FourthLevelH.Add(fourthLevelH);
                    _context.SaveChanges();

                    return new Response { Status = RequestStatusEnum.OK, Message = fourthLevelH.HierarchyName + " Added Successfully" };



                }
                else
                {
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = isFourthLevelHExist.HierarchyName + " Same Hierarchy  Code Already Exists in the selected Election Type" };

                }

            }

            catch (Exception ex)
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = ex.Message };
            }

        }
        public async Task<List<FourthLevelH>> GetFourthLevelHListById(int stateMasterId, int districtMasterId, int assemblyMasterId)
        {
            var getFourthLevelH = await _context.FourthLevelH.Where(d => d.StateMasterId == stateMasterId && d.DistrictMasterId == districtMasterId
                && d.AssemblyMasterId == assemblyMasterId).Include(d => d.StateMaster).Include(d => d.DistrictMaster).Include(d => d.AssemblyMaster).Include(d => d.ElectionTypeMaster).ToListAsync();
            if (getFourthLevelH != null)
            {
                return getFourthLevelH;
            }
            else
            {
                return null;
            }
        }

        public async Task<Response> UpdateFourthLevelH(FourthLevelH fourthLevelH)
        {
            // Retrieve the existing entity
            var existing = await _context.FourthLevelH
                .Where(d => d.FourthLevelHMasterId == fourthLevelH.FourthLevelHMasterId)
                .FirstOrDefaultAsync();

            if (existing == null)
            {
                return new Response
                {
                    Status = RequestStatusEnum.BadRequest,
                    Message = "Hierarchy not found."
                };
            }

            // Check if the HierarchyCode exists in other records
            var isFourthLevelHCodeExist = await _context.FourthLevelH
                .Where(p => p.StateMasterId == fourthLevelH.StateMasterId
                            && p.DistrictMasterId == fourthLevelH.DistrictMasterId
                            && p.AssemblyMasterId == fourthLevelH.AssemblyMasterId
                            && p.HierarchyCode == fourthLevelH.HierarchyCode
                            && p.ElectionTypeMasterId == fourthLevelH.ElectionTypeMasterId) // Exclude current entity
                .FirstOrDefaultAsync();

            if (isFourthLevelHCodeExist != null && existing.HierarchyCode != isFourthLevelHCodeExist.HierarchyCode)
            {
                return new Response
                {
                    Status = RequestStatusEnum.BadRequest,
                    Message = "Hierarchy code already exists for this combination."
                };
            }

            // Allow update if code is the same as existing record or new
            if (isFourthLevelHCodeExist == null || isFourthLevelHCodeExist.HierarchyCode == existing.HierarchyCode)
            {
                // Update the properties of the existing entity
                existing.HierarchyName = fourthLevelH.HierarchyName;
                existing.HierarchyCode = fourthLevelH.HierarchyCode;
                existing.HierarchyType = fourthLevelH.HierarchyType;
                existing.ElectionTypeMasterId = fourthLevelH.ElectionTypeMasterId;
                existing.StateMasterId = fourthLevelH.StateMasterId;
                existing.DistrictMasterId = fourthLevelH.DistrictMasterId;
                existing.AssemblyMasterId = fourthLevelH.AssemblyMasterId;
                existing.HierarchyCreatedAt = fourthLevelH.HierarchyCreatedAt;
                existing.HierarchyUpdatedAt = DateTime.UtcNow;
                existing.HierarchyStatus = fourthLevelH.HierarchyStatus;

                // Save changes to the database
                try
                {
                    await _context.SaveChangesAsync();
                    return new Response
                    {
                        Status = RequestStatusEnum.OK,
                        Message = "Updated successfully."
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

            return new Response
            {
                Status = RequestStatusEnum.BadRequest,
                Message = "Already exists for this code."
            };
        }

        public async Task<FourthLevelH> GetFourthLevelHById(int stateMasterId, int districtMasterId, int assemblyMasterId, int fourthLevelHMasterId)
        {
            var fourthLevelH = await _context.FourthLevelH
                .Where(d => d.StateMasterId == stateMasterId &&
                            d.DistrictMasterId == districtMasterId &&
                            d.AssemblyMasterId == assemblyMasterId &&
                            d.FourthLevelHMasterId == fourthLevelHMasterId)
                .Include(d => d.StateMaster).Include(d => d.DistrictMaster).Include(d => d.AssemblyMaster).Include(d => d.ElectionTypeMaster)
                .FirstOrDefaultAsync();

            return fourthLevelH;
        }

        public async Task<ServiceResponse> DeleteFourthLevelHById(int stateMasterId, int districtMasterId, int assemblyMasterId, int fourthLevelHMasterId)
        {
            // Validate the input ID
            if (fourthLevelHMasterId <= 0)
            {
                return new ServiceResponse
                {
                    IsSucceed = false,
                    Message = "Invalid fourthLevelHMasterId provided."
                };
            }

            // Check if the PSZone entity exists in the database
            var fourthLevelH = await _context.FourthLevelH
                .Where(d => d.FourthLevelHMasterId == fourthLevelHMasterId && d.StateMasterId == stateMasterId && d.DistrictMasterId == districtMasterId && d.AssemblyMasterId == assemblyMasterId)
                .FirstOrDefaultAsync();

            if (fourthLevelH == null)
            {
                return new ServiceResponse
                {
                    IsSucceed = false,
                    Message = "Hierarchy not found."
                };
            }

            // Check if there are any related records in the BoothMaster table
            var boothExists = await _context.BoothMaster
                .CountAsync(b => b.FourthLevelHMasterId == fourthLevelHMasterId &&
                               b.StateMasterId == stateMasterId &&
                               b.DistrictMasterId == districtMasterId &&
                               b.AssemblyMasterId == assemblyMasterId &&
                               b.ElectionTypeMasterId == fourthLevelH.ElectionTypeMasterId);

            if (boothExists > 0)
            {
                return new ServiceResponse
                {
                    IsSucceed = false,
                    Message = $"Cannot delete this hierarchy. There are {boothExists} related booths. Please delete them first."
                };
            }

            // Perform the deletion
            try
            {
                _context.FourthLevelH.Remove(fourthLevelH);
                await _context.SaveChangesAsync();

                return new ServiceResponse
                {
                    IsSucceed = true,
                    Message = "Hierarchy deleted successfully."
                };
            }
            catch (Exception ex)
            {
                // Handle any errors that may have occurred during deletion
                return new ServiceResponse
                {
                    IsSucceed = false,
                    Message = $"An error occurred: {ex.Message}"
                };
            }
        }

        #endregion

        #region  PSZonePanchayat
        public async Task<Response> AddPSZonePanchayat(PSZonePanchayat psZonePanchayat)
        {
            try
            {
                var existingPanchayat = await _context.PSZonePanchayat
                    .FirstOrDefaultAsync(p => p.PSZonePanchayatCode == psZonePanchayat.PSZonePanchayatCode
                                              && p.StateMasterId == psZonePanchayat.StateMasterId
                                              && p.DistrictMasterId == psZonePanchayat.DistrictMasterId
                                              && p.AssemblyMasterId == psZonePanchayat.AssemblyMasterId
                                              && p.ElectionTypeMasterId == psZonePanchayat.ElectionTypeMasterId
                                              && p.FourthLevelHMasterId == psZonePanchayat.FourthLevelHMasterId);

                if (existingPanchayat != null)
                {
                    return new Response
                    {
                        Status = RequestStatusEnum.BadRequest,
                        Message = "PS Zone Panchayat already exists for this code"
                    };
                }

                psZonePanchayat.PSZonePanchayatCreatedAt = BharatDateTime();
                _context.PSZonePanchayat.Add(psZonePanchayat);
                await _context.SaveChangesAsync();

                return new Response
                {
                    Status = RequestStatusEnum.OK,
                    Message = $"{psZonePanchayat.PSZonePanchayatName} added successfully"
                };
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                return new Response
                {
                    Status = RequestStatusEnum.BadRequest,
                    Message = ex.Message
                };
            }
        }

        public async Task<List<PSZonePanchayat>> GetPSZonePanchayatListById(int stateMasterId, int districtMasterId, int assemblyMasterId, int fourthLevelHMasterId)
        {
            var getBlockPanchayat = await _context.PSZonePanchayat.Where(d => d.StateMasterId == stateMasterId && d.DistrictMasterId == districtMasterId && d.AssemblyMasterId == assemblyMasterId && d.FourthLevelHMasterId == fourthLevelHMasterId).Include(d => d.StateMaster).Include(d => d.DistrictMaster).Include(d => d.AssemblyMaster).Include(d => d.FourthLevelH).Include(d => d.ElectionTypeMaster).ToListAsync();
            if (getBlockPanchayat != null)
            {
                return getBlockPanchayat;
            }
            else
            {
                return null;
            }
        }
        public async Task<Response> UpdatePSZonePanchayat(PSZonePanchayat updatedPSZonePanchayat)
        {
            try
            {
                var existingPanchayat = await _context.PSZonePanchayat
                    .FirstOrDefaultAsync(p => p.PSZonePanchayatCode == updatedPSZonePanchayat.PSZonePanchayatCode
                                              && p.StateMasterId == updatedPSZonePanchayat.StateMasterId
                                              && p.DistrictMasterId == updatedPSZonePanchayat.DistrictMasterId
                                              && p.AssemblyMasterId == updatedPSZonePanchayat.AssemblyMasterId
                                              && p.ElectionTypeMasterId == updatedPSZonePanchayat.ElectionTypeMasterId
                                              && p.FourthLevelHMasterId == updatedPSZonePanchayat.FourthLevelHMasterId);

                if (existingPanchayat != null)
                {
                    return new Response
                    {
                        Status = RequestStatusEnum.BadRequest,
                        Message = "PS Zone Panchayat already exists for this code"
                    };
                }
                var existingPSZonePanchayat = await _context.PSZonePanchayat.FirstOrDefaultAsync(p => p.PSZonePanchayatMasterId == updatedPSZonePanchayat.PSZonePanchayatMasterId);

                if (existingPSZonePanchayat == null)
                {
                    return new Response { Status = RequestStatusEnum.NotFound, Message = "PS Zone Panchayat not found" };
                }

                existingPSZonePanchayat.PSZonePanchayatName = updatedPSZonePanchayat.PSZonePanchayatName;
                existingPSZonePanchayat.PSZonePanchayatCode = updatedPSZonePanchayat.PSZonePanchayatCode;
                existingPSZonePanchayat.PSZonePanchayatType = updatedPSZonePanchayat.PSZonePanchayatType;
                existingPSZonePanchayat.ElectionTypeMasterId = updatedPSZonePanchayat.ElectionTypeMasterId;
                existingPSZonePanchayat.StateMasterId = updatedPSZonePanchayat.StateMasterId;
                existingPSZonePanchayat.DistrictMasterId = updatedPSZonePanchayat.DistrictMasterId;
                existingPSZonePanchayat.AssemblyMasterId = updatedPSZonePanchayat.AssemblyMasterId;
                existingPSZonePanchayat.FourthLevelHMasterId = updatedPSZonePanchayat.FourthLevelHMasterId;
                existingPSZonePanchayat.PSZonePanchayatBooths = updatedPSZonePanchayat.PSZonePanchayatBooths;
                existingPSZonePanchayat.PSZonePanchayatCategory = updatedPSZonePanchayat.PSZonePanchayatCategory;
                existingPSZonePanchayat.PSZonePanchayatUpdatedAt = BharatDateTime();
                existingPSZonePanchayat.PSZonePanchayatDeletedAt = updatedPSZonePanchayat.PSZonePanchayatDeletedAt;
                existingPSZonePanchayat.PSZonePanchayatStatus = updatedPSZonePanchayat.PSZonePanchayatStatus;
                _context.PSZonePanchayat.Update(existingPSZonePanchayat);
                _context.SaveChanges();

                return new Response { Status = RequestStatusEnum.OK, Message = "Block Panchayat updated successfully" };
            }
            catch (Exception ex)
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = ex.Message };
            }

        }
        public async Task<PSZonePanchayat> GetPSZonePanchayatById(int stateMasterId, int districtMasterId, int assemblyMasterId, int fourthLevelHMasterId, int psZonePanchayatMasterId)
        {
            var blockPanchayat = await _context.PSZonePanchayat.Where(d => d.StateMasterId == stateMasterId && d.DistrictMasterId == districtMasterId && d.AssemblyMasterId == assemblyMasterId && d.FourthLevelHMasterId == fourthLevelHMasterId && d.PSZonePanchayatMasterId == psZonePanchayatMasterId).Include(d => d.StateMaster).Include(d => d.DistrictMaster).Include(d => d.AssemblyMaster).Include(d => d.FourthLevelH).Include(d => d.ElectionTypeMaster).FirstOrDefaultAsync();

            return blockPanchayat ?? new PSZonePanchayat(); // Return a default instance if null
        }
        public async Task<ServiceResponse> DeletePSZonePanchayatById(int psZonePanchayatMasterId)
        {
            try
            {
                var isBoothExist = await _context.BoothMaster.Where(d => d.PSZonePanchayatMasterId == psZonePanchayatMasterId).CountAsync();
                if (isBoothExist != 0)
                {
                    return new ServiceResponse { IsSucceed = false, Message = $"Booths exist under this Panchayat, kindly delete them first." };
                }
                var blockPanchayat = await _context.PSZonePanchayat
                    .FirstOrDefaultAsync(p => p.PSZonePanchayatMasterId == psZonePanchayatMasterId);
                if (blockPanchayat == null)
                {
                    return new ServiceResponse { IsSucceed = false, Message = "Block Panchayat not found" };
                }

                _context.PSZonePanchayat.Remove(blockPanchayat);
                await _context.SaveChangesAsync();

                return new ServiceResponse { IsSucceed = true, Message = "Block Panchayat deleted successfully" };
            }
            catch (Exception ex)
            {
                return new ServiceResponse { IsSucceed = false, Message = ex.Message };
            }
        }
        #endregion

        #region GPPanchayatWards
        public async Task<Response> AddGPPanchayatWards(GPPanchayatWards gpPanchayatWards)
        {
            try
            {
                var isgpPanchayatWardsExist = await _context.GPPanchayatWards.FirstOrDefaultAsync(p => p.GPPanchayatWardsCode == gpPanchayatWards.GPPanchayatWardsCode
                                                && p.StateMasterId == gpPanchayatWards.StateMasterId
                                                && p.DistrictMasterId == gpPanchayatWards.DistrictMasterId
                                                && p.AssemblyMasterId == gpPanchayatWards.AssemblyMasterId
                                                && p.ElectionTypeMasterId == gpPanchayatWards.ElectionTypeMasterId && p.FourthLevelHMasterId == gpPanchayatWards.FourthLevelHMasterId);

                if (isgpPanchayatWardsExist == null)
                {

                    gpPanchayatWards.GPPanchayatWardsCreatedAt = BharatDateTime();
                    _context.GPPanchayatWards.Add(gpPanchayatWards);
                    _context.SaveChanges();

                    return new Response { Status = RequestStatusEnum.OK, Message = gpPanchayatWards.GPPanchayatWardsName + "Added Successfully" };



                }
                else
                {
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = isgpPanchayatWardsExist.GPPanchayatWardsName + "Same Ward Already Exists in the selected Election Type" };

                }

            }

            catch (Exception ex)
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = ex.Message };
            }
        }
        public async Task<List<GPPanchayatWards>> GetPanchayatWardforRO(int stateMasterId, int districtMasterId, int assemblyMasterId, int fourthLevelHMasterId)
        {
            var getPsZone = await _context.GPPanchayatWards.Where(d => d.StateMasterId == stateMasterId && d.DistrictMasterId == districtMasterId && d.AssemblyMasterId == assemblyMasterId && d.FourthLevelHMasterId == fourthLevelHMasterId).Include(d => d.StateMaster).Include(d => d.DistrictMaster).Include(d => d.AssemblyMaster).Include(d => d.FourthLevelH).Include(d => d.ElectionTypeMaster).ToListAsync();
            if (getPsZone != null)
            {
                return getPsZone;
            }
            else
            {
                return null;
            }
        }
        public async Task<List<GPPanchayatWards>> GetPanchListById(int stateMasterId, int districtMasterId, int assemblyMasterId, int FourthLevelHMasterId, int gpPanchayatWardsMasterId)
        {
            var getPsZone = await _context.GPPanchayatWards.Where(d => d.StateMasterId == stateMasterId && d.DistrictMasterId == districtMasterId && d.AssemblyMasterId == assemblyMasterId && d.FourthLevelHMasterId == FourthLevelHMasterId && d.GPPanchayatWardsMasterId == gpPanchayatWardsMasterId).Include(d => d.StateMaster).Include(d => d.DistrictMaster).Include(d => d.AssemblyMaster).Include(d => d.FourthLevelH).Include(d => d.ElectionTypeMaster).ToListAsync();
            if (getPsZone != null)
            {
                return getPsZone;
            }
            else
            {
                return null;
            }
        }
        public async Task<List<GPPanchayatWards>> GetGPPanchayatWardsListById(int stateMasterId, int districtMasterId, int assemblyMasterId, int FourthLevelHMasterId)
        {
            var getPsZone = await _context.GPPanchayatWards.Where(d => d.StateMasterId == stateMasterId && d.DistrictMasterId == districtMasterId && d.AssemblyMasterId == assemblyMasterId && d.FourthLevelHMasterId == FourthLevelHMasterId).Include(d => d.StateMaster).Include(d => d.DistrictMaster).Include(d => d.AssemblyMaster).Include(d => d.FourthLevelH).Include(d => d.ElectionTypeMaster).ToListAsync();
            if (getPsZone != null)
            {
                return getPsZone;
            }
            else
            {
                return null;
            }
        }
        public async Task<Response> UpdateGPPanchayatWards(GPPanchayatWards gpPanchayatWards)
        {
            // Retrieve the existing entity
            var existingGPPanchayatWards = await _context.GPPanchayatWards
                .Where(d => d.GPPanchayatWardsMasterId == gpPanchayatWards.GPPanchayatWardsMasterId)
                .FirstOrDefaultAsync();

            if (existingGPPanchayatWards == null)
            {
                return new Response
                {
                    Status = RequestStatusEnum.BadRequest,
                    Message = "Wards not found."
                };
            }

            // Check if the GPPanchayatWardsCode exists in other records, excluding the current entity
            var isGPPanchayatWardsCodeExist = await _context.GPPanchayatWards
                .Where(p => p.GPPanchayatWardsCode == gpPanchayatWards.GPPanchayatWardsCode
                            && p.StateMasterId == gpPanchayatWards.StateMasterId
                            && p.DistrictMasterId == gpPanchayatWards.DistrictMasterId
                            && p.AssemblyMasterId == gpPanchayatWards.AssemblyMasterId
                            && p.ElectionTypeMasterId == gpPanchayatWards.ElectionTypeMasterId
                            && p.FourthLevelHMasterId == gpPanchayatWards.FourthLevelHMasterId
                           )
                .FirstOrDefaultAsync();

            if (isGPPanchayatWardsCodeExist != null && existingGPPanchayatWards.GPPanchayatWardsCode != isGPPanchayatWardsCodeExist.GPPanchayatWardsCode)
            {
                return new Response
                {
                    Status = RequestStatusEnum.BadRequest,
                    Message = "Wards Code already exists for this combination."
                };
            }
            // Allow update if code is the same as existing record or new
            if (isGPPanchayatWardsCodeExist == null || isGPPanchayatWardsCodeExist.GPPanchayatWardsCode == existingGPPanchayatWards.GPPanchayatWardsCode)
            {
                // Update the properties of the existing entity
                existingGPPanchayatWards.GPPanchayatWardsName = gpPanchayatWards.GPPanchayatWardsName;
                existingGPPanchayatWards.GPPanchayatWardsCode = gpPanchayatWards.GPPanchayatWardsCode;
                existingGPPanchayatWards.GPPanchayatWardsType = gpPanchayatWards.GPPanchayatWardsType;
                existingGPPanchayatWards.ElectionTypeMasterId = gpPanchayatWards.ElectionTypeMasterId;
                existingGPPanchayatWards.StateMasterId = gpPanchayatWards.StateMasterId;
                existingGPPanchayatWards.DistrictMasterId = gpPanchayatWards.DistrictMasterId;
                existingGPPanchayatWards.AssemblyMasterId = gpPanchayatWards.AssemblyMasterId;
                existingGPPanchayatWards.GPPanchayatWardsCategory = gpPanchayatWards.GPPanchayatWardsCategory;
                existingGPPanchayatWards.GPPanchayatWardsUpdatedAt = DateTime.UtcNow;
                existingGPPanchayatWards.GPPanchayatWardsDeletedAt = gpPanchayatWards.GPPanchayatWardsDeletedAt;
                existingGPPanchayatWards.GPPanchayatWardsStatus = gpPanchayatWards.GPPanchayatWardsStatus;

                // Save changes to the database
                try
                {
                    await _context.SaveChangesAsync();
                    return new Response
                    {
                        Status = RequestStatusEnum.OK,
                        Message = "Wards updated successfully."
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
            return new Response
            {
                Status = RequestStatusEnum.BadRequest,
                Message = "Already exists for this code."
            };
        }

        public async Task<GPPanchayatWards> GetGPPanchayatWardsById(int stateMasterId, int districtMasterId, int assemblyMasterId, int FourthLevelHMasterId, int gpPanchayatWardsMasterId)
        {
            var gpPanchayatWards = await _context.GPPanchayatWards.Include(d => d.StateMaster).Include(d => d.DistrictMaster).Include(d => d.AssemblyMaster).Include(d => d.FourthLevelH)
                .Include(d => d.ElectionTypeMaster)
                .Where(w => w.StateMasterId == stateMasterId &&
                            w.DistrictMasterId == districtMasterId &&
                            w.AssemblyMasterId == assemblyMasterId &&
                            w.FourthLevelHMasterId == FourthLevelHMasterId &&
                            w.GPPanchayatWardsMasterId == gpPanchayatWardsMasterId)
                .FirstOrDefaultAsync();

            if (gpPanchayatWards == null)
            {
                return null;
            }

            return gpPanchayatWards;
        }


        public async Task<Response> DeleteGPPanchayatWardsById(int gpPanchayatWardsMasterId)
        {
            var gpPanchayatWards = await _context.GPPanchayatWards
                .Where(w => w.GPPanchayatWardsMasterId == gpPanchayatWardsMasterId)
                .FirstOrDefaultAsync();

            if (gpPanchayatWards == null)
            {
                return new Response
                {
                    Status = RequestStatusEnum.BadRequest,
                    Message = "Sarpanch Ward not found."
                };
            }

            _context.GPPanchayatWards.Remove(gpPanchayatWards);

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

        #region GPVoter

        public async Task<ServiceResponse> AddGPVoterDetails(GPVoter gpVoterPdf)
        {
            var existing = await _context.GPVoter
                .Where(k => k.FourthLevelHMasterId == gpVoterPdf.FourthLevelHMasterId)
                .ToListAsync();

            if (existing.Any())
            {
                var newRange = gpVoterPdf.WardRange.Split(',').Select(int.Parse).OrderBy(x => x).ToArray();
                int newMin = newRange[0], newMax = newRange[1];

                if (existing.Any(voter =>
                {
                    var range = voter.WardRange.Split(',').Select(int.Parse).OrderBy(x => x).ToArray();
                    return !(newMax < range[0] || newMin > range[1]);
                }))
                {
                    return new ServiceResponse { IsSucceed = false, Message = "Ward Range overlaps with an existing range" };
                }
            }

            _context.GPVoter.Add(gpVoterPdf);
            await _context.SaveChangesAsync();
            return new ServiceResponse { IsSucceed = true, Message = "Successfully added" };
        }
        public async Task<ServiceResponse> UpdateGPVoterDetails(GPVoter gpVoterPdf)
        {
            var existing = await _context.GPVoter.FirstOrDefaultAsync(k => k.GPVoterMasterId == gpVoterPdf.GPVoterMasterId);

            if (existing == null)
            {
                return new ServiceResponse { IsSucceed = false, Message = "GP Voter Pdf not found." };
            }

            // Split the new ward range
            var newRange = gpVoterPdf.WardRange.Split(',').Select(int.Parse).OrderBy(x => x).ToArray();
            int newMin = newRange[0],
                newMax = newRange[1];

            if (gpVoterPdf.WardRange != existing.WardRange)
            {
                // If WardRange is the same, check for overlap with existing GPVoters
                var overlappingVoters = await _context.GPVoter
                    .Where(k => k.FourthLevelHMasterId == gpVoterPdf.FourthLevelHMasterId &&
                                k.StateMasterId == gpVoterPdf.StateMasterId &&
                                k.DistrictMasterId == gpVoterPdf.DistrictMasterId &&
                                k.AssemblyMasterId == gpVoterPdf.AssemblyMasterId)
                    .ToListAsync();

                if (overlappingVoters.Any(voter =>
                {
                    var range = voter.WardRange.Split(',').Select(int.Parse).OrderBy(x => x).ToArray();
                    return !(newMax < range[0] || newMin > range[1]); // Check for overlap
                }))
                {
                    // Return failure if there is an overlap
                    return new ServiceResponse { IsSucceed = false, Message = "Existing data overlaps with the new Ward Range." };
                }
            }

            existing.StateMasterId = gpVoterPdf.StateMasterId;
            existing.DistrictMasterId = gpVoterPdf.DistrictMasterId;
            existing.AssemblyMasterId = gpVoterPdf.AssemblyMasterId;
            existing.FourthLevelHMasterId = gpVoterPdf.FourthLevelHMasterId;
            if (!string.IsNullOrEmpty(gpVoterPdf.GPVoterPdfPath))
            {
                existing.GPVoterPdfPath = gpVoterPdf.GPVoterPdfPath;
            }
            else
            {
                existing.GPVoterPdfPath = existing.GPVoterPdfPath;
            }
            existing.WardRange = gpVoterPdf.WardRange;

            _context.GPVoter.Update(existing);
            await _context.SaveChangesAsync();

            return new ServiceResponse { IsSucceed = true, Message = "GP Voter Pdf updated successfully." };
        }

        public async Task<GPVoterList> GetGPVoterById(int gpVoterMasterId)
        {
            // Fetch the GPVoter record by ID
            var gpGPVoter = await _context.GPVoter
                .Where(w => w.GPVoterMasterId == gpVoterMasterId)
                .FirstOrDefaultAsync();

            if (gpGPVoter == null)
            {
                return null; // Return null if the GPVoter record is not found
            }

            var baseUrl = "https://lbpams.punjab.gov.in/lbpamsdoc/";

            // Fetch the related GPVoter details and return as a single GPVoterList object
            var gpVoterDetail = await _context.GPVoter
                .Where(gv => gv.GPVoterMasterId == gpVoterMasterId)
                .Join(_context.StateMaster,
                      gv => gv.StateMasterId,
                      sm => sm.StateMasterId,
                      (gv, sm) => new { GPVoter = gv, StateMaster = sm })
                .Join(_context.DistrictMaster,
                      j => j.GPVoter.DistrictMasterId,
                      dm => dm.DistrictMasterId,
                      (j, dm) => new { j.GPVoter, j.StateMaster, DistrictMaster = dm })
                .Join(_context.AssemblyMaster,
                      j => j.GPVoter.AssemblyMasterId,
                      am => am.AssemblyMasterId,
                      (j, am) => new { j.GPVoter, j.StateMaster, j.DistrictMaster, AssemblyMaster = am })
                .Join(_context.FourthLevelH,
                      j => j.GPVoter.FourthLevelHMasterId,
                      flh => flh.FourthLevelHMasterId,
                      (j, flh) => new GPVoterList
                      {
                          GPVoterMasterId = j.GPVoter.GPVoterMasterId,
                          StateMasterId = j.GPVoter.StateMasterId,
                          DistrictMasterId = j.GPVoter.DistrictMasterId,
                          AssemblyMasterId = j.GPVoter.AssemblyMasterId,
                          GPVoterPdfPath = $"{baseUrl}{j.GPVoter.GPVoterPdfPath.Replace("\\", "/")}",
                          StateName = j.StateMaster.StateName,
                          DistrictName = j.DistrictMaster.DistrictName,
                          AssemblyName = j.AssemblyMaster.AssemblyName,
                          FourthLevelHMasterId = flh.FourthLevelHMasterId,
                          FourthLevelHName = flh.HierarchyName,
                          WardRange = j.GPVoter.WardRange,
                          GPVoterStatus = j.GPVoter.GPVoterStatus
                      })
                .FirstOrDefaultAsync(); // Return a single GPVoterList object

            return gpVoterDetail;
        }


        public async Task<List<GPVoterList>> GetGPVoterListById(int stateMasterId, int districtMasterId, int assemblyMasterId)
        {
            var baseUrl = "https://lbpams.punjab.gov.in/lbpamsdoc/";

            var gpVoterList = await _context.GPVoter
                .Where(gv => gv.StateMasterId == stateMasterId &&
                             gv.DistrictMasterId == districtMasterId &&
                             gv.AssemblyMasterId == assemblyMasterId)
                .Join(_context.StateMaster,
                      gv => gv.StateMasterId,
                      sm => sm.StateMasterId,
                      (gv, sm) => new { GPVoter = gv, StateMaster = sm })
                .Join(_context.DistrictMaster,
                      j => j.GPVoter.DistrictMasterId,
                      dm => dm.DistrictMasterId,
                      (j, dm) => new { j.GPVoter, j.StateMaster, DistrictMaster = dm })
                .Join(_context.AssemblyMaster,
                      j => j.GPVoter.AssemblyMasterId,
                      am => am.AssemblyMasterId,
                      (j, am) => new { j.GPVoter, j.StateMaster, j.DistrictMaster, AssemblyMaster = am })
                .Join(_context.FourthLevelH,
                      j => j.GPVoter.FourthLevelHMasterId, // Assuming GPVoter has FourthLevelHMasterId
                      flh => flh.FourthLevelHMasterId,
                      (j, flh) => new GPVoterList
                      {
                          GPVoterMasterId = j.GPVoter.GPVoterMasterId,
                          StateMasterId = j.GPVoter.StateMasterId,
                          DistrictMasterId = j.GPVoter.DistrictMasterId,
                          AssemblyMasterId = j.GPVoter.AssemblyMasterId,
                          GPVoterPdfPath = $"{baseUrl}{j.GPVoter.GPVoterPdfPath.Replace("\\", "/")}",
                          StateName = j.StateMaster.StateName,
                          DistrictName = j.DistrictMaster.DistrictName,
                          AssemblyName = j.AssemblyMaster.AssemblyName,
                          FourthLevelHMasterId = flh.FourthLevelHMasterId,
                          FourthLevelHName = flh.HierarchyName,
                          WardRange = j.GPVoter.WardRange,
                          GPVoterStatus = j.GPVoter.GPVoterStatus
                      })
                .ToListAsync();

            return gpVoterList;
        }

        public async Task<ServiceResponse> DeleteGPVoterById(int gpVoterMasterId)
        {
            var isExist = await _context.GPVoter.Where(d => d.GPVoterMasterId == gpVoterMasterId).FirstOrDefaultAsync();
            if (isExist == null)
            {
                return new ServiceResponse { IsSucceed = false, Message = "Record not Found" };
            }
            else
            {
                _context.GPVoter.Remove(isExist);
                _context.SaveChanges();
                return new ServiceResponse { IsSucceed = true, Message = "Record Deleted successfully" };
            }
        }
        #endregion

        #region ResultDeclaration
        public async Task<ServiceResponse> AddResultDeclarationDetails(List<ResultDeclaration> resultDeclaration)
        {

            _context.ResultDeclaration.AddRange(resultDeclaration);
            _context.SaveChanges();


            return new ServiceResponse { IsSucceed = true, Message = "Successfully added" };
        }

        public async Task<Response> UpdateResultDeclarationDetails(ResultDeclaration resultDeclaration)
        {
            var existingresultDeclaration = await _context.ResultDeclaration
                .Where(d => d.ResultDeclarationMasterId == resultDeclaration.ResultDeclarationMasterId)
                .FirstOrDefaultAsync();

            if (existingresultDeclaration == null)
            {
                return new Response
                {
                    Status = RequestStatusEnum.BadRequest,
                    Message = "Result Not Declared."
                };
            }

            // Update the properties of the existing entity
            existingresultDeclaration.StateMasterId = resultDeclaration.StateMasterId;
            existingresultDeclaration.DistrictMasterId = resultDeclaration.DistrictMasterId;
            existingresultDeclaration.ElectionTypeMasterId = resultDeclaration.ElectionTypeMasterId;
            existingresultDeclaration.AssemblyMasterId = resultDeclaration.AssemblyMasterId;
            existingresultDeclaration.FourthLevelHMasterId = resultDeclaration.FourthLevelHMasterId;
            existingresultDeclaration.GPPanchayatWardsMasterId = resultDeclaration.GPPanchayatWardsMasterId;
            existingresultDeclaration.CandidateName = resultDeclaration.CandidateName;
            existingresultDeclaration.FatherName = resultDeclaration.FatherName;
            existingresultDeclaration.VoteMargin = resultDeclaration.VoteMargin;
            existingresultDeclaration.ResultDecUpdatedAt = DateTime.UtcNow;
            existingresultDeclaration.ResultDecStatus = resultDeclaration.ResultDecStatus;


            // Save changes to the database
            try
            {
                await _context.SaveChangesAsync();
                return new Response
                {
                    Status = RequestStatusEnum.OK,
                    Message = "Result Declaration updated successfully."
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
        public async Task<ResultDeclaration> GetResultDeclarationById(int resultDeclarationMasterId)
        {
            var resultDeclaration = await _context.ResultDeclaration.FirstOrDefaultAsync(d => d.ResultDeclarationMasterId == resultDeclarationMasterId);
            if (resultDeclaration == null)
            {
                return null; // or throw an exception, or handle the case appropriately
            }
            return resultDeclaration;
        }

        public async Task<ServiceResponse> DeleteResultDeclarationById(int resultDeclarationMasterId)
        {
            var isExist = await _context.ResultDeclaration.Where(d => d.ResultDeclarationMasterId == resultDeclarationMasterId).FirstOrDefaultAsync();
            if (isExist == null)
            {
                return new ServiceResponse { IsSucceed = false, Message = "Record not Found" };
            }
            else
            {
                _context.ResultDeclaration.Remove(isExist);
                _context.SaveChanges();
                return new ServiceResponse { IsSucceed = true, Message = "Record Deleted successfully" };
            }
        }
        public async Task<List<CandidateListForResultDeclaration>> GetSarpanchListById(int stateMasterId, int districtMasterId, int electionTypeMasterId, int assemblyMasterId, int fourthLevelHMasterId)
        {
            // Query Kyc Table
            var kycCandidates = await (from k in _context.Kyc
                                       where k.StateMasterId == stateMasterId &&
                                             k.DistrictMasterId == districtMasterId &&
                                             k.ElectionTypeMasterId == electionTypeMasterId &&
                                             k.AssemblyMasterId == assemblyMasterId &&
                                             k.FourthLevelHMasterId == fourthLevelHMasterId
                                       select new CandidateListForResultDeclaration
                                       {
                                           CandidateId = k.KycMasterId,
                                           CandidateName = k.CandidateName,
                                           FatherName = k.FatherName,
                                           CandidateType = CandidateTypeEnum.Kyc.ToString() // Candidate from Kyc table
                                       }).ToListAsync();

            // Query UnOpposed Table
            var unOpposedCandidates = await (from u in _context.UnOpposed
                                             where u.StateMasterId == stateMasterId &&
                                                   u.DistrictMasterId == districtMasterId &&
                                                   u.ElectionTypeMasterId == electionTypeMasterId &&
                                                   u.AssemblyMasterId == assemblyMasterId &&
                                                   u.FourthLevelHMasterId == fourthLevelHMasterId
                                             select new CandidateListForResultDeclaration
                                             {
                                                 CandidateId = u.UnOpposedMasterId,
                                                 CandidateName = u.CandidateName,
                                                 FatherName = u.FatherName,
                                                 CandidateType = CandidateTypeEnum.UnOppossed.ToString() // Candidate from UnOpposed table
                                             }).ToListAsync();

            // Combine both lists
            var combinedList = kycCandidates.Concat(unOpposedCandidates).ToList();

            return combinedList;
        }

        public async Task<List<ResultDeclarationList>> GetPanchayatWiseResults(int stateMasterId, int districtMasterId, int electionTypeMasterId, int assemblyMasterId, int fourthLevelHMasterId, int gpPanchayatWardsMasterId)
        {
            var resultList = await _context.ResultDeclaration
                .Where(rd => rd.StateMasterId == stateMasterId &&
                             rd.DistrictMasterId == districtMasterId &&
                             rd.AssemblyMasterId == assemblyMasterId &&
                             rd.FourthLevelHMasterId == fourthLevelHMasterId &&
                             rd.GPPanchayatWardsMasterId == gpPanchayatWardsMasterId)
                .Join(_context.StateMaster,
                      rd => rd.StateMasterId,
                      sm => sm.StateMasterId,
                      (rd, sm) => new { ResultDeclaration = rd, StateMaster = sm })
                .Join(_context.DistrictMaster,
                      joined => joined.ResultDeclaration.DistrictMasterId,
                      dm => dm.DistrictMasterId,
                      (joined, dm) => new { joined.ResultDeclaration, joined.StateMaster, DistrictMaster = dm })
                .Join(_context.ElectionTypeMaster,
                      joined => joined.ResultDeclaration.ElectionTypeMasterId,
                      etm => etm.ElectionTypeMasterId,
                      (joined, etm) => new { joined.ResultDeclaration, joined.StateMaster, joined.DistrictMaster, ElectionTypeMaster = etm })
                .Join(_context.AssemblyMaster,
                      joined => joined.ResultDeclaration.AssemblyMasterId,
                      am => am.AssemblyMasterId,
                      (joined, am) => new { joined.ResultDeclaration, joined.StateMaster, joined.DistrictMaster, joined.ElectionTypeMaster, AssemblyMaster = am })
                .Join(_context.FourthLevelH,
                      joined => joined.ResultDeclaration.FourthLevelHMasterId,
                      flh => flh.FourthLevelHMasterId,
                      (joined, flh) => new { joined.ResultDeclaration, joined.StateMaster, joined.DistrictMaster, joined.ElectionTypeMaster, joined.AssemblyMaster, FourthLevelH = flh })
                .Join(_context.GPPanchayatWards,
                      joined => joined.ResultDeclaration.GPPanchayatWardsMasterId,
                      gpw => gpw.GPPanchayatWardsMasterId,
                      (joined, gpw) => new ResultDeclarationList
                      {
                          ResultDeclarationMasterId = joined.ResultDeclaration.ResultDeclarationMasterId,
                          StateMasterId = joined.ResultDeclaration.StateMasterId,
                          StateName = joined.StateMaster.StateName,
                          DistrictMasterId = joined.ResultDeclaration.DistrictMasterId,
                          DistrictName = joined.DistrictMaster.DistrictName,
                          ElectionTypeMasterId = joined.ResultDeclaration.ElectionTypeMasterId,
                          ElectionType = joined.ElectionTypeMaster.ElectionType,
                          AssemblyMasterId = joined.ResultDeclaration.AssemblyMasterId,
                          AssemblyName = joined.AssemblyMaster.AssemblyName,
                          FourthLevelHMasterId = joined.ResultDeclaration.FourthLevelHMasterId,
                          FourthLevelName = joined.FourthLevelH.HierarchyName,
                          GPPanchayatWardsMasterId = joined.ResultDeclaration.GPPanchayatWardsMasterId,
                          GPPanchayatWardsName = gpw.GPPanchayatWardsName,
                          CandidateName = joined.ResultDeclaration.CandidateName,
                          FatherName = joined.ResultDeclaration.FatherName,
                          VoteMargin = joined.ResultDeclaration.VoteMargin,
                          ResultDecStatus = joined.ResultDeclaration.ResultDecStatus
                      })
                .ToListAsync();

            return resultList;
        }
        public async Task<List<ResultDeclarationList>> GetBlockWiseResults(int stateMasterId, int districtMasterId, int electionTypeMasterId, int assemblyMasterId, int fourthLevelHMasterId)
        {
            var resultList = await _context.ResultDeclaration
                .Where(rd => rd.StateMasterId == stateMasterId &&
                             rd.DistrictMasterId == districtMasterId &&
                             rd.ElectionTypeMasterId == electionTypeMasterId &&
                             rd.AssemblyMasterId == assemblyMasterId &&
                             rd.FourthLevelHMasterId == fourthLevelHMasterId)
                .Join(_context.StateMaster,
                      rd => rd.StateMasterId,
                      sm => sm.StateMasterId,
                      (rd, sm) => new { ResultDeclaration = rd, StateMaster = sm })
                .Join(_context.DistrictMaster,
                      joined => joined.ResultDeclaration.DistrictMasterId,
                      dm => dm.DistrictMasterId,
                      (joined, dm) => new { joined.ResultDeclaration, joined.StateMaster, DistrictMaster = dm })
                .Join(_context.ElectionTypeMaster,
                      joined => joined.ResultDeclaration.ElectionTypeMasterId,
                      etm => etm.ElectionTypeMasterId,
                      (joined, etm) => new { joined.ResultDeclaration, joined.StateMaster, joined.DistrictMaster, ElectionTypeMaster = etm })
                .Join(_context.AssemblyMaster,
                      joined => joined.ResultDeclaration.AssemblyMasterId,
                      am => am.AssemblyMasterId,
                      (joined, am) => new { joined.ResultDeclaration, joined.StateMaster, joined.DistrictMaster, joined.ElectionTypeMaster, AssemblyMaster = am })
                .Join(_context.FourthLevelH,
                      joined => joined.ResultDeclaration.FourthLevelHMasterId,
                      flh => flh.FourthLevelHMasterId,
                      (joined, flh) => new { joined.ResultDeclaration, joined.StateMaster, joined.DistrictMaster, joined.ElectionTypeMaster, joined.AssemblyMaster, FourthLevelH = flh })
                .Select(result => new ResultDeclarationList
                {
                    ResultDeclarationMasterId = result.ResultDeclaration.ResultDeclarationMasterId,
                    StateMasterId = result.ResultDeclaration.StateMasterId,
                    StateName = result.StateMaster.StateName,
                    DistrictMasterId = result.ResultDeclaration.DistrictMasterId,
                    DistrictName = result.DistrictMaster.DistrictName,
                    ElectionTypeMasterId = result.ResultDeclaration.ElectionTypeMasterId,
                    ElectionType = result.ElectionTypeMaster.ElectionType,
                    AssemblyMasterId = result.ResultDeclaration.AssemblyMasterId,
                    AssemblyName = result.AssemblyMaster.AssemblyName,
                    FourthLevelHMasterId = result.ResultDeclaration.FourthLevelHMasterId,
                    FourthLevelName = result.FourthLevelH.HierarchyName,
                    CandidateName = result.ResultDeclaration.CandidateName,
                    FatherName = result.ResultDeclaration.FatherName,
                    VoteMargin = result.ResultDeclaration.VoteMargin,
                    ResultDecStatus = result.ResultDeclaration.ResultDecStatus
                })
                .ToListAsync();

            return resultList;
        }

        public async Task<List<ResultDeclarationList>> GetDistrictWiseResults(int stateMasterId, int districtMasterId, int electionTypeMasterId)
        {
            var resultList = await _context.ResultDeclaration
                .Where(rd => rd.StateMasterId == stateMasterId &&
                             rd.DistrictMasterId == districtMasterId &&
                             rd.ElectionTypeMasterId == electionTypeMasterId)
                .Join(_context.StateMaster,
                      rd => rd.StateMasterId,
                      sm => sm.StateMasterId,
                      (rd, sm) => new { ResultDeclaration = rd, StateMaster = sm })
                .Join(_context.DistrictMaster,
                      joined => joined.ResultDeclaration.DistrictMasterId,
                      dm => dm.DistrictMasterId,
                      (joined, dm) => new { joined.ResultDeclaration, joined.StateMaster, DistrictMaster = dm })
                .Join(_context.ElectionTypeMaster,
                      joined => joined.ResultDeclaration.ElectionTypeMasterId,
                      etm => etm.ElectionTypeMasterId,
                      (joined, etm) => new { joined.ResultDeclaration, joined.StateMaster, joined.DistrictMaster, ElectionTypeMaster = etm })
                .Select(result => new ResultDeclarationList
                {
                    ResultDeclarationMasterId = result.ResultDeclaration.ResultDeclarationMasterId,
                    StateMasterId = result.ResultDeclaration.StateMasterId,
                    StateName = result.StateMaster.StateName,
                    DistrictMasterId = result.ResultDeclaration.DistrictMasterId,
                    DistrictName = result.DistrictMaster.DistrictName,
                    ElectionTypeMasterId = result.ResultDeclaration.ElectionTypeMasterId,
                    ElectionType = result.ElectionTypeMaster.ElectionType,
                    CandidateName = result.ResultDeclaration.CandidateName,
                    FatherName = result.ResultDeclaration.FatherName,
                    VoteMargin = result.ResultDeclaration.VoteMargin,
                    ResultDecStatus = result.ResultDeclaration.ResultDecStatus
                })
                .ToListAsync();

            return resultList;
        }
        public async Task<List<ResultDeclarationList>> GetResultDeclarationListById(int stateMasterId, int districtMasterId, int electionTypeMasterId, int assemblyMasterId, int fourthLevelHMasterId, int psZonePanchayatMasterId)
        {
            throw new NotImplementedException();
        }
        #endregion








        #region Common DateTime Methods


        /// <summary>
        /// if developer want UTC Kind Time only for month just pass month and rest fill 00000
        /// </summary>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="minutes"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        private DateTime BharatTimeDynamic(int month, int day, int hour, int minutes, int seconds)
        {
            DateTime dateTime = DateTime.UtcNow; // Use UTC time instead of DateTime.Now
            TimeSpan istOffset = TimeSpan.FromHours(5) + TimeSpan.FromMinutes(30); // IST offset
            DateTime istDateTime = dateTime + istOffset;

            if (month != 0)
            {
                istDateTime = DateTime.SpecifyKind(istDateTime.AddMonths(month), DateTimeKind.Utc);

            }
            else if (day != 0)
            {
                istDateTime = DateTime.SpecifyKind(istDateTime.AddDays(day), DateTimeKind.Utc);

            }
            else if (hour != 0)
            {
                istDateTime = DateTime.SpecifyKind(istDateTime.AddHours(hour), DateTimeKind.Utc);

            }
            else if (minutes != 0)
            {
                istDateTime = DateTime.SpecifyKind(istDateTime.AddMinutes(minutes), DateTimeKind.Utc);
            }
            else if (seconds != 0)
            {

                istDateTime = DateTime.SpecifyKind(istDateTime.AddSeconds(seconds), DateTimeKind.Utc);

            }

            return istDateTime;


        }

        #endregion

    }
}
