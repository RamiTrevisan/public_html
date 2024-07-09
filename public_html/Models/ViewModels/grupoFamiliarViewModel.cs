using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace webCentenario.Models.ViewModels
{
    public class grupoFamiliarViewModel
    {
        public int gfID { get; set; }
        public int gfSocioID { get; set; }
        public string gfSocioPrincipal { get; set; }

        [Display(Name = "Observaciones")]
        public string gfObservacion { get; set; }

        [Display(Name = "Socio principal")]
        [Required(ErrorMessage = "Debe seleccionar un socio principal.")]
        public int selPrincipal { get; set; }

        [Display(Name = "Socio")]
        [Required(ErrorMessage = "Debe seleccionar un socio.")]
        public int selIntegrante { get; set; }
        public List<SelectListItem> sociosList { get; set; }

        [Display(Name = "Parentesco")]
        [Required(ErrorMessage = "Debe seleccionar un parentesco.")]
        public int selParentesco { get; set; }
        public List<SelectListItem> parentescoList { get; set; }

        public ICollection<integranteGrupoFamiliarViewModel> gfIntegrantesList { get; set; }
    }

    public class integranteGrupoFamiliarViewModel
    {
        public int igfID { get; set; }
        public int igfIntegranteID { get; set; }
        public string igfNombre { get; set; }
        public string igfCategoria { get; set; }
        public string igfNumDoc { get; set; }
        public int igfParentescoID { get; set; }
        public string igfParentesco { get; set; }
        //public bool igfEstado { get; set; }
    }
}