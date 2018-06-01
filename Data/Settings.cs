using System;

namespace Data
{
    [Serializable]
    public class Settings
    {
        public int nextUserId { get; set; }
        public int nextRoleId { get; set; }
        public int levelUpAddAttributePoint { get; set; }
    }
}
