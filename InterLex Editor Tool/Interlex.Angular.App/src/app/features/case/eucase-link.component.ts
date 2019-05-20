import { Component, OnInit, Input } from '@angular/core';
import { HttpService } from '../../core/services/http.service';
import { IEucaseLink, importance, INomenclature, yesNoRule} from '../../models/case-editor.model';

@Component({
    selector: 'app-eucase-link',
    templateUrl: 'eucase-link.component.html',
    styleUrls: [
        '../../../../node_modules/bootstrap/dist/css/bootstrap.min.css',
        './eucase-link.component.css']
})
export class EucaseLinkComponent implements OnInit {
    importances: INomenclature[];
    prelRuling: INomenclature[];

    courts: INomenclature[] = [
        { label: 'Court of Justice', code: 'C' },
        { label: 'General Court', code: 'T' },
        { label: 'Civil Service Tribunal', code: 'F' }
    ];
    docTypes: INomenclature[] = [
        { label: 'Judgment', code: 'J' },
        { label: 'Order', code: 'O' },
        { label: 'Opinion of the Advocate-General', code: 'C' }
    ];


    constructor(private http: HttpService) {
        this.importances = importance;
        this.prelRuling = yesNoRule;
    }
    @Input() euCaseLink: IEucaseLink;
    @Input() isDoc: boolean;
    @Input() isEuDoc: boolean;
    ngOnInit() {

    }

    onChange(event) {
        if (this.euCaseLink.docNumber && this.euCaseLink.year && this.euCaseLink.court && this.euCaseLink.docType) {
            const dtLetter = this.euCaseLink.docType.code;
            const cLetter = this.euCaseLink.court.code;
            const docNumber = this.euCaseLink.docNumber.toString().padStart(4, '0');
            this.euCaseLink.celex = '6' + this.euCaseLink.year + cLetter + dtLetter + docNumber;
        } else {
            this.euCaseLink.celex = '';
        }
    }
}
