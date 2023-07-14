using FarmStation.Models;
using FarmStationDb.Models.FarmDataJson;

namespace FarmStation.DataLayer;

public class MappingProfile : AutoMapper.Profile
{
	public MappingProfile()
	{
		CreateMap<FarmData, FarmViewModel>()		
		.ForMember(f => f.FarmedBalanceCrypto, opt => opt.MapFrom(src => src.Balance))
		.ForMember(f => f.PlotsCount, opt => opt.MapFrom(src => src.TotalPlots))
		.ForMember(f => f.ResponseTimeSecondsMax, opt => opt.MapFrom(src => src.MaxTime))
		.ForMember(f => f.ResponseTimeSecondsMedian, opt => opt.MapFrom(src => src.MedianTime))
		.ForMember(f => f.WalletBalanceCrypto, opt => opt.MapFrom(src => src.Wallets[0].ConfirmedBalance / src.Wallets[0].MajorToMinorMultiplier))
		.ForMember(f => f.TotalDriveSpaceBytes, opt => opt.MapFrom( src => src.TotalDiskSpace))
		.ForMember(f => f.LastUpdated, opt => opt.MapFrom( src => DateTimeOffset.FromUnixTimeMilliseconds(src.LastUpdated).UtcDateTime ))		
		.AfterMap((src, dst) =>
		{
			var coldWallet = src.Wallets.Where(w => w.Name == "AllTheBlocks Cold Wallet").FirstOrDefault();
			if (coldWallet == null)
			{
				dst.HasColdWallet = false;
			}
			else
			{
				dst.HasColdWallet = true;
				dst.ColdWalletBalanceCrypto = coldWallet.NetBalance / coldWallet.MajorToMinorMultiplier;
			}

			var oldestPlot = src.Plots.OrderBy(p => p.EndUnixDate).First();

			dst.FarmingSinceDaysAgo =(int) (DateTime.UtcNow - DateTimeOffset.FromUnixTimeMilliseconds(oldestPlot.EndUnixDate).UtcDateTime).TotalDays; 
		});
		
	}
}
