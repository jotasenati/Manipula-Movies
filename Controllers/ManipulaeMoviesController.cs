using Manipulaê.Infraestruture.Interfaces;
using Manipulaê.Infraestruture.Interfaces.Repository;
using Manipulaê.Infraestruture.Model.Entities;
using Manipulaê.Infraestruture.Model.Requests;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Buffers;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ManipulaêMovies.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManipulaeMoviesController : Controller
    {
        private readonly IYoutubeApi _youtubeApi;
        private readonly IMoviesRepository _moviesRepository;
        public ManipulaeMoviesController(IYoutubeApi youtubeApi, IMoviesRepository moviesRepository)
        {
            _youtubeApi = youtubeApi;
            _moviesRepository = moviesRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _moviesRepository.AllMovies();

            return result.Any()
                ? Ok(result)
                : NoContent();
        }

        [HttpGet("Videos")]
        public async Task<IActionResult> SearchMovies(string paramenter, bool insertOnDb)
        {
            if (paramenter == null)
            {
                return NotFound();
            }
            else
            {
                var result = await _youtubeApi.YoutubeSearch(paramenter, insertOnDb);

                return Ok(result);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            if (id != Guid.Empty)
            {
                var result = await _moviesRepository.MovieById(id);
                return result != null
                    ? Ok(result)
                    : NotFound();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("Novo Video/Filme")]
        public async Task<IActionResult> Insert(MoviesEntities movies)
        {
            var result = await _moviesRepository.InsertMovies(movies);

            if (result.Id != Guid.Empty || result.Id != null)
                return Created("Video Inserido com sucesso!", result);
            else
                return BadRequest();
        }

        [HttpPatch("Alteração")]
        public async Task<IActionResult> Update(MoviesEntities movies)
        {
            if (movies.Id != null)
            {
                var result = await _moviesRepository.UpdateMovies(movies);
                return CreatedAtAction("Atualizado com sucesso", result);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id != null)
            {
                await _moviesRepository.DeleteMovies(id);
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
    }
}
