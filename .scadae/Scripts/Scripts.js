function CodeEqualNumber(code, number) {
    return code.value == number.value;
}

var old_kolichestvoVkluchenii = 0;
var index_vklucheniya = 8;

function kolichestvoVkluchenii(array, id) {
    var data = array.value;

    if (data[0] == id.value) {
        old_kolichestvoVkluchenii = data[index_vklucheniya];
        return data[index_vklucheniya];
    }

    else {
        return old_kolichestvoVkluchenii;
    }

}


var old_narabotka = 0;
var index_narabotka = 5;

function narabotka(array, id) {
    var data = array.value;

    if (data[0] == id.value) {
        old_narabotka = data[index_narabotka];
        return data[index_narabotka];
    }

    else {
        return old_narabotka;
    }

}

var old_do_servisa_ostalos = 0;
var index_do_servisa_ostalos = 6;
var status = "";
var message = "";
var index_status = 7;

function doServisaOstalos(array, id) {

    var data = array.value;
    var kod_statusa = data[index_status];

    if (data[0] == id.value) {
        old_do_servisa_ostalos = data[index_do_servisa_ostalos];

        if (kod_statusa == 1) {
            message = "ДО НАЧАЛА СЕРВИСА \n" + old_do_servisa_ostalos + " ЧАСОВ";
            return "ДО НАЧАЛА СЕРВИСА \n" + old_do_servisa_ostalos + " ЧАСОВ";
        }

        if (kod_statusa == 2) {
            message = "ДО ОКОНЧАНИЯ СЕРВИСА \n" + old_do_servisa_ostalos + " ЧАСОВ";
            return "ДО ОКОНЧАНИЯ СЕРВИСА \n" + old_do_servisa_ostalos + " ЧАСОВ";

        }

        if (kod_statusa == 8) {
            message = "СЕРВИС ПРОСРОЧЕН НА \n" + old_do_servisa_ostalos + " ЧАСОВ";
            return "СЕРВИС ПРОСРОЧЕН НА \n" + old_do_servisa_ostalos + " ЧАСОВ";

        }

        message = "ДО НАЧАЛА СЕРВИСА \n" + old_do_servisa_ostalos + " ЧАСОВ";
        return "ДО НАЧАЛА СЕРВИСА \n" + old_do_servisa_ostalos + " ЧАСОВ";
    }

    else {
        return message;
    }

}


var arhiv_do_okonchania_servisa = -1;
function ExpressionDlyaArhiva_servis_nastal(array, id, signal_dlya_sohranenia) {

    var data = array.value;
    var kod_statusa = data[index_status];

    if (data[0] == id.value && kod_statusa == 2) {
        arhiv_do_okonchania_servisa = data[index_do_servisa_ostalos];
    }

    if (signal_dlya_sohranenia.value == 2) {

        return arhiv_do_okonchania_servisa;
    }

    else {
        return -2;
    }

}

var arhiv_servis_prosrochen_na = -1;
function ExpressionDlyaArhiva_servis_prosrochen(array, id, signal_dlya_sohranenia) {

    var data = array.value;
    var kod_statusa = data[index_status];

    if (data[0] == id.value && kod_statusa == 8) {
        arhiv_servis_prosrochen_na = data[index_do_servisa_ostalos];
    }

    if (signal_dlya_sohranenia.value == 8) {

        return arhiv_servis_prosrochen_na;
    }

    else {
        return -2;
    }

}

var arhiv_do_nachala_servisa = -1;
function ExpressionDlyaArhiva_servis_skoro(array, id, signal_dlya_sohranenia) {

    var data = array.value;
    var kod_statusa = data[index_status];

    if (data[0] == id.value && kod_statusa == 1) {
        arhiv_do_nachala_servisa = data[index_do_servisa_ostalos];
    }

    if (signal_dlya_sohranenia.value == 1) {

        return arhiv_do_nachala_servisa;
    }

    else {
        return -2;
    }

}


var arhiv_servis_projden = -1;
function ExpressionDlyaArhiva_servis_projden(array, id, signal_dlya_sohranenia) {

    var data = array.value;
    var kod_statusa = data[index_status];

    if (data[0] == id.value && (kod_statusa == 4 || kod_statusa == 16)) {
        arhiv_servis_projden = data[index_do_servisa_ostalos];
    }

    if (signal_dlya_sohranenia.value == 4 || signal_dlya_sohranenia.value == 16) {

        return arhiv_servis_projden;
    }

    else {
        return -2;
    }

}

var old_status = "";
function doServisaOstalosStatus(array, id) {

    var data = array.value;
    var kod_statusa = data[index_status];

    if (data[0] == id.value) {

        if (kod_statusa == 1) {
            old_status = "СЕРВИС СКОРО";
            return "СЕРВИС СКОРО";
        }

        if (kod_statusa == 2) {
            old_status = "СЕРВИС НАСТАЛ";
            return "СЕРВИС НАСТАЛ";
        }


        if (kod_statusa == 8) {
            old_status = "СЕРВИС ПРОСРОЧЕН";
            return "СЕРВИС ПРОСРОЧЕН";
        }

        if (kod_statusa == 4 || kod_statusa == 16) {
            old_status = "СЕРВИС ПРОЙДЕН";
            return "СЕРВИС ПРОЙДЕН";
        }
    }
    return old_status;
}


