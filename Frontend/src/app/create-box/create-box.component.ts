import {Component, OnInit} from '@angular/core';
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {HttpClient, HttpErrorResponse} from "@angular/common/http";
import {environment} from "../../environments/environment";
import {firstValueFrom} from "rxjs";
import {Router} from "@angular/router";
import {Box} from "../models";

@Component({
  selector: 'app-create-box',
  templateUrl: './create-box.component.html',
  styleUrls: ['./create-box.component.scss'],
})
export class CreateBoxComponent  implements OnInit {

  constructor(private http: HttpClient, private router: Router) {
    price: ['', [Validators.required, Validators.pattern(/^\d+(\.\d{1,2})?$/)]]
  }

  ngOnInit() {}

  title = new FormControl('', [
    Validators.required,
    Validators.minLength(5),
    Validators.maxLength(50)
  ]);

  description = new FormControl('', [
    Validators.maxLength(500)
  ]);

  price = new FormControl('', [
    Validators.required
  ]);

  imageUrl = new FormControl('', [
    Validators.required,
  ]);

  width = new FormControl('', [
    Validators.required,
  ]);

  length = new FormControl('', [
    Validators.required,
  ]);

  height = new FormControl('', [
    Validators.required,
  ]);

  formControlGroup = new FormGroup({
    title: this.title,
    description: this.description,
    price: this.price,
    imageUrl: this.imageUrl,
    width: this.width,
    length: this.length,
    height: this.height
  });


  async createBox() {
    const call = this.http.post<Box>(environment.apiBaseUrl + "createBox", this.formControlGroup.value)
    await firstValueFrom(call).then(
      (response) =>{
        this.router.navigate(['/details/' + response.productID]);
      },
      (error: HttpErrorResponse) => {
        console.log(error)
      }
    );
  }


}
