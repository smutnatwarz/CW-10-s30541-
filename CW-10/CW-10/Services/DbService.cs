using CW_10.Data;
using CW_10.DTOs;
using CW_10.Exceptions;
using CW_10.Models;
using Microsoft.EntityFrameworkCore;

namespace CW_10.Services;


public interface IDbService
{
    public Task<TripsWithDetailsOnPageGetDto> GetAllTripsWithDetailsOnPage(int page,int pageSize);
    public Task DeleteClient(int id);
    
    public Task CreateClientWithTrips(int id,ClientWithTripsCreateDto clientWithTripsCreateDto);
}
public class DbService (_2019sbdContext data) : IDbService
{
    public async Task<TripsWithDetailsOnPageGetDto> GetAllTripsWithDetailsOnPage(int page,int pageSize)
    {
        var skip = (page - 1) * pageSize;
        
        var trips=data.Trips.Select(t => new TripsWithDetailsGetDto()
        {
            Name = t.Name,
            Description = t.Description,
            DateFrom = t.DateFrom,
            DateTo = t.DateTo,
            MaxPeople = t.MaxPeople,
            countries = t.IdCountries.Select(c => new CountryGetDto()
            {
                Name = c.Name
            }).ToList(), 
            clients = t.ClientTrips.Select(ct => new ClientGetDto()
            {
                FirstName = ct.IdClientNavigation.FirstName,
                LastName = ct.IdClientNavigation.LastName,
            }).ToList() 

        });
        var tripcount = await trips.CountAsync();
        var  allpage=Math.Ceiling((double)tripcount/pageSize);
        
        var help=await trips.OrderByDescending(e=>e.DateFrom).Skip(skip).Take(pageSize).ToListAsync();
        var returned = new TripsWithDetailsOnPageGetDto()
        {
            PageNum = page,
            PageSize = pageSize,
            AllPages = (int)allpage,
            Trips = help

        };
        return returned; 
    }

    public async Task DeleteClient(int id)
    {
        var client = await data.Clients.FirstOrDefaultAsync(e => e.IdClient == id);
        if (client == null)
        {
            throw new NotFoundException($"Client with {id} not found");
        }
        
        var countclienttrips= await data.ClientTrips.CountAsync(e => e.IdClient == id);
        if (countclienttrips > 0)
        {
            throw new ConflictException("Client has trips ");
        }
       
        data.Clients.Remove(client);
    }

    public async Task CreateClientWithTrips(int id, ClientWithTripsCreateDto dto)
    {
        var idtrips=new List<int> { id, dto.IdTrip };
        var pesel = dto.Pesel;
        var today = DateTime.Now;
        var client = await data.Clients.FirstOrDefaultAsync(e => e.Pesel == pesel);
        if (client != null)
        {
            throw new ConflictException($"Client with {pesel} already exists");
        }


        var transaction = await data.Database.BeginTransactionAsync();
        try
        {
            var newclient = new Client()
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Pesel = pesel,
                Email = dto.Email,
                Telephone = dto.Telephone,
            };



            await data.Clients.AddAsync(newclient);
            await data.SaveChangesAsync();



            foreach (var idtrip in idtrips)
            {
                var trip = await data.Trips.FirstOrDefaultAsync(t => t.IdTrip == idtrip);
                if (trip == null)
                {
                    throw new NotFoundException($"Trip with {idtrip} not exists");
                }

                if (trip.DateFrom <= today)
                {
                    throw new BadRequestException($"Trip with {idtrip} already started");
                }

                var clienttripcout = await data.ClientTrips.Where(c => c.IdTrip == idtrip)
                    .Where(k => k.IdClient == idtrip).CountAsync();

                if (clienttripcout > 0)
                {
                    throw new ConflictException($"client can't be signed two times in the same trip");
                }

                var clienttrips = new ClientTrip()
                {
                    IdClient = newclient.IdClient,
                    IdTrip = idtrip,
                    RegisteredAt = today,
                    PaymentDate = dto.PaymentDate
                };
                await data.ClientTrips.AddAsync(clienttrips);
                await data.SaveChangesAsync();


            }

            await transaction.CommitAsync();
        }
        catch (NotFoundException)
        {
            await transaction.RollbackAsync();
            throw;
        }
        catch (BadRequestException)
        {
            await transaction.RollbackAsync();
            throw;
        }
        catch (ConflictException)
        {
            await transaction.RollbackAsync();
            throw;
        }
        catch (Exception )
        {
            await transaction.RollbackAsync();
            throw;
        }
        
    }
}