var old_color = "Gray";
function doServisaOstalosStatusColor(array, id) {

    var data = array.value;
    var kod_statusa = data[index_status];

    if (data[0] == id.value) {

        if (kod_statusa == 1) {
            old_color = "Gold";
            //return "СЕРВИС СКОРО";
            return "Gold";
        }

        if (kod_statusa == 2) {
            old_color = "Orange";
            //old_status = "СЕРВИС НАСТАЛ";
            return "Orange";
        }


        if (kod_statusa == 8) {
            old_color = "Red";
            //old_status = "СЕРВИС ПРОСРОЧЕН";
            return "Red";
        }

        if (kod_statusa == 4 || kod_statusa == 16) {
            old_color = "LimeGreen";
            //old_status = "СЕРВИС ПРОЙДЕН";
            return "LimeGreen";
        }
    }
    return old_color;
}


function ValueInRange(tag_value, tag_min, tag_max) {
    var val = tag_value.dirtyValue;
    if ((val >= tag_min.value) && (val <= tag_max.value))
        return false;
    else
        return true;
}

function ValueInRangeSkorostPodachi(tag_value, tag_min, tag_max) {
    var val = tag_value.dirtyValue;
    if ((val >= (-1) *  tag_max.value) && (val <= tag_max.value))
        return false;
    else
        return true;
}

function ValueInRangeTextSkorostPodachi(tag_min, tag_max, float_value_format_digit, units) {
    return "Введите значение в диапазоне " + (-1) * tag_max.value.toFixed(float_value_format_digit.value) + "..." + tag_max.value.toFixed(float_value_format_digit.value) + " " + units.value;
}


function MaxValue(tag_value, tag_min, tag_max) {
    var val = tag_value.dirtyValue;
    if (val > tag_min.value)
        return false;
    else
        return true;
}

function MinValue(tag_value, tag_min, tag_max) {
    var val = tag_value.dirtyValue;
    if (val <= tag_max.value)
        return false;
    else
        return true;
}

function kz_or_bypass(kz, bypass) {
    if (kz.value || bypass.value) return true;

    return false;
}

function Logic_OR(bool1, bool2) {
    if (bool1.value || bool2.value) return true;

    return false;
}

function ValueInRangeText(tag_min, tag_max, float_value_format_digit, units) {
    return "Введите значение в диапазоне " + tag_min.value.toFixed(float_value_format_digit.value) + "..." + tag_max.value.toFixed(float_value_format_digit.value) + " " + units.value;
}

var do_it = false;
var arhiv_narabotka = 0;
function ExpressionDlyaArhivaServisa(array, id, signal_dlya_sohranenia) {
    var data = array.value;

    if (data[0] == id.value) {
        arhiv_narabotka = data[index_narabotka];
    }

    if (signal_dlya_sohranenia.value == true) {

        return arhiv_narabotka;
    }

    else return -1;
}

function KTU_1000V(QS1, QS2) {
    return QS1.value && QS1.value;
}

function NKU_400V(Q1, QF1, QF2) {
    return (Q1.value && QF1.value) || QF2.value;
}

function SyncControlAllowed(pump1_dc, pump2_dc) {
    return pump1_dc.value && pump2_dc.value;
}

function Summary(tank1, isEnableTank1,
    tank2, isEnableTank2,
    tank3, isEnableTank3,
    tank4, isEnableTank4,
    tank5, isEnableTank5,
    tank6, isEnableTank6,
    tank7, isEnableTank7,
    tank8, isEnableTank8) {

    var level1 = 0;

    if (isEnableTank1.value == true)
        level1 = tank1.value;

    var level2 = 0;

    if (isEnableTank2.value == true)
        level2 = tank2.value;

    var level3 = 0;

    if (isEnableTank3.value == true)
        level3 = tank3.value;

    var level4 = 0;

    if (isEnableTank4.value == true)
        level4 = tank4.value;

    var level5 = 0;

    if (isEnableTank5.value == true)
        level5 = tank5.value;

    var level6 = 0;

    if (isEnableTank6.value == true)
        level6 = tank6.value;

    var level7 = 0;

    if (isEnableTank7.value == true)
        level7 = tank7.value;

    var level8 = 0;

    if (isEnableTank8.value == true)
        level8 = tank8.value;

    return level1 + level2 + level3 + level4 + level5 + level6 + level7 + level8;
}

function GreaterThan(level, max_level) {

    if (level.value > max_level.value)
        return true;

    return false;
}


function LessThan(level, min_level) {

    if (level.value < min_level.value)
        return true;

    return false;
}

function WriteValueChanged(tag_value,tag_read_value) {
    var val = tag_value.dirtyValue;
    if (val != tag_read_value.value)
        return tag_value.dirtyValue;
    else
        return tag_read_value;
}


function DrivePower(tag_moment, tag_rpm) {
    
    return (tag_moment.value * tag_rpm.value / 9.55);
}

function KruPower(tag_Q, tag_P) {

    return Math.sqrt((tag_Q.value * tag_Q.value) + (tag_P.value * tag_P.value));
}

function DoNotWriteByIndex(tagValue) {
    if (tagValue.newValue == -1 || tagValue.newValue == 255)
        return true;

    return false;
}

function GetScaleMaxForMsabGas(tagValue) {
    if (tagValue.value == 2)
        return 100;

    else
        return 50;
}

function GetVisibilityMsabGas(tagValue) {
    if (tagValue.value == 1)
        return false;

    return true;

}