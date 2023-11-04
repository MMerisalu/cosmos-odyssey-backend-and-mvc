# cosmos-odyssey-backend-and-mvc
A repository for cosmos odyssey backend and mvc

The cosmos odyssey application contains of MVC (backend, client) and Blazor Webassemly (client).
Both apps are hosted in Azure.
Link to open Blazor WebAssemly client app: https://cosmosclientappblazor.azurewebsites.net/
Link to open MVC version of the app: https://cosmosodysseyapp.azurewebsites.net/

On the front page of both applications is a search box. To find a reservation, the reservation Id and a reservation owner's last name must be provided.

If the reservation is found, the reservation's details page will be opened.
All the reservation are listed on the reservations index page. In order to create a new reservation, click on the "Create new" link.
To create a reservation, user must enter their first name, last name and choose a planet from where they start their flight. To get the list of available destinations, user can click on the "Add Flight" button. A table view with all the possible destinations, will appear. Flights can be filtered by company name, destination and sorted by price, distance or travel time. Clicking on the "Select" button, adds the selected route to the reservation. Reservation can have as many routes as needed. To create a resevation on the MVC app, click on "Create", on Blazor client app, click on "Submit". 
Next a reservation details page, is opened which will have all the reservation details.
If there occurs any database connection problems, just give it a minute and then refresh the page. 
Github repository for the Blazor client app: https://github.com/MMerisalu/cosmos-odyssey-client
Github repository for the MVC app: https://github.com/MMerisalu/cosmos-odyssey-backend-and-mvc
 