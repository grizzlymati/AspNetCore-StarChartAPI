using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

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
            var objs = _context.CelestialObjects.Where(x => x.Name == name);
            if (objs.Count() == 0) return NotFound();
            foreach (var item in objs)
            {
                item.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == item.Id).ToList();
            }

            return Ok(objs);
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

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();
            return CreatedAtRoute("GetById", new { id = celestialObject.Id }, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var dbObj = _context.CelestialObjects.Find(x => x.Id == id);
            if (dbObj == null) return NotFound();
            dbObj.Name = celestialObject.Name;
            dbObj.OrbitalPeriod = celestialObject.OrbitalPeriod;
            dbObj.OrbitedObjectId = celestialObject.OrbitedObjectId;
            _context.Update(dbObj);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var dbObj = _context.CelestialObjects.FirstOrDefault(x => x.Id == id);
            if (dbObj == null) return NotFound();
            dbObj.Name = name;
            _context.Update(dbObj);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var dbObjs = _context.CelestialObjects.Where(x => x.Id == id || x.OrbitedObjectId == id);
            if (dbObjs.Count() == 0) return NotFound();
            _context.RemoveRange(dbObjs);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
