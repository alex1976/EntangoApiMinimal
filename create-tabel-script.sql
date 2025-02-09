
/* Update 09-02-2025 - base tables */

/***************************/
/***** Cities_It table *****/
/***************************/
CREATE TABLE Cities_It (
    Id int IDENTITY(1,1),
    Code nvarchar(5),							/* codice comune progressivo ISTAT */
    Name nvarchar(100),							/* nome comune ISTAT */
    CountryCode nvarchar(5),					/* unità terrirotiale ISTAT */
    IstatCode nvarchar(10),						/* ProvinceCode + Code */
    RegionCode nvarchar(5),						/* Codice Regione ISTAT */
    RegionDescription nvarchar(100),			/* Denominazione regione ISTAT */
    ProvinceCode nvarchar(5),					/* Sigla automobilistica ISTAT */
    ProvinceDescription nvarchar(100),			
    ProvinceAbbreviation nvarchar(5),
    GeographicCode nvarchar(5),					/* Codice Ripartizione Geografica ISTAT */
    GeographicDescription nvarchar(100),		/* Ripartizione Geografica ISTAT */
    LandRegistryCode nvarchar(5),				/* codice catastale */
    UpdateOn date,
	CONSTRAINT PK_PrimaryKey PRIMARY KEY CLUSTERED (Id)
);

/**********************/
/***** User table *****/
/**********************/
CREATE TABLE Users (
    Id int IDENTITY(1,1),
    Username nvarchar(100) NOT NULL,
    Password nvarchar(250) NOT NULL,
    Email nvarchar(100),
    CreatedOn date
);

/**************************************/
/***** ConstructionSites_It table *****/
/**************************************/
/*CREATE TABLE ConstructionSites (
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
);*/

