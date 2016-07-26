var gulp = require('gulp');
var browserSync = require('browser-sync');

// this task utilizes the browsersync plugin
// to create a dev server instance
// at http://localhost:9000
gulp.task('serve', ['build'], function(done) {
  browserSync({
    online: false,
    open: false,
    port: 9000,
    server: {
      baseDir: ['.'],
      middleware: function(req, res, next) {
        res.setHeader('Access-Control-Allow-Origin', '*');

        // Mock API calls
        if (req.url.indexOf('/api/') > -1) {
          console.log('[serve] responding ' + req.method + ' ' + req.originalUrl);

          var jsonResponseUri = req._parsedUrl.pathname + '/' + req.method + '.json';

          // Require file for logging purpose, if not found require will 
          // throw an exception and middleware will cancel the retrieve action
          var jsonResponse = require('../..' + jsonResponseUri);

          // Replace the original call with retrieving json file as reply
          req.url = jsonResponseUri;
          req.method = 'GET';
        }

        next();
      }
    }
  }, done);
});

// this task utilizes the browsersync plugin
// to create a dev server instance
// at http://localhost:9000
gulp.task('serve-bundle', ['bundle'], function(done) {
  browserSync({
    online: false,
    open: false,
    port: 9000,
    server: {
      baseDir: ['.'],
      middleware: function(req, res, next) {
        res.setHeader('Access-Control-Allow-Origin', '*');
        next();
      }
    }
  }, done);
});

// this task utilizes the browsersync plugin
// to create a dev server instance
// at http://localhost:9000
gulp.task('serve-export', ['export'], function(done) {
  browserSync({
    online: false,
    open: false,
    port: 9000,
    server: {
      baseDir: ['./export'],
      middleware: function(req, res, next) {
        res.setHeader('Access-Control-Allow-Origin', '*');
        next();
      }
    }
  }, done);
});
