# **Weather Project Notes**

The solution was built using the following:

* client side web/SPA app using **React**. React was chosen given it's ability to separate a web app into components, from which you can compose as needed. React allows for a more declarative style of development and also nicely encapsulates both UI (DOM) and behavior within each component (based on the app design). Such a design paradigm (for web front end development) more easily allows and facilities large scale development.     
* server side microservice using **Azure Functions**

### **Source Code**
**Web project
**

* https://github.com/ajonno/pacteraweather.git

**Server side project
**

* https://github.com/ajonno/pactera-microservices.git

### **User Guide**

**Web Project**
 
 You can access the web project running live via this URL: [Pactera Weather App](http://pacteraweather.herokuapp.com/) (hosted on Heroku). *Please note that as this is a free Heroku "dyno" it may take 10+ seconds for the web server to spin up and present a web page after you enter the URL, so please be patient. Once the web app is running response times are excellent*.
 
Getting the current weather for either of the 3 nominated cities is simple. Just choose a city from the drop-down box on the web page and wait for the response to be returned from the server. The results will be displayed in a grid, as per the following screen shot.

![](/wiki-images/ui.png)

### **High Level Solution Diagram**

![](/wiki-images/high-level.png)

### **Assumptions/Notes**

* the server side is stateless and does not persist any data
* client/server communication is via **https**, however no functionality was included to secure the Azure microservice app key. This could be added via a mechanism such as JSON Web Token (JWT).
* there are only a couple of string literals, for temperature, in the micro service app purely for this sample - in a prod app I would add more logic to cater for scenario such as either degrees C, or Fahrenheit etc. and build the string accordingly
* the web app would not need modification, if more cities were to be made available. To do this - it's simply a matter of modifying the web service call being made from the server side app (to receieve additional cities). The additional cities would be added to the existing (outgoing) payload array. Mods on the server side would be minimal.
* Error handling in this test app is light, I would certainly add more rigorous and "defensive" error handling for a production app
* Being a test app minimal focus was placed on the UI/UX and more could be done here to improve look and feel.


### Simple UML Activity Diagram
 
![](/wiki-images/UML-Activity.png)