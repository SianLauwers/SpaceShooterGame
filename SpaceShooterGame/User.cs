using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceShooterGame
{
    public class User
    {
        [Key]
        public int Id { get; set; } 
        public int HighScore { get; set; }
    }
}