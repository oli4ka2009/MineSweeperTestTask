using System.ComponentModel.DataAnnotations;

namespace MineSweeper.Models.ViewModels
{
    public class NewGameViewModel
    {
        [Required(ErrorMessage = "Будь ласка, введіть ваше ім'я")]
        [Display(Name = "Ім'я гравця")]
        public string PlayerName { get; set; }

        [Required]
        [Range(5, 30, ErrorMessage = "Ширина поля має бути від 5 до 30")]
        [Display(Name = "Ширина поля")]
        public int Width { get; set; }

        [Required]
        [Range(5, 30, ErrorMessage = "Висота поля має бути від 5 до 30")]
        [Display(Name = "Висота поля")]
        public int Height { get; set; }

        [Required]
        [Range(1, 200, ErrorMessage = "Кількість мін має бути від 1 до 200")]
        [Display(Name = "Кількість мін")]
        public int Mines { get; set; }
    }
}
