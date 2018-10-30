var gulp = require("gulp"),
    fs = require("fs"),
    less = require("gulp-less");

gulp.task("mainless", function () {
    return gulp.src('Styles/site.less')
        .pipe(less())
        .pipe(gulp.dest('wwwroot/css'));
});

gulp.task("lipaspotwebuiless",
    function() {
        return gulp.src('Styles/lipaspotwebui.site.less')
            .pipe(less())
            .pipe(gulp.dest('wwwroot/css'));
    });