using Enities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountryService : ICountriesService
    {
        List<Country> _countries;
        public CountryService(bool initialize)
        {
            _countries = new List<Country>();
            if (initialize)
            {

                Enumerable.Range(1, 10)
                    .Select(i => AddCountry(new() 
                    { CountryName = "Country Number" + i.ToString() }
                    ));
            }
        }
        public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
        {

            if (countryAddRequest is null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }


            if (countryAddRequest.CountryName is null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest.CountryName));
            }


            Country country = new Country();

            country.Name = countryAddRequest.CountryName;

            country.CountryId = Guid.NewGuid();

            if (_countries.Where(c => c.Name == countryAddRequest.CountryName).Count() > 0)
            {
                throw new ArgumentNullException(nameof(countryAddRequest.CountryName));
            }

            _countries.Add(country);

            return country.ToCountryResponse();
        }

        public List<CountryResponse> GetAllCountrise()
        {
            List<CountryResponse> responseList = new List<CountryResponse>();

            foreach (Country country in _countries)
            {
                responseList.Add(country.ToCountryResponse());
            }

            return _countries.Select(country => country.ToCountryResponse()).ToList();



        }

        public CountryResponse? GetCountryById(Guid? countryId)
        {
            var result = _countries.FirstOrDefault(c => c.CountryId == countryId);

            return result?.ToCountryResponse() ?? null;
        }

    }
}
