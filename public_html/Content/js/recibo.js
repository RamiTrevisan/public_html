function imprimirRecibo() {
    var dd = {
        content: [
            {
                columns: [
                    {
                        image:
                            'data:image/png;base64,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',
                        width: 50,
                    },
                    [
                        {
                            text: 'CENTENARIO M.S.D. y B.',
                            color: '#333333',
                            width: '*',
                            fontSize: 20,
                            bold: true,
                            alignment: 'center',
                            margin: [0, 10, 0, 0],
                        },
                        {
                            text: 'San Martin y Castellanos, San Jose de la Esquina',
                            bold: false,
                            fontSize: 12,
                            alignment: 'center',
                            color: '#333333',
                            width: 100,
                        }
                    ],
                ],
            },
            {
                columns: [
                    {
                        text: 'Recibimos de',
                        color: '#aaaaab',
                        bold: true,
                        fontSize: 14,
                        alignment: 'left',
                        margin: [0, 20, 0, 5],
                    },
                ],
            },
            {
                columns: [
                    {
                        text: 'GABRIEL MARTINEZ\n DNI N° 39855211',
                        bold: true,
                        color: '#333333',
                        alignment: 'left',
                    },
                    {
                        text: 'LISANDRO DE LA TORRE\n LOS MOLINOS',
                        bold: true,
                        color: '#333333',
                        alignment: 'left',
                    },
                ],
            },
            '\n\n',
            {
                layout: {
                    defaultBorder: false,
                    hLineWidth: function (i, node) {
                        return 1;
                    },
                    vLineWidth: function (i, node) {
                        return 1;
                    },
                    hLineColor: function (i, node) {
                        if (i === 1 || i === 0) {
                            return '#bfdde8';
                        }
                        return '#eaeaea';
                    },
                    vLineColor: function (i, node) {
                        return '#eaeaea';
                    },
                    hLineStyle: function (i, node) {
                        // if (i === 0 || i === node.table.body.length) {
                        return null;
                        //}
                    },
                    // vLineStyle: function (i, node) { return {dash: { length: 10, space: 4 }}; },
                    paddingLeft: function (i, node) {
                        return 10;
                    },
                    paddingRight: function (i, node) {
                        return 10;
                    },
                    paddingTop: function (i, node) {
                        return 2;
                    },
                    paddingBottom: function (i, node) {
                        return 2;
                    },
                    fillColor: function (rowIndex, node, columnIndex) {
                        return '#fff';
                    },
                },
                table: {
                    headerRows: 1,
                    widths: ['*', 80],
                    body: [
                        [
                            {
                                text: 'Detalle',
                                fillColor: '#f5f5f5',
                                border: [false, true, false, true],
                                margin: [0, 5, 0, 5],
                                textTransform: 'uppercase',
                            },
                            {
                                text: 'Monto',
                                border: [false, true, false, true],
                                alignment: 'right',
                                fillColor: '#f5f5f5',
                                margin: [0, 5, 0, 5],
                                textTransform: 'uppercase',
                            },
                        ],
                        [
                            {
                                text: 'Cuota societaria Activos - Abril 2021',
                                border: [false, false, false, true],
                                margin: [0, 5, 0, 5],
                                alignment: 'left',
                            },
                            {
                                border: [false, false, false, true],
                                text: '$250',
                                fillColor: '#f5f5f5',
                                alignment: 'right',
                                margin: [0, 5, 0, 5],
                            }
                        ],
                        [
                            {
                                text: 'Cuota societaria Activos - Mayo 2021',
                                border: [false, false, false, true],
                                margin: [0, 5, 0, 5],
                                alignment: 'left',
                            },
                            {
                                text: '$250',
                                border: [false, false, false, true],
                                fillColor: '#f5f5f5',
                                alignment: 'right',
                                margin: [0, 5, 0, 5],
                            }
                        ],
                        [
                            {
                                text: 'TOTAL',
                                border: [false, false, false, true],
                                margin: [0, 5, 0, 5],
                                alignment: 'left',
                            },
                            {
                                text: '$500',
                                border: [false, false, false, true],
                                fillColor: '#f5f5f5',
                                alignment: 'right',
                                margin: [0, 5, 0, 5],
                            }
                        ],
                    ],
                },
            },
            '\n\n\n\n',
            {
                text: '__________________________________\nFIRMA',
                bold: true,
                color: '#333333',
                alignment: 'center',
            },
        ],
        styles: {
            notesTitle: {
                fontSize: 10,
                bold: true,
                margin: [0, 50, 0, 3],
            },
            notesText: {
                fontSize: 10,
            },
        },
        defaultStyle: {
            columnGap: 20,
        },
    };

    pdfMake.createPdf(dd).open()
}