# SWApi
This project was made to test some tech skills

#### This documentation is divided in three parts
* ### API
* ### How to run the project
* ### Code Coverage

# API
The API consists in 4 endpoints, being them:

| Name | Route                              | Verb  | Description |
|:-----|:--------------------------------------|:------|-|
| GetById    | api/planets/{id}                    | GET   | Gets a planet by its id |
| GetByName  | api/planets/name/{name}             | GET   | Gets a list of planets by their names |
| GetAll     | api/planets?page={page}&pageSize={pageSize} | GET   | Gets a dto with pagination infos and the paginated planets list
| Delete     | api/planets/{id}                    | DELETE| Deletes a planet by its id |


# Responses by Endpoints
#### Note: All endpoints can return a 500 response

## GetById
#### Route: api/planets/{id}
#### Parameter type: GUID/UUID4
#### Parameter constraints:
* Must not be empty/null
* Must be a valid value (invalid value e.g. "00000000-0000-0000-0000-000000000000")

|HttpStatusCode| ResponseType | Message (if error)            |
|:-------------|:-------------|:------------------------------|
| 200          | PlanetDto    |N/A                            |
| 400          | string       |"Only valid GUIDs are allowed."|
| 404          | string       |N/A                            |

## GetByName
#### Route: api/planets/{name}
#### Parameter type: string (case insensitive)
#### Parameter constraints:
* Must not be empty/null
* Length should be greater than 0 and smaller than 51 characters

|HttpStatusCode| ResponseType | Message (if error)                                               |
|:-------------|:-------------|:-----------------------------------------------------------------|
| 200          | PlanetDto[]  |N/A                                                               |
| 400          | string       |"Planet name for search should be informed."                      |
| 400          | string       |"Planet name for search should not be greater than 50 characters."|

## GetAll
#### Route: api/planets?page={page}&pageSize={pageSize}
#### Parameter types:
* page: int or null value
* pageSize: int or null value

#### Parameters constraints:
* page/pageSize can be empty/null
* pageSize cannot be greater than 100

### Default pagination values
* If page is null or 0, the API will return data from page 1
* if pageSize is null or 0, the API will return data with pageSize of 60

|HttpStatusCode| ResponseType | Message (if error)                                               |
|:-------------|:-------------|:-----------------------------------------------------------------|
| 200          | GetAllDto    |N/A                                                               |
| 400          | string       |"Page size cannot be greater than 100."                           |


## Delete
#### Route: api/planets/{id}
#### Parameter type: GUID/UUID4
#### Parameter constraints:
* Must not be empty/null
* Must be a valid value (invalid value e.g. "00000000-0000-0000-0000-000000000000")

|HttpStatusCode| ResponseType | Message (if error)            |
|:-------------|:-------------|:------------------------------|
| 200          | PlanetDto    |N/A                            |
| 400          | string       |"Only valid GUIDs are allowed."|
| 404          | string       |N/A                            |

# Schemas

## PlanetDto
```
{
  "id": "string",
  "name": "string",
  "climate": "string",
  "terrain": "string",
  "films": FilmDto[]
}
```
#### where

| Property | Nullable? |
|:---------|:----------|
| id       | no        |
| name     | yes       |
| climate  | yes       |
| terrain  | yes       |
| films    | no        |

## FilmDto
```
{
  "title": "string",
  "director": "string",
  "releaseDate": "string"
}
```

| Property     | Nullable? |
|:-------------|:----------|
| title        | yes       |
| director     | yes       |
| releaseDate  | yes       |

## GetAllDto
```
{
  "pageSize": int,
  "totalCount": int,
  "nextPage": int,
  "previousPage": int,
  "items": PlanetDto[]
}
```

| Property     | Nullable? | When will be null               |
|:-------------|:----------|:--------------------------------|
| pageSize     | no        |N/A                              |
| totalCount   | no        |N/A                              |
| nextPage     | yes       |when theres no data on next page |
| previousPage | yes       |when it's on first page          |
| items        | no        |N/A                              |

# Setup to run the project
To run the code coverage analysis, I recommend you to use a docker infrastructure manager, like [Rancher Desktop](https://rancherdesktop.io/)

After installing and configuring the docker manager command on the root page (the path is the root of SWApi):
```
    docker-compose up -d
```

It will create a container of mongodb, where the connection string is:
```
    mongodb://admin:admin@127.0.0.1:27017
```

After that, you can use the application of your choice to run the mongo db command [in this gist](https://gist.github.com/LuanZwang/2b2f0780bd703580caa5da0230c5a724) to add the [swapi planets](https://swapi.dev/api/planets) on your localhost MongoDB.

With all complete, you can run the application


# Code Coverage
To run the code coverage analysis, I recommend you to use a docker infrastructure manager, like [Rancher Desktop](https://rancherdesktop.io/)

After installing and configuring the docker manager, run a sonarqube container using the following command:
```
docker run -d --name sonarqube -p 9000:9000 -p 9092:9092 sonarqube:8.9.1-community
```

When finished, access https://localhost:9000 using the credential "admin" for user and "admin" for password, then you'll be redirected to a page to redefine your password.

When you redefined your password, go to [The Security Page](http://localhost:9000/account/security/), where you will create a token with name "SWApi".

Having the token in hands, go to the tests project (the path is SWApi\SWApi.Test) and do a right click on the sonar.bat file, then click in edit.

Replace the "TOKEN_VALUE" paramater to the token you got, keeping it in the double quotes.

The process will be like this:

You will change /d:sonar.login="TOKEN_VALUE" to /d:sonar.login="51ac19b8a6e82d62010865b54945524d8a339682"

Now, save the file, close the editor and do a double click on it. A console window will open and start to run.

When it finishes, you can click any key to close the console and you will be able to se a dashboard on the main page on your [localhost sonarqube](http://127.0.0.1:9000/projects) and you will can see the coverage results.
