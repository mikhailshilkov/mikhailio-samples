import {inject} from 'aurelia-framework';
import {HttpClient} from 'aurelia-fetch-client';
import 'fetch';

@inject(HttpClient)
export class Products {
  heading = 'Products';
  products = [];

  constructor(http) {
    http.configure(config => {
      config
        .useStandardConfiguration();
    });

    this.http = http;
  }

  activate() {
    return this.http.fetch('/api/products')
      .then(response => response.json())
      .then(products => this.products = products);
  }
}
