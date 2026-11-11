// Не показувати консольне вікно у release-збірці на Windows.
#![cfg_attr(not(debug_assertions), windows_subsystem = "windows")]

fn main() {
    updater_lib::run()
}
