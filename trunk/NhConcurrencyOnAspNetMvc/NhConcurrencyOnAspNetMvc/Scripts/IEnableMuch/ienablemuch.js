
(function ($) {

    $.fn.modelValidation = function (data) {

        var form = $(this);

        if (form.size() > 1) {
            alert('There are more than one form in this page. Contact the dev to indicate the specific form that needed be validated');
            return;
        }



        if (!data.ModelState.IsValid) {
            showInvalid(data, form);
        }
        else
            clearInvalid();

        return data.ModelState.IsValid;

    }



    function clearInvalid(form) {
        $('div[data-valmsg-summary]', form).removeClass().addClass('validation-summary-valid');
    }

    function showInvalid(data, form) {

        valSum = $('div[data-valmsg-summary]', form);


        if (valSum.attr('data-valmsg-summary') == undefined)
            valSum = null;



        var errorList = null;
        if (valSum) {
            valSum.removeClass().addClass('validation-summary-errors');
            errorList = $('> ul', valSum);
            errorList.html('');
        }


        $.each(data.ModelState.PropertyErrors, function () {

            if (valSum && valSum.attr('data-valmsg-summary') == 'true') {
                errorList.append($('<li />').text(this.ErrorMessage));
            }


            var propVal = $('span[data-valmsg-for=' + this.Key + ']', form);
            propVal.removeClass().addClass('field-validation-error').text(this.ErrorMessage);

            var prop = $('input[name=' + this.Key + ']', form);
            prop.addClass('input-validation-error');
        });



        if (valSum) {
            $.each(data.ModelState.ModelErrors, function () {
                errorList.append($('<li />').text(this.ErrorMessage));
            });
        }

    }//showInvalid


})(jQuery);



