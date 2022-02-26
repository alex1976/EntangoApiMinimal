CREATE TABLE Towns (
    Id int IDENTITY(1,1),
    Code nvarchar(5), /*codice comune progressivo*/
    Name nvarchar(100), /*nome comune*/
    CountryCode nvarchar(5), /*unità terrirotiale*/
    IstatCode nvarchar(10), /*ProvinceCode + Code*/
    RegionCode nvarchar(5),
    RegionDescription nvarchar(100),
    ProvinceCode nvarchar(5),
    ProvinceDescription nvarchar(100), /*Non presente nel tracciato ISTAT*/
    ProvinceAbbreviation nvarchar(5),
    GeographicCode nvarchar(5),
    GeographicDescription nvarchar(100),
    LandRegistryCode nvarchar(5), /*codice catastale*/
    UpdateOn date
);

CREATE TABLE Users (
    Id int IDENTITY(1,1),
    Username nvarchar(100) NOT NULL,
    Password nvarchar(250) NOT NULL,
    Email nvarchar(100),
    CreatedOn date
);

CREATE TABLE BuildingSites (
		Id int IDENTITY(1,1),
		Code nvarchar(50) NOT NULL,
		Description nvarchar(250) NOT NULL,
		Client nvarchar(100) NOT NULL,
		Contractor nvarchar(100) NOT NULL,
        Latitde decimal(28,14),
        Longitde decimal(28,14),
        Address nvarchar(100),
        Location nvarchar(100),
        WorksAmount decimal(28,14),
        WorksStartDate date,
        WorksEndDate date,
        Progress decimal(28,14),
        UpdateOn date
);

