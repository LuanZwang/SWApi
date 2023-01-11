# SWApi
This project was made to test some tech skills

## API
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

