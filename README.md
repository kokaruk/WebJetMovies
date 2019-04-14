# WebJetMovies
Test project to query two API and aggregate the output based on name and price

## Webjet API Test

This is a sample API provides access to a mocked movies database.

There are 2 API operations available for 2 popular movie databases, cinemaworld and filmworld

/api/{cinemaworld or filmworld}/movies : This returns the movies that are available

/api/{cinemaworld or filmworld}/movie/{ID}: This returns the details of a single movie

To access this API you'll require an API token.

Also just like any realworld API these may be flakey at times :)

### Exercise

Build a web app to allow customers to get the cheapest price for movies from these two providers in a timely manner.

Design a solution to have a functioning app when not all of the providers are functioning at 100 %.

The goal of the test is to allow you to present your best code that you feel proud off.

Feel free to make and document any assumptions you have made.

The API token provided to you should not be exposed to the public.


Implementation Assumptions
-----------

1. External API spits out limited size of lists (possible to fit in the memory of a server without degrading performance).
2. Data for prices and movies doesn\'t change very often. It is possible to in-memory cache responses (up to 10 minutes for supplied api) and up to 180 minutes for images URI retrieved from the img_db database API (see below discovered API limitations)
3. Main all movies listing doesn\'t require displaying lowers price. Only when more info requested. 
4. Information about a movie doesn\'t require displaying all supplied information (\'**cinemaworld**\' schema has an extra attribute `awards`, I'm skipping it). 

API limitation
-----

Supplied API data returns broken links to images from IMDB, in order to address the issue, other provider ([TMDB](https://www.themoviedb.org)) is being queried to retrieve working URI and then it is being replaced in the output on the fly