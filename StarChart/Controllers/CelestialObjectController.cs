using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace StarChart.Controllers
{
    using Microsoft.EntityFrameworkCore;

    using StarChart.Data;
    using StarChart.Models;

    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet(template: "{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = this._context.CelestialObjects.FirstOrDefault(x => x.Id == id);
            if (celestialObject == null)
            {
                return this.NotFound();
            }

            celestialObject.Satellites = this._context.CelestialObjects.Where(w => w.OrbitedObjectId == celestialObject.Id).ToList();
            return this.Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = this._context.CelestialObjects.Where(x => x.Name == name).ToList();
            if (celestialObjects.Count == 0)
            {
                return this.NotFound();
            }

            foreach (var celestialObject in celestialObjects)
            {
                celestialObject.Satellites = this._context.CelestialObjects.Where(w => w.OrbitedObjectId == celestialObject.Id).ToList();
            }
            
            return this.Ok(celestialObjects);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = this._context.CelestialObjects.ToList();
            if (celestialObjects.Count == 0)
            {
                return this.NotFound();
            }

            foreach (var celestialObject in celestialObjects)
            {
                celestialObject.Satellites = this._context.CelestialObjects.Where(w => w.OrbitedObjectId == celestialObject.Id).ToList();
            }

            return this.Ok(celestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject celestialObject)
        {
            this._context.CelestialObjects.Add(celestialObject);
            var result = this._context.SaveChanges();
            return this.CreatedAtRoute("GetById", new { id = celestialObject.Id }, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var databaseObject = this._context.CelestialObjects.FirstOrDefault(x => x.Id == id);
            if (databaseObject == null)
            {
                return this.NotFound();
            }

            databaseObject.Name = celestialObject.Name;
            databaseObject.OrbitedObjectId = celestialObject.OrbitedObjectId;
            databaseObject.OrbitalPeriod = celestialObject.OrbitalPeriod;
            this._context.SaveChanges();

            return this.NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var databaseObject = this._context.CelestialObjects.FirstOrDefault(x => x.Id == id);
            if (databaseObject == null)
            {
                return this.NotFound();
            }

            databaseObject.Name = name;
            this._context.CelestialObjects.Update(databaseObject);
            this._context.SaveChanges();

            return this.NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var databaseObjects = this._context.CelestialObjects.Where(x => x.Id == id || x.OrbitedObjectId == id).ToList();
            if (databaseObjects.Count == 0)
            {
                return this.NotFound();
            }

            this._context.CelestialObjects.RemoveRange(databaseObjects);
            this._context.SaveChanges();

            return this.NoContent();
        }
    }
}
