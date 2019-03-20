using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace UsedGoodApp.Models
{
    public class UserView
    {
        [Display(Name = "Идентификатор")]
        [HiddenInput]
        public string Id { get; set; }

        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }

        [Display(Name = "Электронная почта")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Неверный формат электронной почты")]
        public string Email { get; set; }

        [Display(Name = "Номер")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Display(Name = "Роль")]
        public string SelectedRole { get; set; }
        //public IEnumerable<string> Roles { get; set; }
    }

    public class EditableUserView
    {
        [HiddenInput]
        public string Id { get; set; }

        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }

        [Display(Name = "Электронная почта")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Электронная почта обязательна к заполнению")]
        public string Email { get; set; }

        [Display(Name = "Номер")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Неверный формат телефона")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Пароль")]
        [DataType(DataType.Password, ErrorMessage = "Неверно указан пароль")]
        public string Password { get; set; }

        [Display(Name = "Подтвердите пароль")]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Роль")]
        public string SelectedRole { get; set; }
       
        public IEnumerable<SelectListItem> Roles { get; set; }
    }

    public class CreateUserView : EditableUserView
    {
        [Display(Name = "Электронная почта")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Электронная почта обязательна к заполнению")]
        public new string Email { get; set; }

        [Required(ErrorMessage = "Пароль обязателен к заполнению")]
        [DataType(DataType.Password, ErrorMessage = "Неверно указан пароль")]
        public new string Password { get; set; }
    }
}