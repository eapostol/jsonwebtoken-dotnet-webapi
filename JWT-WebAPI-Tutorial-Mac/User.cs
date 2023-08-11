using System;
namespace JWT_WebAPI_Tutorial_Mac
{
	public class User
	{
		public string Username { get; set; } = string.Empty;
		public byte[]? PasswordHash { get; set; } = null;
        public byte[]? PasswordSalt { get; set; } = null;



        public User()
		{
		}
	}
}

