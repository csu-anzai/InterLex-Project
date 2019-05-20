import {Component, OnInit} from '@angular/core';
import {HttpService} from '../../core/services/http.service';

@Component({
  selector: 'app-home',
  templateUrl: 'home.component.html',
  styles: []
})
export class HomeComponent implements OnInit {


  constructor(private http: HttpService) {
  }

  ngOnInit() {

  }

  doStuff() {
    const result = this.http.get('/Case/GetWholeTree').subscribe(x => console.log(x));
  }
}
