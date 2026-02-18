using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDHiring.Core.Models
{
    public class Candidate
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? LinkedIn { get; set; }
        public string? ImagePath { get; set; }
        public int PositionId { get; set; }
        public int AgencyId { get; set; }
        public DateTime DateFound { get; set; }
        public bool WouldHire { get; set; }
        public bool Active { get; set; }
        public bool IsCurrent { get; set; }
        public string? Notes { get; set; }
        public DateTime LastUpdated { get; set; }
        public int LastUpdatedByUserId { get; set; }

        // Joined fields (populated by queries)
        public string? PositionName { get; set; }
        public string? AgencyName { get; set; }
        public string? LastUpdatedByName { get; set; }
    }
}
