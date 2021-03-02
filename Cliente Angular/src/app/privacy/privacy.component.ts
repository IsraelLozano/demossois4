import { Component, OnInit } from '@angular/core';
import { RepositoryService } from '../shared/services/repository.service';
import {  myClain } from '../_interfaces/company.model';

@Component({
  selector: 'app-privacy',
  templateUrl: './privacy.component.html',
  styleUrls: ['./privacy.component.css']
})
export class PrivacyComponent implements OnInit {
  public claims: myClain[];
  constructor(private _repository: RepositoryService) { }

  ngOnInit(): void {
    this.getClaims();
  }
  public getClaims = () =>{
    this._repository.getData('api/companies/getprivado')
    .subscribe(res => {

      this.claims = res as myClain[]

      console.log(this.claims);
    });
  }
}
