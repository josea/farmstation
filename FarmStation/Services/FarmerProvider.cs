using AutoMapper;
using FarmStation.Models;
using static MudBlazor.CategoryTypes;
using System.Text.Json;
using FarmStationDb.DataLayer;

namespace FarmStation.Services;

public class FarmerProvider
{
    private readonly IMapper _mapper;
    private readonly FiatService _fiatService;
    private readonly FarmerRepository _repository;

    public FarmerProvider(IMapper mapper, FiatService fiatService, FarmStationDb.DataLayer.FarmerRepository repository)
	{
        _mapper = mapper;
        _fiatService = fiatService;
        _repository = repository;
    }
    private async Task CompleteFiatDataAsync(FarmViewModel data)
    {
        data.FiatCurrency = "cad";
        var rate = await _fiatService.GetRateAsync(data.Crypto, data.FiatCurrency);

        data.FarmedBalanceFiat = rate * data.FarmedBalanceCrypto;
        data.ColdWalletBalanceFiat = rate * data.ColdWalletBalanceCrypto;
        data.WalletBalanceFiat = rate * data.WalletBalanceCrypto;
    }

    public async Task<List<FarmViewModel>> GetFarmsDataAsync(string userId)
    {
        var data = await _repository.GetFarmsDataAsync(userId);
        
        var result = _mapper.Map<List<FarmViewModel>>(data);

        // TODO: it is calling the rate many times, to fix somehow
        foreach (var farm in result) await CompleteFiatDataAsync(farm);

        return result.ToList();
    }

    public async Task<FarmViewModel> GetFarmDataAsync(string farmId, string userId)
    {
        var farmData = await _repository.GetFarmDataAsync(farmId, userId);

        var result = _mapper.Map<FarmViewModel>(farmData);

        await CompleteFiatDataAsync(result);

        return result;
    }
}
