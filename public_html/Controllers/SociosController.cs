using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using webCentenario.Models;
using webCentenario.Models.ViewModels;
using webCentenario.Filters;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System.ComponentModel;

namespace webCentenario.Controllers
{
    public class SociosController : Controller
    {
        private webCentenarioEntities db = new webCentenarioEntities();

        [HttpGet]
        [AuthenticationFilter]
        [AuthorizationFilter("Secretaria", "Administrador")]
        public ActionResult Listado()
        {
            int usuariosTotales = db.socioData.Where(x => x.socioEstado == true).Count();
            ViewBag.usersTotales = usuariosTotales;
            bajaViewModel model = new bajaViewModel();
            model.causaBajaList = ListarCausaBaja();
            return View(model);
        }

        [HttpPost]
        [AuthenticationFilter]
        [AuthorizationFilter("Secretaria", "Administrador")]
        public ActionResult CargarSocios()
        {

            try
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                var socioData = from socio in db.socioData
                                join cat in db.categoriaSocioData on socio.socioCategoriaID equals cat.catID
                                join doc in db.tipoDocumentoData on socio.socioTipoDoc equals doc.tdID
                                join zc in db.zonaCobroData on socio.socioZonaCobroID equals zc.zcID
                                where socio.socioEstado == true
                                select new socioViewModel
                                {
                                    socioID = socio.socioID,
                                    socioNombreCompleto = socio.socioApellido + " " + socio.socioNombre,
                                    socioDocumento = doc.tdNombre + " " + socio.socioNumDoc.ToString(),
                                    socioCategoriaNombre = cat.catNombre,
                                    socioZonaCobro = zc.zcNombre
                                };

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    if (sortColumn == "socioID")
                    {
                        socioData = sortColumnDir == "asc" ? socioData.OrderBy(s => s.socioID) : socioData.OrderByDescending(s => s.socioID);
                    }
                    else if (sortColumn == "socioNombreCompleto")
                    {
                        socioData = sortColumnDir == "asc" ? socioData.OrderBy(s => s.socioNombreCompleto) : socioData.OrderByDescending(s => s.socioNombreCompleto);
                    }
                }
                else
                {
                    socioData = socioData.OrderByDescending(s => s.socioID);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    socioData = socioData.Where(m => m.socioID.ToString().ToLower().Contains(searchValue.ToLower()) || m.socioNombreCompleto.ToLower().Contains(searchValue.ToLower()) || m.socioDocumento.ToLower().Contains(searchValue.ToLower()) || m.socioCategoriaNombre.ToLower().Contains(searchValue.ToLower()) || m.socioZonaCobro.ToLower().Contains(searchValue.ToLower()));
                }

                recordsTotal = socioData.Count();
                var data = socioData.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpGet]
        [AuthenticationFilter]
        [AuthorizationFilter("Secretaria", "Administrador")]
        public ActionResult Bajas()
        {
            int usuariosTotalesBaja = db.socioData.Where(z => z.socioEstado == false).Count();
            ViewBag.usersTotalesBaja = usuariosTotalesBaja;
            return View();
        }

        [HttpPost]
        [AuthenticationFilter]
        [AuthorizationFilter("Secretaria", "Administrador")]
        public ActionResult CargarBajas()
        {
            try
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                var socioData = from socio in db.socioData
                                join cat in db.categoriaSocioData on socio.socioCategoriaID equals cat.catID
                                join doc in db.tipoDocumentoData on socio.socioTipoDoc equals doc.tdID
                                join cb in db.causaBajaData on socio.socioCausaBajaID equals cb.cbID
                                where socio.socioEstado == false
                                select new socioViewModel
                                {
                                    socioID = socio.socioID,
                                    socioNombreCompleto = socio.socioApellido + " " + socio.socioNombre,
                                    socioDocumento = doc.tdNombre + " " + socio.socioNumDoc.ToString(),
                                    socioCausaBaja = cb.cbNombre,
                                    socioFechaBaja = socio.socioFechaBaja
                                };

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    if (sortColumn == "socioID")
                    {
                        socioData = sortColumnDir == "asc" ? socioData.OrderBy(s => s.socioID) : socioData.OrderByDescending(s => s.socioID);
                    }
                    else if (sortColumn == "socioNombreCompleto")
                    {
                        socioData = sortColumnDir == "asc" ? socioData.OrderBy(s => s.socioNombreCompleto) : socioData.OrderByDescending(s => s.socioNombreCompleto);
                    }
                    else if (sortColumn == "socioFechaBaja")
                    {
                        socioData = sortColumnDir == "asc" ? socioData.OrderBy(s => s.socioFechaBaja) : socioData.OrderByDescending(s => s.socioFechaBaja);
                    }
                }
                else
                {
                    socioData = socioData.OrderByDescending(s => s.socioID);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    socioData = socioData.Where(m => m.socioID.ToString().ToLower().Contains(searchValue.ToLower()) || m.socioNombreCompleto.ToLower().Contains(searchValue.ToLower()) || m.socioDocumento.ToLower().Contains(searchValue.ToLower()) || m.socioCausaBaja.ToLower().Contains(searchValue.ToLower()));
                }

