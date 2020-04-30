using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;

namespace StarChart.Controllers
{
    [ApiController]
    [Route("")]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var obj = _context.CelestialObjects.FirstOrDefault(x => x.Id == id);
            if (obj == null) return NotFound();
            var satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == id).ToList();
            obj.Satellites = satellites;
            
            return Ok(obj);
        }

        [HttpGet("{name}", Name = "GetByName")]
        public IActionResult GetByName(string name)
        {
            var obj = _context.CelestialObjects.FirstOrDefault(x => x.Name == name);
            if (obj == null) return NotFound();
            var satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == obj.Id).ToList();
            obj.Satellites = satellites;

            return Ok(obj);
        }

        [HttpGet(Name = "GetAll")]
        public IActionResult GetAll()
        {
            var objs = _context.CelestialObjects.ToList();

            foreach (var item in objs)
            {
                item.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == item.Id).ToList();
            }

            return Ok(objs);
        }
    }
}
