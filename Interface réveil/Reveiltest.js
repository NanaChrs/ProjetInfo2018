var bigSleeper=false;
    var choix_meteo=true;
    var choix_agenda=true;
    var choix_fete=true;
    var choix_date=true;
    var choix_traffic=true;
    var modeauto=true;
    var activation=false;
    var HeureSonnerie;
    var Days=[];
     //concernant le changement de source des musiques
    var src_musique;
    var musique=document.querySelector("#audioPlayer");
    //concernant le lancement de la sonnerie :
    var player=document.querySelector("#audioPlayer");
    player.load();

    var temps;
    var adresse;
    console.log(bigSleeper);
    var constellation = $.signalR.createConstellationConsumer("http://localhost:8088", "123456789", "Test API JS");


    function myFunction(){
        console.log(document.getElementById("modeauto").checked);
        constellation.server.sendMessage({ Scope: 'Package', Args: ['Brain'] }, 'ChangeParametresServeur', { "IsActive":document.getElementById("activation").checked, "BigSleeper":document.getElementById("grosdormeur").checked, "Days":Days.toString(), "ManualMode":document.getElementById("modeauto").checked, "ManualAlarmHour":document.getElementById("time").valueAsDate.getHours()-1, "ManualAlarmMinute":document.getElementById("time").valueAsDate.getMinutes()});
         ///////////////on change la source de la musique lorsqu'elle est sélectionnée dans les paramètres//////////
         console.log(src_musique);//c'est la source de la musique choisir précédement par l'utilisateur
         musique.setAttribute("src", src_musique);
        console.log(musique.src);
      }


 
    constellation.connection.stateChanged(function (change) {
      if (change.newState === $.signalR.connectionState.connected) {
        console.log("Je suis connecté");
        constellation.client.registerStateObjectLink("DESKTOP-CI66GL2", "DayInfo", "NameDay", "*", function (so) {
        console.log(so);
        $("#InfoName").text(so.Value);
      });
 
      if(minutes==null){
        minutes=0;
      }
      if(heures==null){
        heures=0;
      }

      var HeureSonnerie= new Date (document.getElementById("time").valueAsDate);
      console.log(HeureSonnerie);
      var minutes=HeureSonnerie.getMinutes();
      var heures=HeureSonnerie.getHours();
          // Snippet compliant from the API 1.8.2 (Constellation-1.8.2.js) 
 
      constellation.client.registerStateObjectLink("DESKTOP-CI66GL2", "Brain", "Parametres_reveil", "*",function(so){
        console.log(so);
      });
 
      document.getElementById("valider").addEventListener("click", myFunction)
      
      constellation.client.registerStateObjectLink("DESKTOP-CI66GL2", "DayInfo", "SunInfo", "*", function (so) {
        console.log(so);
        var d = new Date(so.Value.Date);
        $("#year").text(d.getFullYear());
        var mois=d.getMonth()+1;
        $("#month").text((mois<10?'0':'')+mois);
        $("#day").text((d.getDate()<10?'0':'')+d.getDate());
      });

            //on s'abonne au groupe Reveil-->
      constellation.server.subscribeMessages("Reveil");
  
     //on consomme le state object 'IsRinging' : si le réveil sonne, on passe à la page 0
      constellation.client.registerStateObjectLink("DESKTOP-CI66GL2", "Brain", "Alarm", "*",function (so) {
        var IR = so.Value.IsRinging;
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
           if(so.Value.AlarmHour<10 & so.Value.AlarmHour>0){so.Value.AlarmHour='0'+so.Value.AlarmHour};
            if(so.Value.AlarmMinutes<10 & so.Value.AlarmMinutes>0){so.Value.AlarmMinutes='0'+so.Value.AlarmMinutes};
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        var min=so.Value.AlarmMinutes;
        $("#heure").text(so.Value.AlarmHour);
        $("#min").text((min<10?'0':'')+min);
        console.log("is ringing est detecte")
        console.log( so);
        if (IR) {
          player.play();
          $("#page0").css("display", "block"); 
          console.log("test");         
        }
      });

      constellation.client.registerStateObjectLink("DESKTOP-CI66GL2", "GoogleCalendar", "Events", "*", function (so) {
        console.log(so);
        $("#AgendaNom").text(so.Value[0].Nom);
        $("#DateDebut").text(so.Value[0].DateDebut);
        $("#DateFin").text(so.Value[0].DateFin);
        $("#Lieu").text(so.Value[0].Lieu);
        var lieu=so.Value[0].Lieu;
        if (lieu==null) {
          lieu="ISEN Lille";
        }
 
        constellation.server.sendMessageWithSaga(function(response){
          lat=response.Data.latitude;
          lng=response.Data.longitude;
          constellation.server.sendMessageWithSaga(function(reponse){
            console.log(reponse.Data.currently);
            var degre=Math.round(reponse.Data.currently.apparentTemperature);
            $("#Temperature").text(degre);
            var png=reponse.Data.currently.icon;
            document.getElementById("icon").src="images/"+png+".png";
          },{Scope:'Package',Args:['ForecastIO']},'GetWeatherForecast',lng,lat);
        }, {Scope:'Package', Args:['GeocodingApi']},'CoordoneesGPS',lieu)})
              
      }
    });
        

