using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace StarChart.Controllers
{
    using StarChart.Data;

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
    }
}
