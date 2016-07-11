﻿/*
This file in the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require('gulp');

gulp.task('default', function () {
    // place code for your default task here
});

gulp.task('copylib', function () {
    gulp.src("lib/wymeditor/dist/wymeditor/*")
    .pipe(gulp.dest("scripts/wymeditor/"));
});