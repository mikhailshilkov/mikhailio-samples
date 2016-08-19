import {NavigationInstruction, Next} from 'aurelia-router';
import {Microsoft} from 'ApplicationInsights';

@autoinject
export class AppInsights {
  private client: Microsoft.ApplicationInsights.AppInsights;

  constructor() {
    let snippet = {
      config: {
        instrumentationKey: 'YOUR INSTRUMENTATION KEY'
      },
      queue: []
    };
    let init = new Microsoft.ApplicationInsights.Initialization(snippet);
    this.client = init.loadAppInsights();
  }

  run(routingContext: NavigationInstruction, next: Next): Promise<any> {
    this.client.trackPageView(routingContext.fragment, window.location.href);
    return next();
  }
}
