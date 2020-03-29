using System.ComponentModel.DataAnnotations;

namespace ModelLayer.DataTransferObjects
{
    public class CredentialsDto
    {
        [Required(AllowEmptyStrings = false)]
        public string UserNameOrEmail { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; }
    }
}