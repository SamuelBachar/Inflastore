(function () {
    $(document).ready(function () {
        $.get('/WeatherForeCast', function (data) {
            console.log(data);
        });
    });
})();