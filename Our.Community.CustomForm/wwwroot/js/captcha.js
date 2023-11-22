        $(document).on("click","#btn-captcha", function(e) {
            e.preventDefault();
            captchaCheck($("#Captcha").val(), function(data) {
                if (data) {
                    $("#captcha-check").hide();
                    $("#captcha-refresh").hide();
                    $("#form-submit").show();
                    $("#form-submit-label").show();

                } else {
                    $("#Captcha").val("");
                    $("#Captcha").attr("placeholder", "Incorrect, please try again.");
                }
            });
        });
        $(document).on("click","#captcha-refresh", function(e) {
            e.preventDefault();
            $.ajax({
                url: "/umbraco/surface/formssurface/refreshcaptcha",
                success: function (data) {
                    $("#captchaimage-div").html(data);
                }
            });
        });
        function captchaCheck(answer, callback) {
            $.ajax({
                url: '/checkcaptcha/' + answer,
                type: 'GET',
                success: function(data) {
                    if (callback) {callback(data); }
                },
                error: function(jqXHR, exception) {
                    console.log(jqXHR);
                    return false;
                }
            });
        };