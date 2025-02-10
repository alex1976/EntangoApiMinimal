using EntangoApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EntangoApi.Endpoints
{
    public static class CitiesEndpoints
    {
        public static void Map(WebApplication app)
        {
            //POST cities
            //app.MapPost("/cities", /*[Authorize]*/ async (Cities city, CitiesDb db) =>
            //{
            //    db.Cities.Add(city);
            //    await db.SaveChangesAsync();

            //    return Results.Created($"/cities/{city.Id}", city);
            //}).WithTags("Cities");

            //GET all cities (limited to 10000 elements)
            app.MapGet("/cities", /*[Authorize]*/ async (CitiesDb db) =>
                await db.Cities
                .Take(10000)
                .OrderBy(Id => Id)
                .ToListAsync()).WithTags("Cities"); //serve per raggruppare gli endpoints

            //GET by page
            app.MapGet("/cities_by_page", /*[Authorize]*/ async (int pageNumber, int pageSize, CitiesDb db) =>
                 await db.Cities
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync())
                .WithTags("Cities");

            //GET by province
            app.MapGet("/cities_by_province", /*[Authorize]*/ async (string provinceCode, CitiesDb db) =>
                 await db.Cities
                 .Where(x => x.ProvinceCode == provinceCode)
                 .ToListAsync())
                 .WithTags("Cities");

            //GET by string
            app.MapGet("/cities_by_string", /*[Authorize]*/ async (string search, CitiesDb db) =>
                 await db.Cities
                 .Where(x => x.ProvinceCode.Contains(search) || x.Name.Contains(search) || x.RegionDescription.Contains(search) || x.ProvinceDescription.Contains(search))
                 .ToListAsync())
                 .WithTags("Cities");

            //GET by landRegistryCode
            app.MapGet("/cities_by_landcode", /*[Authorize]*/ async (string landCode, CitiesDb db) =>
                 await db.Cities
                 .Where(x => x.LandRegistryCode == landCode)
                 .ToListAsync())
                 .WithTags("Cities");

            ////GET by id
            //app.MapGet("/cities/{id}", /*[Authorize]*/ async (int id, CitiesDb db) =>
            //      await db.Cities.FindAsync(id)
            //      is Cities city
            //      ? Results.Ok(city)
            //      : Results.NotFound())
            //      .WithTags("Cities");

            //PUT
            //app.MapPut("/cities/{id}", /*[Authorize]*/ async (int id, Cities inputCity, CitiesDb db) =>
            //{
            //    var city = await db.Cities.FindAsync(id);

            //    if (city is null) return Results.NotFound();

            //    city.Name = inputCity.Name;
            //    city.IstatCode = inputCity.IstatCode;
            //    city.ProvinceCode = inputCity.ProvinceCode;

            //    await db.SaveChangesAsync();

            //    return Results.NoContent();
            //}).WithTags("Cities");

            //DELETE
            //app.MapDelete("/cities/{id}", /*[Authorize]*/ async (int id, CitiesDb db) =>
            //{
            //    if (await db.Cities.FindAsync(id) is Cities city)
            //    {
            //        db.Cities.Remove(city);
            //        await db.SaveChangesAsync();
            //        return Results.Ok(city);
            //    }

            //    return Results.NotFound();
            //}).WithTags("Cities");
        }
    }
}
