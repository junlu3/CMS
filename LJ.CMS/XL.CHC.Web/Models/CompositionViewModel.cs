using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XL.CHC.Web.Models
{
    public class CompositionViewModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "组分中文名不能为空")]
        public string Composition_Name { get; set; }
        [Required(ErrorMessage = "CAS 号不能为空")]
        //[RegularExpression(@"\d{1,}-\d{1,}-\d{1,}", ErrorMessage = "CAS 号格式错误。 e.g. 00-00-00 ")]
        public string CASCode { get; set; }
    }
}