                recordsTotal = socioData.Count();
                var data = socioData.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpGet]
        [AuthenticationFilter]
        [AuthorizationFilter("Secretaria", "Administrador")]
        public async Task<ActionResult> Detalles(int? id)
        {
            socioData socioData = await db.socioData.FindAsync(id);

            if(socioData == null)
                throw new HttpException((int)HttpStatusCode.BadRequest, "La consulta no devuelve contenido.");

            var cat = await db.categoriaSocioData.FindAsync(socioData.socioCategoriaID);
            var documento = await db.tipoDocumentoData.FindAsync(socioData.socioTipoDoc);
            var sex = await db.sexoData.FindAsync(socioData.socioSexoID);
            var zc = await db.zonaCobroData.FindAsync(socioData.socioZonaCobroID);
            var loc = await db.locData.FindAsync(socioData.socioLocalidadID);
            var prov = await db.provData.FindAsync(loc.locProvID);

            socioViewModel socio = new socioViewModel()
            {
                socioID = socioData.socioID,
                socioNombre = socioData.socioNombre,
                socioApellido = socioData.socioApellido,
                socioCategoriaNombre = cat.catNombre,
                socioDocumento = documento.tdNombre + " " + socioData.socioNumDoc,
                socioFechaNac = socioData.socioFechaNacimiento,
                socioSexo = sex.sexNombre,
                socioDireccion = socioData.socioDireccion,
                socioLocalidad = loc.locNombre,
                socioProvincia = prov.provNombre,
                socioTelefono = socioData.socioTelefono
            };
            return View(socio);
        }

        [HttpPost]
        [AuthenticationFilter]
        [AuthorizationFilter("Secretaria", "Administrador")]
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

                var cuotasData = from cuota in db.cuotaSocioData
                                  where cuota.csSocioID == id
                                  join socio in db.socioData on cuota.csSocioID equals socio.socioID
                                  join cat in db.categoriaSocioData on cuota.csCategoriaID equals cat.catID
                                  where cuota.csEstadoID == 1
                                  select new cuotaSocioViewModel
                                  {
                                      csID = cuota.csID,
                                      socioNombreCompleto = socio.socioApellido + " " + socio.socioNombre,
                                      csCategoriaNombre = cat.catNombre,
                                      csMonto = cuota.csMonto,
                                      csFechaPeriodo = cuota.csFechaPeriodo,
                                      csFechaVencimiento = cuota.csFechaVencimiento
                                  };

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

                var cuotasData = from cuota in db.cuotaSocioData
                                 where cuota.csSocioID == id
                                 join socio in db.socioData on cuota.csSocioID equals socio.socioID
                                 join cat in db.categoriaSocioData on cuota.csCategoriaID equals cat.catID
                                 where cuota.csEstadoID == 2 || cuota.csEstadoID == 3
                                 select new cuotaSocioViewModel
                                 {
                                     csID = cuota.csID,
                                     socioNombreCompleto = socio.socioApellido + " " + socio.socioNombre,
                                     csCategoriaNombre = cat.catNombre,
                                     csMonto = cuota.csMonto,
                                     csFechaPeriodo = cuota.csFechaPeriodo,
                                     csFechaPago = cuota.csFechaPago
                                 };

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

        [HttpGet]
        [AuthenticationFilter]
        [AuthorizationFilter("Secretaria", "Administrador")]
        public ActionResult Crear()
        {
            socioEditViewModel socio = new socioEditViewModel();
            socio.provList = ListarProvincias();
            socio.locList = ListarLocalidades(null);
            socio.sexList = ListarSexos();
            socio.zonaCobroList = ListarZonasCobro();
            socio.tipoDocList = ListarTiposDoc();
            socio.selProv = 22;
            socio.selLoc = 2132;
            socio.selTipoDoc = 1;
            return View(socio);
        }

        [HttpPost]
        [AuthenticationFilter]
        [ValidateAntiForgeryToken]
        [AuthorizationFilter("Secretaria", "Administrador")]
        public async Task<ActionResult> Crear(socioEditViewModel model)
        {
            model.provList = ListarProvincias();
            model.locList = ListarLocalidades(null);
            model.sexList = ListarSexos();
            model.zonaCobroList = ListarZonasCobro();
            model.tipoDocList = ListarTiposDoc();

            if (!ModelState.IsValid)
                return View(model);

            if (db.socioData.Where(s => s.socioNumDoc == model.socioNumDoc).Any())
            {
                ModelState.AddModelError("socioNumDoc", "El número de DNI ya se encuentra registrado.");
                return View(model);
            }

            if (model.socioFechaNacimiento.Year < 1900 || model.socioFechaNacimiento == null)
            {
                ModelState.AddModelError("socioFechaNacimiento", "La fecha de nacimiento es incorrecta.");
                return View(model);
            }

            int edad = DateTime.Now.Year - model.socioFechaNacimiento.Year;
            if (model.socioFechaNacimiento.Month > DateTime.Now.Month) edad--;

            var categoria = await db.categoriaSocioData.Where(cat => cat.catEdadMax > edad).FirstOrDefaultAsync();

            socioData socioDTO = new socioData()
            {
                socioNombre = model.socioNombre.ToUpper(),
                socioApellido = model.socioApellido.ToUpper(),
                socioDireccion = model.socioDireccion.ToUpper(),
                socioTelefono = model.socioTelefono,
                socioEmail = model.socioEmail,
                socioLocalidadID = model.selLoc,
                socioTipoDoc = model.selTipoDoc,
                socioNumDoc = model.socioNumDoc,
                socioFechaNacimiento = model.socioFechaNacimiento,
                socioSexoID = model.selSex,
                socioZonaCobroID = model.selZonaCobro,
                socioCategoriaID = categoria.catID,
                socioEstado = true,
                socioFechaIngreso = DateTime.Now,
            };
            db.socioData.Add(socioDTO);
            await db.SaveChangesAsync();

            string message = "Diste de alta un nuevo socio (#<b>" + socioDTO.socioID + "</b>)." +
                "</br></br><b>Nombre:</b> " + socioDTO.socioApellido + " " + socioDTO.socioNombre +
                "</br><b>Categoría:</b> " + categoria.catNombre;

            ViewBag.Message = message;
            ViewBag.SocioID = socioDTO.socioID;

            Auditoria((int)Session["Session"], 2, 1, "Dió de alta al socio " + socioDTO.socioApellido + " " + socioDTO.socioNombre + " - DNI N°: " + socioDTO.socioNumDoc + " (#" + socioDTO.socioID + ")");

            ModelState.Clear();
            return View(new socioEditViewModel() { provList = ListarProvincias(), locList = ListarLocalidades(null), sexList = ListarSexos(), zonaCobroList = ListarZonasCobro(), tipoDocList = ListarTiposDoc() });
        }

        [HttpGet]
        [AuthenticationFilter]
        [AuthorizationFilter("Secretaria", "Administrador")]
        public async Task<ActionResult> Modificar(int id)
        {
            socioData socioData = await db.socioData.FindAsync(id);

            if (socioData == null)
                throw new HttpException((int)HttpStatusCode.BadRequest, "La consulta no devuelve contenido.");

            var provID = db.locData.Where(l => l.locID == socioData.socioLocalidadID).Select(l => l.locProvID).FirstOrDefault();
            socioEditViewModel socio = new socioEditViewModel()
            {
                socioID = socioData.socioID,
                socioNombre = socioData.socioNombre,
                socioApellido = socioData.socioApellido,
                socioDireccion = socioData.socioDireccion,
                socioTelefono = socioData.socioTelefono,
                socioEmail = socioData.socioEmail,
                tipoDocList = ListarTiposDoc(),
                selTipoDoc = socioData.socioTipoDoc,
                socioNumDoc = socioData.socioNumDoc,
                socioFechaNacimiento = socioData.socioFechaNacimiento,
                locList = ListarLocalidades(null),
                selLoc = socioData.socioLocalidadID,
                provList = ListarProvincias(),
                selProv = provID,
                sexList = ListarSexos(),
                selSex = socioData.socioSexoID,
                zonaCobroList = ListarZonasCobro(),
                catList = ListarCategorias(),
                selCat = socioData.socioCategoriaID,
                socioEstado = socioData.socioEstado
            };

            return View(socio);
        }

        [HttpPost]
        [AuthenticationFilter]
        [ValidateAntiForgeryToken]
        [AuthorizationFilter("Secretaria", "Administrador")]
        public async Task<ActionResult> Modificar(socioEditViewModel model)
        {
            model.tipoDocList = ListarTiposDoc();
            model.locList = ListarLocalidades(null);
            model.provList = ListarProvincias();
            model.sexList = ListarSexos();
            model.zonaCobroList = ListarZonasCobro();
            model.catList = ListarCategorias();

            if (!ModelState.IsValid)
                return View(model);
            
            var socioID = db.socioData.FirstOrDefault(p => p.socioID == model.socioID);

            if (db.socioData.Where(s => s.socioNumDoc == model.socioNumDoc && s.socioID != model.socioID).Any())
            {
                ModelState.AddModelError("socioNumDoc", "El número de DNI ya se encuentra registrado.");
                return View(model);
            }

            if (model.socioFechaNacimiento.Year < 1900 || model.socioFechaNacimiento == null)
            {
                ModelState.AddModelError("socioFechaNacimiento", "La fecha de nacimiento es incorrecta.");
                return View(model);
            }
            socioID.socioNombre = model.socioNombre.ToUpper();
            socioID.socioApellido = model.socioApellido.ToUpper();
            socioID.socioDireccion = model.socioDireccion.ToUpper();
            socioID.socioTelefono = model.socioTelefono;
            socioID.socioEmail = model.socioEmail;
            socioID.socioLocalidadID = model.selLoc;
            socioID.socioTipoDoc = model.selTipoDoc;
            socioID.socioNumDoc = model.socioNumDoc;
            socioID.socioFechaNacimiento = model.socioFechaNacimiento;
            socioID.socioSexoID = model.selSex;
            socioID.socioZonaCobroID = model.selZonaCobro;
            socioID.socioCategoriaID = model.selCat;
            socioID.socioFechaIngreso = DateTime.Now;

            db.Entry(socioID).State = EntityState.Modified;
            await db.SaveChangesAsync();

            string message = "Modificaste la información del socio <b>" + model.socioApellido + " " + model.socioNombre + "</b> (#<b>" + model.socioID + "</b>) con éxito.";
            TempData["Info"] = message;

            Auditoria((int)Session["Session"], 2, 2, "Modificó la información del socio " + socioID.socioApellido + " " + socioID.socioNombre + " - DNI N°: " + socioID.socioNumDoc + " (#" + socioID.socioID + ")");

            return RedirectToAction("Detalles", "Socios", new { id = model.socioID });
        }

        [HttpGet]
        [AuthorizationFilter("Secretaria", "Administrador")]
        public ActionResult Baja(int id)
        {
            socioData socioData = db.socioData.Find(id);
            grupoFamiliarIntegranteData intGF = db.grupoFamiliarIntegranteData.Where(x => x.igfIntegranteID == id).FirstOrDefault();

            if (intGF == null)
            {
                if (socioData == null)
                    throw new HttpException((int)HttpStatusCode.BadRequest, "La consulta no devuelve contenido.");

                bajaViewModel model = new bajaViewModel()
                {
                    socioID = socioData.socioID,
                    causaBajaList = ListarCausaBaja()
                };
                return PartialView("_Baja", model);
            }
            else
            {
                db.grupoFamiliarIntegranteData.Remove(intGF);
                db.SaveChangesAsync();

                if (socioData == null)
                    throw new HttpException((int)HttpStatusCode.BadRequest, "La consulta no devuelve contenido.");

                bajaViewModel model = new bajaViewModel()
                {
                    socioID = socioData.socioID,
                    causaBajaList = ListarCausaBaja()
                };
                return PartialView("_Baja", model);
            }
        }

        [HttpPost]
        [AuthorizationFilter("Secretaria", "Administrador")]
        public async Task<ActionResult> Baja(bajaViewModel model)
        {
            string mensaje;
            if (model.selCausaBaja == null)
            {
                mensaje = "Debe seleccionar una causa de baja.";
                return Json(new { success = false, mensaje }, JsonRequestBehavior.AllowGet);
            }
            
            try
            {
                socioData socioData = await db.socioData.FindAsync(model.socioID);

                socioData.socioEstado = false;
                socioData.socioFechaBaja = DateTime.Now;
                socioData.socioCausaBajaID = model.selCausaBaja;
                db.Entry(socioData).State = EntityState.Modified;
                await db.SaveChangesAsync();

                Auditoria((int)Session["Session"], 2, 3, "Dió de baja al socio " + socioData.socioApellido + " " + socioData.socioNombre + " - DNI N°: " + socioData.socioNumDoc + " (#" + socioData.socioID + ") - Causa de baja: #" + model.selCausaBaja);

                mensaje = "El socio <b>" + socioData.socioApellido + " " + socioData.socioNombre + "</b> fue dado de baja con éxito.";
                return Json(new { success = true, mensaje }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                mensaje = "Tuvimos un problema para actualizar la información del socio. Si el error persiste, comunicate con el desarrollador.";
                return Json(new { success = false, mensaje }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [AuthorizationFilter("Secretaria", "Administrador")]
        public async Task<ActionResult> Alta(int id)
        {
            string message;
            try
            {
                socioData socioData = await db.socioData.FindAsync(id);

                socioData.socioEstado = true;
                socioData.socioCausaBajaID = null;
                socioData.socioFechaBaja = null;

                db.Entry(socioData).State = EntityState.Modified;
                await db.SaveChangesAsync();

                Auditoria((int)Session["Session"], 2, 1, "Dió de alta al socio " + socioData.socioApellido + " " + socioData.socioNombre + " - DNI N°: " + socioData.socioNumDoc + " (#" + socioData.socioID + ")");

                message = "El socio <b>" + socioData.socioApellido + " " + socioData.socioNombre + "</b> fue dado de alta con éxito.";
                return Json(new { success = true, message }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                message = "Tuvimos un problema para actualizar la información del socio. Si el error persiste, comunicate con el desarrollador.";
                return Json(new { success = false, message }, JsonRequestBehavior.AllowGet);
            }
        }

        public void Auditoria(int userID, int moduloID, int accionID, string detalle)
        {
            sistemaAuditoriaData auditDTO = new sistemaAuditoriaData()
            {
                auditUsuarioID = userID,
                auditModuloID = moduloID,
                auditAccionID = accionID,
                auditDetalle = detalle,
                auditFecha = DateTime.Now
            };
            db.sistemaAuditoriaData.Add(auditDTO);
            db.SaveChangesAsync();
        }

        [HttpPost]
        public async Task<JsonResult> Eliminar(int id)
        {
            using (var db = new webCentenarioEntities())
            {
                socioData socioData = await db.socioData.FindAsync(id);
                grupoFamiliarIntegranteData intGF = await db.grupoFamiliarIntegranteData.Where(x => x.igfIntegranteID == id).FirstOrDefaultAsync();

                if (intGF == null)
                {
                    if (socioData == null)
                        return Json(false, JsonRequestBehavior.AllowGet);

                    db.socioData.Remove(socioData);
                    await db.SaveChangesAsync();
                }
                else
                {
                    db.grupoFamiliarIntegranteData.Remove(intGF);
                    await db.SaveChangesAsync();

                    if (socioData == null)
                        return Json(false, JsonRequestBehavior.AllowGet);

                    db.socioData.Remove(socioData);
                    await db.SaveChangesAsync();
                }

                string message = "Eliminaste al socio nro.:" + id;

                return Json(new { result = true, message }, JsonRequestBehavior.AllowGet);

            }
        }

        #region --> Listas



        private static List<SelectListItem> ListarProvincias()
        {
            socioViewModel socio = new socioViewModel();
            List<SelectListItem> items = new List<SelectListItem>();

            using (var db = new webCentenarioEntities())
            {
                var provs = db.provData.ToList();
                foreach (var item in provs)
                {
                    items.Add(new SelectListItem
                    {
                        Text = item.provNombre.ToUpper(),
                        Value = item.provID.ToString()
                    });
                }
            }
            return items;
        }

        [HttpPost]
        public JsonResult ObtenerLocalidades(int id)
        {
            return Json(ListarLocalidades(id), JsonRequestBehavior.AllowGet);
        }

        private static List<SelectListItem> ListarLocalidades(int? id)
        {
            using (var db = new webCentenarioEntities())
            {
                if (id == null)
                {
                    socioViewModel socio = new socioViewModel();
                    List<SelectListItem> items = new List<SelectListItem>();

                    var provs = db.locData.ToList();
                    foreach (var item in provs)
                    {
                        items.Add(new SelectListItem
                        {
                            Text = item.locNombre.ToUpper(),
                            Value = item.locID.ToString(),
                        });
                    }
                    return items;
                }
                else
                {
                    var locs = db.locData.Where(x => x.locProvID == id)
                        .OrderBy(x => x.locID)
                        .Select(x => new SelectListItem { Value = x.locID.ToString(), Text = x.locNombre.ToUpper() })
                        .ToList();

                    return locs;
                }
            }
        }

        private static List<SelectListItem> ListarSexos()
        {
            socioViewModel socio = new socioViewModel();
            List<SelectListItem> items = new List<SelectListItem>();
            using (var db = new webCentenarioEntities())
            {
                var sexs = db.sexoData.ToList();
                foreach (var item in sexs)
                    items.Add(new SelectListItem()
                    {
                        Text = item.sexNombre,
                        Value = item.sexID.ToString()
                    });
            }
            return items;
        }

        private static List<SelectListItem> ListarCategorias()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            using (var db = new webCentenarioEntities())
            {
                var cats = db.categoriaSocioData.ToList();
                foreach (var item in cats)
                    items.Add(new SelectListItem()
                    {
                        Text = item.catNombre,
                        Value = item.catID.ToString()
                    });
            }
            return items;
        }

        private static List<SelectListItem> ListarZonasCobro()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            using (var db = new webCentenarioEntities())
            {
                var zc = db.zonaCobroData.ToList();
                foreach (var item in zc)
                {
                    items.Add(new SelectListItem()
                    {
                        Text = item.zcNombre,
                        Value = item.zcID.ToString()
                    });
                }
            }
            return items;
        }

        private static List<SelectListItem> ListarTiposDoc()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            using (var db = new webCentenarioEntities())
            {
                var tipodoc = db.tipoDocumentoData.ToList();
                foreach (var item in tipodoc)
                    items.Add(new SelectListItem()
                    {
                        Text = item.tdNombre,
                        Value = item.tdID.ToString()
                    });
            }
            return items;
        }

        [HttpPost]
        public JsonResult ObtenerCausasBaja()
        {
            return Json(ListarCausaBaja(), JsonRequestBehavior.AllowGet);
        }

        private static List<SelectListItem> ListarCausaBaja()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            using (var db = new webCentenarioEntities())
            {
                var cb = db.causaBajaData.ToList();
                foreach (var item in cb)
                    items.Add(new SelectListItem()
                    {
                        Text = item.cbNombre,
                        Value = item.cbID.ToString()
                    });
            }
            return items;
        }
        #endregion
    }
}
