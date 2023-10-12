import { Component, OnInit } from '@angular/core';
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {DataService} from "../data.service";
import {HttpClient} from "@angular/common/http";
import {environment} from "../../environments/environment";
import {Box} from "../models";
import {firstValueFrom} from "rxjs";
import {Router} from "@angular/router";

@Component({
  selector: 'app-update-box',
  templateUrl: './update-box.component.html',
  styleUrls: ['./update-box.component.scss'],
})
export class UpdateBoxComponent  implements OnInit {

  constructor(public dataService: DataService,
              private http: HttpClient,
              private router: Router) { }

  ngOnInit() {}

  productID = new FormControl(this.dataService.currentBox.productID)

  title = new FormControl(this.dataService.currentBox.title, [
    Validators.required,
    Validators.minLength(5),
    Validators.maxLength(50)
  ]);

  description = new FormControl(this.dataService.currentBox.description, [
    Validators.maxLength(500)
  ]);

  price = new FormControl(this.dataService.currentBox.price, [
    Validators.required
  ]);

  imageURL = new FormControl(this.dataService.currentBox.imageURL, [
    Validators.required,
  ]);

  width = new FormControl(this.dataService.currentBox.width, [
    Validators.required,
  ]);

  length = new FormControl(this.dataService.currentBox.length, [
    Validators.required,
  ]);

  height = new FormControl(this.dataService.currentBox.height, [
    Validators.required,
  ]);

  formControlGroup = new FormGroup({
    productID: this.productID,
    title: this.title,
    description: this.description,
    price: this.price,
    imageUrl: this.imageURL,
    width: this.width,
    length: this.length,
    height: this.height
  });

  async updateBox() {
    const call = this.http.put<Box>(environment.apiBaseUrl + "products", this.formControlGroup.value);
    const result = await firstValueFrom<Box>(call);
    this.dataService.currentBox = result;

    this.router.navigate(['details/' + this.dataService.currentBox.productID]).then(() => window.location.reload());
  }
}
