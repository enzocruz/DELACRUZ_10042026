namespace FileProcessing.Api.Endpoints;

interface IEndpoint
{
    void Map(IEndpointRouteBuilder app);
}
interface IGroupEndpoint
{
    void MapGroup(IEndpointRouteBuilder app);
}