Paramètres = function(){
  $("#page2").hide();
  $("#page3").show();

  if($("#activation").is(":checked")){
    console.log("bjr");
    $("#hidemodeauto").css("display","block");
    $("#audiofiles").css("display","block");
    $("#hideGD").css("display","block");

    if($("#modeauto").is(":checked")){
      $("#HideTime").css("display","block");
      $("#jours").css("display","block");
      
    }
    else{
      $("#HideTime").css("display","none");
      $("#jours").css("display","none");
    }
  }
  else{
    $("#hidemodeauto").css("display","none");
    $("#audiofiles").css("display","none");
    $("#hideGD").css("display","none");
    $("#HideTime").css("display","none");
    $("#jours").css("display","none");
  }
  
}
 
$("#activation").change(function() {
    if(this.checked) {
      $("#hidemodeauto").css("display","block");
      $("#audiofiles").css("display","block");
      $("#hideGD").css("display","block");
      modeauto.checked=false;
    }
    else{
      $("#hidemodeauto").css("display","none");
      $("#audiofiles").css("display","none");
      $("#hideGD").css("display","none");
      $("#HideTime").css("display","none");
      $("#jours").css("display","none");
      modeauto.checked=false;
    }
});

$("#modeauto").change(function(){
  if(this.checked){
    $("#jours").css("display","block");
    $("#HideTime").css("display","block");
  }
  else{
    $("#jours").css("display","none");
    $("#HideTime").css("display","none");
  }
});

$("#valider").click(function(){
    bigSleeper=document.getElementById("grosdormeur").checked;
    modeauto=document.getElementById("modeauto").checked;
    activation=document.getElementById("activation").checked;
    HeureSonnerie= new Date (document.getElementById("time").value);
    src_musique=document.getElementById("audiofiles").value;
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
    $("#page3").hide();
    $("#page1").show();
    Days=[];
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////      
    if ($("#lundi").is(":checked")){
      console.log("hellow");
      Days.push(1);
    }
    if ($("#mardi").is(":checked")){
      Days.push(2);
    }
    if ($("#mercredi").is(":checked")){
      Days.push(3);
    }
    if ($("#jeudi").is(":checked")){
      Days.push(4);
    }
    if ($("#vendredi").is(":checked")){
      Days.push(5);
    }
    if ($("#samedi").is(":checked")){
      Days.push(6);
    }
    if ($("#dimanche").is(":checked")){
      Days.push(0);
    }
  });


  window.onload=function() {
            horloge('div_horloge');
               constellation.connection.start();
            $("#boutonstop").on("click", ()=>{
            //message stopAlarm envoyé a constellation
            player.pause();
            $("#page0").css("display", "none"); 
            constellation.server.sendMessage({ Scope: 'Package', Args: ['Brain'] }, 'StopAlarm');
            });
            //$("#page0").get(0).style.display = "none";
            $("#boutonsnooze").on("click", ()=>{
            //message SnoozeAlarm envoyé a constellation
            player.pause();
            $("#page0").css("display", "none"); 
            constellation.server.sendMessage({ Scope: 'Package', Args: ['Brain'] }, 'SnoozeAlarm');
            });

            $("#next1").click(function() {
              $("#page1").hide();
              $("#page2").show();
            })
            $("#back").click(function() {
              $("#page2").hide();
              $("#page1").show();
            })
            $("#back2").click(function() {
              $("#page3").hide();
              $("#page2").show();
            })
        };

        function horloge(el) {
            if(typeof el=="string") { el = document.getElementById(el); }
            function actualiser() {
              var date = new Date();
              var str = date.getHours();
              str += ':'+(date.getMinutes()<10?'0':'')+date.getMinutes();
              el.innerHTML = str;
            }
            actualiser();
            setInterval(actualiser,1000);
        }