using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskGauge.Common.Enum
{
    public enum UserSecurityQuestion
    {
        [Display(Name = "What is your favorite color?")]
        WhatIsYourFavoriteColor,

        [Display(Name = "What is the name of your childhood friend?")]
        WhatIsTheNameOfYourChildhoodFriend
    }
}
