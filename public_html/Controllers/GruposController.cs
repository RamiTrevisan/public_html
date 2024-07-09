using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using webCentenario.Filters;
using webCentenario.Models;
using webCentenario.Models.ViewModels;

namespace webCentenario.Controllers
{
    public class GruposController : Controller
    {

        private webCentenarioEntities db = new webCentenarioEntities();

        #region --> Datatables
        [HttpGet]
        [AuthenticationFilter]
        [AuthorizationFilter("Cobrador", "Secretaria", "Administrador")]
        public ActionResult Listado()
        {
            return View();
        }

        [HttpPost]
        [AuthenticationFilter]
        [AuthorizationFilter("Cobrador", "Secretaria", "Administrador")]
        public ActionResult CargarGrupos()
        {
            try
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                var grupoData = from gf in db.grupoFamiliarData
                                join socio in db.socioData on gf.gfPrincipalID equals socio.socioID
                                select new grupoFamiliarViewModel
                                {
                                    gfID = gf.gfID,
                                    gfSocioPrincipal = socio.socioApellido + " " + socio.socioNombre,
                                    gfObservacion = gf.gfObservacion != null ? gf.gfObservacion : "Sin descripción"
                                };

                if (!string.IsNullOrEmpty(searchValue))
                {
                    grupoData = grupoData.Where(m => m.gfSocioPrincipal.ToLower().Contains(searchValue.ToLower()) || m.gfObservacion.ToLower().Contains(searchValue.ToLower()));
                }

                recordsTotal = grupoData.Count();
                var data = grupoData.OrderBy(s => s.gfID).Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [AuthenticationFilter]
        [AuthorizationFilter("Cobrador", "Secretaria", "Administrador")]
        public ActionResult Detalles(int? id)
        {
            var grupo = (from gf in db.grupoFamiliarData
                         where gf.gfID == id
                         //join igf in db.integranteGrupoFamiliarData on gf.gfID equals igf.igfGrupoID
                         join socio in db.socioData on gf.gfPrincipalID equals socio.socioID
                         select new grupoFamiliarViewModel
                         {
                             gfID = gf.gfID,
                             gfSocioID = socio.socioID,
                             gfSocioPrincipal = socio.socioApellido + " " + socio.socioNombre,
                             gfObservacion = gf.gfObservacion
                         }).FirstOrDefault();

            if (grupo == null)
                return new HttpStatusCodeResult(HttpStatusCode.NoContent);

            grupoFamiliarViewModel model = new grupoFamiliarViewModel()
            {
                gfID = grupo.gfID,
                gfSocioID = grupo.gfSocioID,
                gfSocioPrincipal = grupo.gfSocioPrincipal,
                gfObservacion = grupo.gfObservacion
            };
            return View(model);
        }

        [HttpPost]
        [AuthenticationFilter]
        [AuthorizationFilter("Cobrador", "Secretaria", "Administrador")]
        public ActionResult CargarIntegrantes(int id)
        {
            try
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                var integrantesData = from igf in db.grupoFamiliarIntegranteData
                                   where igf.igfGrupoID == id
                                   join gf in db.grupoFamiliarData on igf.igfGrupoID equals gf.gfID
                                   join pgf in db.grupoFamiliarParentescoData on igf.igfParentescoID equals pgf.pgfID
                                   join socio in db.socioData on igf.igfIntegranteID equals socio.socioID
                                   join cat in db.categoriaSocioData on socio.socioCategoriaID equals cat.catID
                                   select new integranteGrupoFamiliarViewModel
                                   {
                                       igfID = igf.igfID,
                                       igfIntegranteID = socio.socioID,
                                       igfNombre = socio.socioApellido + " " + socio.socioNombre,
                                       igfCategoria = cat.catNombre,
                                       igfNumDoc = socio.socioNumDoc.ToString(),
                                       igfParentesco = pgf.pgfNombre
                                   };
                
                if (!string.IsNullOrEmpty(searchValue))
                {
                    integrantesData = integrantesData.Where(igf => igf.igfNombre.ToLower().Contains(searchValue.ToLower()) || igf.igfCategoria.ToLower().Contains(searchValue.ToLower()) || igf.igfNumDoc.ToLower().Contains(searchValue.ToLower()));
                }

                recordsTotal = integrantesData.Count();
                var data = integrantesData.OrderBy(igf => igf.igfID).Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [AuthenticationFilter]
        [AuthorizationFilter("Cobrador", "Secretaria", "Administrador")]
        public ActionResult CargarDeudas(int id)
        {
            try
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                
                var cuotasData = (from cuota in db.cuotaSocioData
                                 join gf in db.grupoFamiliarData on id equals gf.gfID
                                 join igf in db.grupoFamiliarIntegranteData on gf.gfID equals igf.igfGrupoID into igfJoin
                                 from integrante in igfJoin.DefaultIfEmpty()
                                 join cat in db.categoriaSocioData on cuota.csCategoriaID equals cat.catID
                                 from socio in db.socioData
                                 where socio.socioID == gf.gfPrincipalID || socio.socioID == integrante.igfIntegranteID
                                 where cuota.csSocioID == socio.socioID && cuota.csEstadoID == 1
                                 select new cuotaSocioViewModel
                                 {
                                     csID = cuota.csID,
                                     socioNombreCompleto = socio.socioApellido + " " + socio.socioNombre,
                                     csCategoriaNombre = cat.catNombre,
                                     csMonto = cuota.csMonto,
                                     csFechaPeriodo = cuota.csFechaPeriodo,
                                     csFechaVencimiento = cuota.csFechaVencimiento
                                 }).Distinct();

                if (!string.IsNullOrEmpty(searchValue))
                {
                    cuotasData = cuotasData.Where(m => m.socioNombreCompleto.ToLower().Contains(searchValue.ToLower()) || m.csCategoriaNombre.ToLower().Contains(searchValue.ToLower()));
                }

                recordsTotal = cuotasData.Count();
                var data = cuotasData.OrderBy(cs => cs.csID).Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [AuthenticationFilter]
        [AuthorizationFilter("Secretaria", "Administrador")]
        public ActionResult CargarHistorial(int id)
        {
            try
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                var cuotasData = (from cuota in db.cuotaSocioData
                                 join gf in db.grupoFamiliarData on id equals gf.gfID
                                 join igf in db.grupoFamiliarIntegranteData on gf.gfID equals igf.igfGrupoID into igfJoin
                                 from integrante in igfJoin.DefaultIfEmpty()
                                 join cat in db.categoriaSocioData on cuota.csCategoriaID equals cat.catID
                                 from socio in db.socioData
                                 where socio.socioID == gf.gfPrincipalID || socio.socioID == integrante.igfIntegranteID
                                 where cuota.csSocioID == socio.socioID && (cuota.csEstadoID == 2 || cuota.csEstadoID == 3)
                                 select new cuotaSocioViewModel
                                 {
                                     csID = cuota.csID,
                                     socioNombreCompleto = socio.socioApellido + " " + socio.socioNombre,
                                     csCategoriaNombre = cat.catNombre,
                                     csMonto = cuota.csMonto,
                                     csFechaPeriodo = cuota.csFechaPeriodo,
                                     csFechaPago = cuota.csFechaPago
                                 }).Distinct();

                if (!string.IsNullOrEmpty(searchValue))
                {
                    cuotasData = cuotasData.Where(m => m.socioNombreCompleto.ToLower().Contains(searchValue.ToLower()) || m.csCategoriaNombre.ToLower().Contains(searchValue.ToLower()));
                }

                recordsTotal = cuotasData.Count();
                var data = cuotasData.OrderByDescending(cs => cs.csID).Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        [HttpGet]
        [AuthenticationFilter]
        [AuthorizationFilter("Secretaria", "Administrador")]
        public ActionResult Crear()
        {
            grupoFamiliarViewModel model = new grupoFamiliarViewModel();
            model.sociosList = ListarSocios();
            return View(model);
        }

        [HttpPost]
        [AuthenticationFilter]
        [ValidateAntiForgeryToken]
        [AuthorizationFilter("Secretaria", "Administrador")]
        public async Task<ActionResult> Crear(grupoFamiliarViewModel model)
        {
            model.sociosList = ListarSocios();

            if (!ModelState.IsValid)
                return View(model);

            grupoFamiliarData grupoDTO = new grupoFamiliarData()
            {
                gfPrincipalID = model.selPrincipal,
                gfObservacion = model.gfObservacion,
                gfFechaCreacion = DateTime.Now,
            };
            db.grupoFamiliarData.Add(grupoDTO);
            await db.SaveChangesAsync();

            return RedirectToAction("Integrantes", "Grupos", new { id = grupoDTO.gfID });
        }

        [HttpGet]
        [AuthenticationFilter]
        [AuthorizationFilter("Secretaria", "Administrador")]
        public async Task<ActionResult> Modificar(int? id)
        {
            grupoFamiliarData grupo = await db.grupoFamiliarData.FindAsync(id);

            if (grupo == null)
                throw new HttpException((int)HttpStatusCode.BadRequest, "La consulta no devuelve contenido.");

            grupoFamiliarViewModel model = new grupoFamiliarViewModel()
            {
                gfID = grupo.gfID,
                sociosList = ListarSocios(),
                selPrincipal = grupo.gfPrincipalID,
                gfObservacion = grupo.gfObservacion
            };

            var socio = db.socioData.Where(s => s.socioID == grupo.gfPrincipalID).FirstOrDefault();

            model.sociosList.Add(new SelectListItem()
            {
                Text = socio.socioApellido + " " + socio.socioNombre,
                Value = socio.socioID.ToString()
            });

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizationFilter("Secretaria", "Administrador")]
        public async Task<ActionResult> Modificar(grupoFamiliarViewModel model)
        {
            var socio = db.socioData.Where(s => s.socioID == model.selPrincipal).FirstOrDefault();

            model.sociosList = ListarSocios();
            model.sociosList.Add(new SelectListItem()
            {
                Text = socio.socioApellido + " " + socio.socioNombre,
                Value = socio.socioID.ToString()
            });

            if (!ModelState.IsValid)
                return View(model);

            grupoFamiliarData grupoFamiliarData = db.grupoFamiliarData.FirstOrDefault(g => g.gfID == model.gfID);

            grupoFamiliarData.gfPrincipalID = model.selPrincipal;
            grupoFamiliarData.gfObservacion = model.gfObservacion;

            db.Entry(grupoFamiliarData).State = EntityState.Modified;
            await db.SaveChangesAsync();

            string message = "Modificaste la información del grupo familiar de <b>" + socio.socioApellido + " " + socio.socioNombre + "</b> con éxito.";
            TempData["Info"] = message;

            return RedirectToAction("Detalles", "Grupos", new { id = model.gfID });
        }

        [HttpGet]
        [AuthenticationFilter]
        [AuthorizationFilter("Secretaria", "Administrador")]
        public ActionResult Integrantes(int id)
        {
            var grupo = (from gf in db.grupoFamiliarData
                         where gf.gfID == id
                         join socio in db.socioData on gf.gfPrincipalID equals socio.socioID
                         select new grupoFamiliarViewModel
                         {
                             gfID = gf.gfID,
                             gfSocioPrincipal = socio.socioApellido + " " + socio.socioNombre
                         }).FirstOrDefault();

            if (grupo == null)
                return RedirectToAction("Listado");

            grupoFamiliarViewModel model = new grupoFamiliarViewModel()
            {
                gfID = grupo.gfID,
                gfSocioPrincipal = grupo.gfSocioPrincipal,
                parentescoList = ListarRoles()
            };

            return View(model);
        }

        [HttpPost]
        [AuthenticationFilter]
        [ValidateAntiForgeryToken]
        [AuthorizationFilter("Secretaria", "Administrador")]
        public async Task<ActionResult> Integrantes(grupoFamiliarViewModel model)
        {
            var socioData = db.socioData.Where(x => x.socioEstado);
            model.parentescoList = ListarRoles();

            if (!ModelState.IsValid)
                return View(model);

            //if (socioData = true)
            //{
                
            //}


            grupoFamiliarIntegranteData igfDTO = new grupoFamiliarIntegranteData()
            {
                igfGrupoID = model.gfID,
                igfIntegranteID = model.selIntegrante,
                igfParentescoID = model.selParentesco
            };
            db.grupoFamiliarIntegranteData.Add(igfDTO);
            await db.SaveChangesAsync();
            ModelState.Clear();
            return View(new grupoFamiliarViewModel() { gfID = model.gfID, gfSocioPrincipal = model.gfSocioPrincipal, parentescoList = ListarRoles() });
        }

        [HttpPost]
        [AuthenticationFilter]
        [AuthorizationFilter("Secretaria", "Administrador")]
        public async Task<JsonResult> EliminarIntegrante(int id)
        {
            grupoFamiliarIntegranteData igfID = await db.grupoFamiliarIntegranteData.FindAsync(id);

            if (igfID == null)
                return Json(false, JsonRequestBehavior.AllowGet);

            db.grupoFamiliarIntegranteData.Remove(igfID);
            await db.SaveChangesAsync();

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AuthenticationFilter]
        [AuthorizationFilter("Secretaria", "Administrador")]
        public async Task<JsonResult> Eliminar(int id)
        {
            grupoFamiliarData grupoFamiliarData = await db.grupoFamiliarData.FindAsync(id);

            if (grupoFamiliarData == null)
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);

            try
            {
                db.grupoFamiliarData.Remove(grupoFamiliarData);
                await db.SaveChangesAsync();

                var socioData = db.socioData.Find(grupoFamiliarData.gfPrincipalID);

                string message = "Se eliminó el grupo familiar de <b>" + socioData.socioApellido + " " + socioData.socioNombre + "</b> con éxito.";
                return Json(new { result = true, message }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }
        }

        #region --> Listados
        [HttpPost]
        public JsonResult ObtenerSocios()
        {
            return Json(ListarSocios(), JsonRequestBehavior.AllowGet);
        }

        public List<SelectListItem> ListarSocios()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            using (var db = new webCentenarioEntities())
            {
                var socios = (from socio in db.socioData
                               where !(db.grupoFamiliarIntegranteData.Any(igf => igf.igfIntegranteID == socio.socioID) || db.grupoFamiliarData.Any(gf => gf.gfPrincipalID == socio.socioID)) && socio.socioEstado == true
                               select socio).ToList(); 

                foreach (var item in socios)
                {
                    items.Add(new SelectListItem()
                    {
                        Text = item.socioNombre + " " + item.socioApellido,
                        Value = item.socioID.ToString()
                    });
                }
            }
            return items;
        }

        private static List<SelectListItem> ListarRoles()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            using (var db = new webCentenarioEntities())
            {
                var roles = db.grupoFamiliarParentescoData.ToList();
                foreach (var item in roles)
                    items.Add(new SelectListItem()
                    {
                        Text = item.pgfNombre,
                        Value = item.pgfID.ToString()
                    });
            }
            return items;
        }
        #endregion
    }
}
