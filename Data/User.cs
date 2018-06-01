using System;

namespace Data
{
    [Serializable]
    public class User
    {
        public int userId { get; set; }
        public string accountNumber { get; set; }
        public string password { get; set; }
        public int roleId { get; set; }
    }
}
