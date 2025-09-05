using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMS_ACore.ServiceModels
{
    public class PanchyatMappingResponseList
    {
        
            public int Id
            {
                get;
                set;
            }

            public string? HierarchyName { get; set; }
            public int HierarchyCode { get; set; }

            public string? HierarchyType { get; set; }

            public string? Type
            {
                get;
                set;
            }

            public int ElectionTypeMasterId
            {
                get;
                set;
            }


            public int StateMasterId
            {
                get;
                set;
            }


            public int DistrictMasterId
            {
                get;
                set;
            }


            public int AssemblyMasterId
            {
                get;
                set;
            }

            public int FourthLevelHMasterId
            {
                get;
                set;
            }

            public int PSZonePanchayatMasterId
            {
                get;
                set;
            }

            public bool Status { get; set; }



         
    }
}
