var express = require('express');
var router = express.Router();

/* GET users listing. */
router.get('/', function(req, res) {
//  res.send('respond with a resource');

var myObj = {
    "App":"Fabrikam API",
    "Department":"Express",
    "users": [
      {"name":"boring_dubinsky", "age":31, "city":"New York"},
      {"name":"elegant_franklin", "age":30, "city":"London"},
      {"name":"cocky_khorana", "age":28, "city":"Paris"},
      {"name":"romantic_northcutt", "age":30, "city":"Tokyo"},
      {"name":"lonely_fermat", "age":42, "city":"New York"},
      {"name":"jovial_booth", "age":41, "city":"Dallas"},
      {"name":"elated_yonath", "age":39, "city":"Seattle"},
      {"name":"suspicious_goldberg", "age":22, "city":"Hyderabad"},
      {"name":"reverent_raman", "age":24, "city":"New York"},
      {"name":"agitated_borg", "age":19, "city":"Paris"},
      {"name":"determined_kilby", "age":21, "city":"London"}
    ]
 }

  res.send(JSON.stringify(myObj));
});

module.exports = router;
