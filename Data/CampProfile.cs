using AutoMapper;
using CoreCodeCamp.Models;

namespace CoreCodeCamp.Data
{
    public class CampProfile: Profile
    {
        public CampProfile()
        {
            CreateMap<Camp, CampModel>()
                .ForMember(o => o.Venue, o => o.MapFrom(o => o.Location.VenueName)).ReverseMap();

            CreateMap<Talk, TalkModel>()
                .ReverseMap()
                .ForMember(t => t.Camp, opt => opt.Ignore()) //for PUT we dont want model to talk complete copy.
                .ForMember(t => t.Speaker, opt => opt.Ignore());

            CreateMap<Speaker, SpeakerModel>().ReverseMap();
        }
    }
}
