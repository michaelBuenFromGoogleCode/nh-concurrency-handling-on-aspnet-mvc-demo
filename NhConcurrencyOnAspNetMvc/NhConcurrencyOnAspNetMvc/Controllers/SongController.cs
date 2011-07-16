using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using NHibernate.Linq;
using NhConcurrencyOnAspNetMvc.Models;
using NHibernate;

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
        public ActionResult Save(Song song)
        {
            using (var s = Mapper.GetSessionFactory().OpenSession())
            using (var tx = s.BeginTransaction())
            {
                try
                {
                    s.SaveOrUpdate(song);

                    tx.Commit();


                    ModelState.Remove("SongId");
                    // no need to issue ModelState.Remove on Version property; with byte array, ASP.NET MVC always get its value from the model, not from ModelState
                    // ModelState.Remove("Version") 

                }
                catch(StaleObjectStateException ex)
                {                       
                    s.Evict(song);
                    var dbValues = s.Get<Song>(song.SongId);
                    var userValues = song;
                   
                    
                    if (dbValues == null) throw new Exception("This record you are attempting to save is already deleted by other user");
                   
                    if (dbValues.SongName != userValues.SongName)
                        ModelState.AddModelError("SongName", "Current value made by other user: " + dbValues.SongName);

                    
                    if (dbValues.AlbumName != userValues.AlbumName)
                        ModelState.AddModelError("AlbumName", "Current value made by other user: " + dbValues.AlbumName);

                    ModelState.AddModelError(string.Empty, "The record you attempted to edit was already modified by another user since you last loaded it. Open the latest changes on this record");

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }


                return View("Input", song);
            }
        }
    }
}
