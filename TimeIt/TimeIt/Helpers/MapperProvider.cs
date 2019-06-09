using AutoMapper;
using AutoMapper.Configuration;
using GalaSoft.MvvmLight.Ioc;

namespace TimeIt.Helpers
{
    public class MapperProvider
    {
        public IMapper GetMapper()
        {
            var mce = new MapperConfigurationExpression();
            mce.ConstructServicesUsing(SimpleIoc.Default.GetInstance);

            mce.AddProfiles(typeof(MappingProfile).Assembly);

            var mc = new MapperConfiguration(mce);
            mc.AssertConfigurationIsValid();

            IMapper m = new Mapper(mc, t => SimpleIoc.Default.GetInstance(t));

            return m;
        }
    }
}
