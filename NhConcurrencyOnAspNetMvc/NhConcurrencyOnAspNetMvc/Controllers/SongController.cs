using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using NHibernate.Linq;
using NhConcurrencyOnAspNetMvc.Models;
using NHibernate;

using Ienablemuch;

namespace NhConcurrencyOnAspNetMvc.Controllers
{
    public class SongController : Controller
    {
        //
        // GET: /Song/

        public ActionResult Index()
        {
            using (var s = Mapper.GetSessionFactory().OpenSession())
            {
                return View(s.Query<Song>().OrderBy(x => x.SongName).ToList());
            }
        }


        public ViewResult Input(int id = 0)
        {
            using (var s = Mapper.GetSessionFactory().OpenSession())            
            {
                var song = id != 0 ? s.Get<Song>(id) : new Song();
                return View(song);
            }
        }

        
        [HttpPost]
        public JsonResult SaveViaAjax(Song song)
        {
            SaveCommon(song);

            // you can use the following commented code
            
            /* whitebox
             
            var v = from m in ModelState.AsEnumerable()
                    from e in m.Value.Errors
                    select new { m.Key, e.ErrorMessage };
            
            
            return Json(
                new 
                { 
                    ModelState = 
                        new 
                        { 
                            IsValid = ModelState.IsValid, 
                            PropertyErrors = v.Where(x => !string.IsNullOrEmpty(x.Key)),
                            ModelErrors = v.Where(x => string.IsNullOrEmpty(x.Key))
                        },
                    SongId = song.SongId,
                    Version = Convert.ToBase64String(song.Version ?? new byte[]{} )
                });*/


            
            // alternatively, you can use an extension method, less error-prone

            return Json(
                new 
                { 
                    ModelState = ModelState.ToJsonValidation(),
                    SongId = song.SongId,
                    Version = Convert.ToBase64String(song.Version ?? new byte[]{} )
                });


        }


        [HttpPost]
        public ActionResult Save(Song song)
        {
            SaveCommon(song);

            return View("Input", song);
        }

        private void SaveCommon(Song song)
        {
            using (var s = Mapper.GetSessionFactory().OpenSession())
            using (var tx = s.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        s.SaveOrUpdate(song);

                        tx.Commit();

                        ModelState.Remove("SongId");
                        // no need to issue ModelState.Remove on Version property; with byte array, ASP.NET MVC always get its value from the model, not from ModelState
                        // ModelState.Remove("Version") 
                    }

                }
                catch (StaleObjectStateException)
                {                    
                    s.Evict(song);
                    var dbValues = s.Get<Song>(song.SongId);
                    var userValues = song;


                    if (dbValues == null)
                    {
                        ModelState.AddModelError(string.Empty, "This record you are attempting to save is already deleted by other user");
                        return;
                    }

                    if (dbValues.SongName != userValues.SongName)
                        ModelState.AddModelError("SongName", "Current value made by other user: " + dbValues.SongName);


                    if (dbValues.AlbumName != userValues.AlbumName)
                        ModelState.AddModelError("AlbumName", "Current value made by other user: " + dbValues.AlbumName);

                    ModelState.AddModelError(string.Empty,
                        "The record you attempted to edit was already modified by another user since you last loaded it. Open the latest changes on this record");

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }

            }
        }


        public ActionResult Delete(int id, byte[] Version)
        {
            using (var s = Mapper.GetSessionFactory().OpenSession())
            using (var tx = s.BeginTransaction())
            {

                
                if (Request.HttpMethod == "POST")
                {
                    var versionedDelete = new Song { SongId = id, Version = Version };
                    try
                    {
                        s.Delete(versionedDelete);
                        tx.Commit();

                        return RedirectToAction("Index");
                    }
                    catch (StaleObjectStateException)
                    {
                        s.Evict(versionedDelete);

                        var songToDelete = s.Get<Song>(id);
                        if (songToDelete == null)
                            return RedirectToAction("Index");

                        ModelState.AddModelError(string.Empty, "The record you are attempting to delete was modified by other user first. Do you still want to delete this record?");
                        return View(songToDelete);
                    }
                }
                else
                {
                    var songToDelete = s.Get<Song>(id);
                    return View(songToDelete);
                }
                
            }
        }

    }//class
}
