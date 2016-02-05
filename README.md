# CSharp-FormToObjectMapper
Maps NameValueCollections from input forms to objects

The Form To Object Mapper is for taking a List or Collection of Key Value Pairs and mapping them to an object. 

Why Was this Created? 
------------------------
In a web application a user may not always be presented with a form that contains input elements for all of the properties associated with the entity being edited. This means that when the form is submitted and serialized on the server into an object, not all of the properties will be set. It may require fetching the existing record from the database, and then updating that fetched object with the inputs that were submitted from the web form. This ensures that none of the already existing entity properties are overwritten with blanks, null, or other defaults simply because they were not part of the form. This removes the need of having a lot of hidden inputs on the screen you don't want to the user to see just so they get serialized to the object when the form is submitted.

Once the fetched object has been updated using the FormToObjectMapper with the inputs from the form it can be written and saved to the data storage option of your choice.
